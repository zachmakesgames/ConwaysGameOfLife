using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        //Delegates for safe cross thread form control
        private delegate void SetImageDelegate(Bitmap image);
        private delegate void UpdateImageDelegate();
        private delegate void UpdateGenerationDelegate(int generation);


        //Board dimensions
        int DIMENSIONX = 128;
        int DIMENSIONY = 128;


        //Generation arrays
        //b1 is current gen
        //b2 is next gen
        int[,] b1, b2;


        //The image to display generation data on
        Bitmap img;


        //Wether or not the thread is running
        bool running = false;

        //Determines if clicking on the board should set or clear a pixel
        bool setBits = true;


        //The thread to run the simulation on
        Thread runThread;

        //The current generation number
        int generation = 0;


        //The base time to sleep between generations
        //this value modifies the slider to produce the value below
        int sleepTimeBase = 100;

        //The time in milliseconds to sleep between generations
        int threadSleepTime = 1000;//1 second

        //The number of worker threads to spawn.
        //Note: This does NOT include the control thread!
        int threadCount = 8;

        //The list of worker threads.
        List<Thread> workerThreads = new List<Thread>();


        //The work queue for worker threads to consume data from.
        Queue<WorkOrder> WorkQueue = new Queue<WorkOrder>();



        //The lock used to ensure that no two workers have the same piece of data.
        object queueLock = new object();

        //The size each chunk of data should be.
        //The number of work chunks would be DIMENSIONX / workChunkSize
        int workChunkSize = 8;

        //The barriers used to syncronize worker threads and control thread.
        Barrier PrepBarrier;
        Barrier WorkBarrier;
        Barrier ProcessBarrier;


        /// <summary>
        /// The initializer of the form. Sets up the data grids, creates the image, and starts the work threads.
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            b1 = new int[DIMENSIONX, DIMENSIONY];
            b2 = new int[DIMENSIONX, DIMENSIONY];
            ClearArray(b1);
            ClearArray(b2);

            img = new Bitmap(DIMENSIONX, DIMENSIONY);

            runThread = new Thread(new ThreadStart(LockedThreadProcedure));
            runThread.Start();

            PrepBarrier = new Barrier(threadCount + 1);
            WorkBarrier = new Barrier(threadCount + 1);
            ProcessBarrier = new Barrier(threadCount + 1);
        }


        /// <summary>
        /// Updates the image on form load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = img;
            pictureBox1.Update();
        }


        /// <summary>
        /// Sets or clears a bit in the grid and updates the image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!running)
            {

                //Get the coordinates of the click.
                //This section scales the click down to fit within the
                //dimensions of the acutal image.
                MouseEventArgs MEA = (MouseEventArgs)e;
                Point clickLocation = MEA.Location;
                int X = clickLocation.X;
                int Y = clickLocation.Y;
                float scaleX = (float)X / (float)512;
                float scaleY = (float)Y / (float)512;

                float newX = scaleX * (float)DIMENSIONX;
                float newY = scaleY * (float)DIMENSIONY;
                X = (int)newX;
                Y = (int)newY;


                
                Graphics g = Graphics.FromImage(img);
                if (setBits)
                {
                    g.FillRectangle(Brushes.Black, X, Y, 1, 1);

                    b1[X, Y] = 1;
                }
                else
                {
                    g.FillRectangle(Brushes.White, X, Y, 1, 1);

                    b1[X, Y] = 0;
                }
                
                pictureBox1.Image = img;
                pictureBox1.Update();
                
            }
        }


        /// <summary>
        /// Clears the array to all 0s.
        /// </summary>
        /// <param name="array">The array to clear.</param>
        private void ClearArray(int[,] array)
        {
            for(int i = 0; i < DIMENSIONX; ++i)
            {
                for(int j = 0; j < DIMENSIONY; ++j)
                {
                    array[i, j] = 0;
                }
            }
        }


        /// <summary>
        /// Advances the simulation by one frame on button "stepButton" click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stepButton_Click(object sender, EventArgs e)
        {
            AdvanceOneFrame();
        }


        /// <summary>
        /// Loads array data into the display image.
        /// </summary>
        /// <param name="array">The array to load into the image.</param>
        private void LoadArrayIntoBitmap(int[,] array)
        {
            Graphics g = Graphics.FromImage(img);
            g.Clear(Color.White);

            for (int i = 0; i < DIMENSIONX; ++i)
            {
                for (int j = 0; j < DIMENSIONY; ++j)
                {
                    if(array[i,j] > 0) 
                    {
                        g.FillRectangle(Brushes.Black, i, j, 1, 1);
                    }
                }
            }
        }


        /// <summary>
        /// Advances the simulation by one frame.
        /// </summary>
        private void AdvanceOneFrame()
        {
            ++generation;
            UpdateGenerationLabel(generation);
            ClearArray(b2);
            for(int i = 0; i < DIMENSIONX; ++i)
            {
                for(int j = 0; j < DIMENSIONY; ++j)
                {
                    if(b1[i,j] == 0)
                    {
                        ProcessDeadPixel(i, j);
                    }
                    else
                    {
                        ProcessAlivePixel(i, j);
                    }
                }
            }

            LoadArrayIntoBitmap(b2);

            //Transfer the data from b2 to b1
            ClearArray(b1);
            for(int i = 0; i < DIMENSIONX; ++i)
            {
                for(int j = 0; j < DIMENSIONY; ++j)
                {
                    b1[i, j] = b2[i, j];
                }
            }

            SetPictureBoxImage(img);
            UpdatePictureBox();

        }


        /// <summary>
        /// Computes the next generation at location i, j based on the current generation being dead.
        /// </summary>
        /// <param name="i">The row of the pixel to process.</param>
        /// <param name="j">The column of the pixel to process.</param>
        private void ProcessDeadPixel(int i, int j)
        {
            int surroundSum = GetSurroundSum(i, j);

            if(surroundSum == 3)
            {
                b2[i, j] = 1;
            }
        }


        /// <summary>
        /// Computes the next generation at location i, j based on the current generation being alive.
        /// </summary>
        /// <param name="i">The row of the pixel to process.</param>
        /// <param name="j">The column of the pixel to process.</param>
        private void ProcessAlivePixel(int i, int j)
        {
            int surroundSum = GetSurroundSum(i, j);
            if(surroundSum == 2 || surroundSum == 3)
            {
                b2[i, j] = 1;
            }
            
        }


        /// <summary>
        /// Clears the current generation array and updates the display image. Also sets the current
        /// generation to 0.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateArray_Click(object sender, EventArgs e)
        {
            Stop();
            ClearArray(b1);
            generation = 0;
            UpdateGenerationLabel(generation);
            Graphics g = Graphics.FromImage(img);
            g.Clear(Color.White);
            SetPictureBoxImage(img);
            UpdatePictureBox();
        }


        /// <summary>
        /// Swaps to setting a pixel to alive when clicking on the image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setPixel_Click(object sender, EventArgs e)
        {
            setBits = true;
        }


        /// <summary>
        /// Swaps to setting a pixel to dead when clicking on the image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearPixel_Click(object sender, EventArgs e)
        {
            setBits = false;
        }


        /// <summary>
        /// Gets the number of alive cells surrounding the cell at i, j.
        /// </summary>
        /// <param name="i">The row of the cell to check.</param>
        /// <param name="j">The column of the cell to check.</param>
        /// <returns>The number of surrounding cells that are alive.</returns>
        private int GetSurroundSum(int i, int j)
        {
            int surroundSum = 0;

            if(i - 1 >= 0)
            {
                surroundSum += b1[i - 1, j];
            }
            if(i + 1 < DIMENSIONX)
            {
                surroundSum += b1[i + 1, j];
            }


            if (j - 1 >= 0)
            {
                surroundSum += b1[i, j-1];
            }
            if (j + 1 < DIMENSIONY)
            {
                surroundSum += b1[i, j+1];
            }


            if(i-1 >= 0 && j-1 >= 0)
            {
                surroundSum += b1[i - 1, j - 1];
            }

            if (i + 1 < DIMENSIONX && j + 1 < DIMENSIONY)
            {
                surroundSum += b1[i + 1, j + 1];
            }

            if(i-1 >= 0 && j + 1 < DIMENSIONY)
            {
                surroundSum += b1[i - 1, j + 1];
            }
            if(i+1 < DIMENSIONX && j-1 >= 0)
            {
                surroundSum += b1[i + 1, j - 1];
            }

            return surroundSum;
        }


        /// <summary>
        /// A thread safe method to set the image displayed in the picture box.
        /// </summary>
        /// <param name="image">The new image to display</param>
        private void SetPictureBoxImage(Bitmap image)
        {
            if (pictureBox1.InvokeRequired)
            {
                SetImageDelegate d = new SetImageDelegate(SetPictureBoxImage);
                pictureBox1.Invoke(d, new object[] { image });
            }
            else
            {
                pictureBox1.Image = image;
            }
        }


        /// <summary>
        /// A thread safe method to update the picture box.
        /// </summary>
        private void UpdatePictureBox()
        {
            if (pictureBox1.InvokeRequired)
            {
                UpdateImageDelegate d = new UpdateImageDelegate(UpdatePictureBox);
                pictureBox1.Invoke(d, new object[] { });
            }
            else
            {
                pictureBox1.Update();
            }
        }

        /// <summary>
        /// Gracefully stops all running worker threads before the program terminates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            running = false;
            runThread.Abort();
            runThread.Join();
        }

        /// <summary>
        /// Stops the simulation.
        /// </summary>
        private void Stop()
        {
            running = false;
            toolStripStatusLabel1.Text = "Stopped";
        }


        /// <summary>
        /// Starts the simulation.
        /// </summary>
        private void Start()
        {
            running = true;
            toolStripStatusLabel1.Text = "Running";
        }


        /// <summary>
        /// Handles starting the simulation when "startButton" is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, EventArgs e)
        {
            Start();
        }


        /// <summary>
        /// Handles stopping the simulation when "stopButton" is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, EventArgs e)
        {
            Stop();
        }


        /// <summary>
        /// Updates the simulation speed when "speedTrackBar" is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void speedTrackBar_Scroll(object sender, EventArgs e)
        {
            threadSleepTime = (10 - speedTrackBar.Value) * sleepTimeBase;
        }


        /// <summary>
        /// A thread safe method to set the text of "generationNumLabel".
        /// </summary>
        /// <param name="generation">The generation number to display.</param>
        private void UpdateGenerationLabel(int generation)
        {
            if (generationNumLabel.InvokeRequired)
            {
                UpdateGenerationDelegate d = new UpdateGenerationDelegate(UpdateGenerationLabel);
                generationNumLabel.Invoke(d, new object[] { generation });
            }
            else
            {
                generationNumLabel.Text = "Generation " + generation.ToString();
            }
            
            

        }

        /// <summary>
        /// The thread procedure that controls all worker threads.
        /// </summary>
        private void LockedThreadProcedure()
        {
            try
            {

                //Set up and start the worker threads.
                for(int i = 0; i < threadCount; ++i)
                {
                    Thread t = new Thread(ConsumerThreadProcedure);
                    t.Start(i);
                    workerThreads.Add(t);
                }

                while (true)
                {
                    while (running)
                    {
                        Console.WriteLine("Setting up the work orders");
                        //set up the work load.
                        ClearArray(b2);

                        Stopwatch clock = new Stopwatch();

                        clock.Start();

                        int numberChunks = DIMENSIONX / workChunkSize;
                        for(int i = 0; i < numberChunks; ++i)
                        {
                            WorkOrder w = new WorkOrder();
                            w.rowCount = workChunkSize;
                            w.rowIndex = i * workChunkSize;
                            WorkQueue.Enqueue(w);
                        }


                        PrepBarrier.SignalAndWait();
                        Console.WriteLine("Waiting for workers to finish");
                        WorkBarrier.SignalAndWait();


                        Console.WriteLine("Processing data");
                        //Process the results from the worker threads.

                        ++generation;
                        LoadArrayIntoBitmap(b2);


                        ClearArray(b1);
                        for (int i = 0; i < DIMENSIONX; ++i)
                        {
                            for (int j = 0; j < DIMENSIONY; ++j)
                            {
                                b1[i, j] = b2[i, j];
                            }
                        }


                        UpdateGenerationLabel(generation);
                        SetPictureBoxImage(img);
                        UpdatePictureBox();

                        clock.Stop();
                        long elapsedtime = clock.ElapsedMilliseconds;
                        if (elapsedtime < (long)threadSleepTime)
                        {
                            long delta = (long)threadSleepTime - elapsedtime;
                            Thread.Sleep((int)delta);
                        }

                        ProcessBarrier.SignalAndWait();
                    }
                }
            }
            catch(ThreadAbortException ex)
            {
                //If this thread is aborted, we need to also stop all the worker threads.
                foreach(Thread t in workerThreads)
                {
                    t.Abort();
                    t.Join();
                }
                return;
            }
        }

        /// <summary>
        /// Handles loading the demo patter when the button "loadPatternButton" is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadPatternButton_Click(object sender, EventArgs e)
        {
            if (!running)
            {
                ClearArray(b1);
                b1[10, 10] = 1;
                b1[11, 10] = 1;
                b1[10, 11] = 1;
                b1[11, 11] = 1;

                b1[20, 10] = 1;
                b1[20, 11] = 1;
                b1[20, 12] = 1;

                b1[21, 9] = 1;
                b1[21, 13] = 1;
                b1[22, 8] = 1;
                b1[23, 8] = 1;
                b1[22, 14] = 1;
                b1[23, 14] = 1;

                b1[24, 11] = 1;

                b1[25, 9] = 1;
                b1[25, 13] = 1;
                b1[26, 10] = 1;
                b1[26, 11] = 1;
                b1[26, 12] = 1;

                b1[27, 11] = 1;

                b1[30, 10] = 1;
                b1[30, 9] = 1;
                b1[30, 8] = 1;
                b1[31, 10] = 1;
                b1[31, 9] = 1;
                b1[31, 8] = 1;
                b1[32, 7] = 1;
                b1[32, 11] = 1;

                b1[34, 11] = 1;
                b1[34, 12] = 1;
                b1[34, 7] = 1;
                b1[34, 6] = 1;

                b1[44, 8] = 1;
                b1[45, 8] = 1;
                b1[44, 9] = 1;
                b1[45, 9] = 1;



                LoadArrayIntoBitmap(b1);
                SetPictureBoxImage(img);
                UpdatePictureBox();
            }
        }


        /// <summary>
        /// The procedure used to perform work on the array.
        /// </summary>
        /// <param name="data">The ID of the thread cast to an object.</param>
        private void ConsumerThreadProcedure(object data)
        {
            int myID = (int)data;

            try
            {
                while (true)
                {
                    PrepBarrier.SignalAndWait();

                    //This second loop is necessary so that the worker thread keeps trying to
                    //consume data if there is data left on the queue. The previous version
                    //worked only because there were 8 threads and exactly 8 pieces of work to
                    //be done each frame. If the grid was increased in size it would break because
                    //not all of the work would be done each frame. This fixes that issue.
                    while (true)
                    {
                        WorkOrder w = null;
                        lock (queueLock)
                        {
                            if (WorkQueue.Count > 0)
                            {
                                //grab the work
                                w = WorkQueue.Dequeue();
                            }
                            else
                            {
                                break;
                            }
                        }
                        Console.WriteLine("Thread: " + myID.ToString() + " has acquired its lock and will begin working");



                        if (w != null)
                        {
                            //Process the work order.

                            for (int i = w.rowIndex; i < w.rowIndex + w.rowCount; ++i)
                            {
                                for (int j = 0; j < DIMENSIONY; ++j)
                                {
                                    if (b1[i, j] == 1)
                                    {
                                        ProcessAlivePixel(i, j);
                                    }
                                    else
                                    {
                                        ProcessDeadPixel(i, j);
                                    }
                                }
                            }
                        }
                    }

                    WorkBarrier.SignalAndWait();
                    ProcessBarrier.SignalAndWait();

                }
            }
            catch(ThreadAbortException ex)
            {
                return;
            }
        }
    
    }


    /// <summary>
    /// This class is used to add work to the queue for worker threads to work on.
    ///     rowIndex = The row in the array to start working on.
    ///     rowCount = The number of subsequent rows to process.
    /// </summary>
    public class WorkOrder
    {
        public int rowIndex;
        public int rowCount;
    }
}
