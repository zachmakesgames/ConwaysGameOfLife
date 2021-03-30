namespace GameOfLife
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.stepButton = new System.Windows.Forms.Button();
            this.updateArray = new System.Windows.Forms.Button();
            this.setPixel = new System.Windows.Forms.Button();
            this.clearPixel = new System.Windows.Forms.Button();
            this.speedTrackBar = new System.Windows.Forms.TrackBar();
            this.speedLabel = new System.Windows.Forms.Label();
            this.generationNumLabel = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.loadPatternButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedTrackBar)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(512, 512);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(530, 21);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 1;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(530, 50);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 2;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // stepButton
            // 
            this.stepButton.Location = new System.Drawing.Point(530, 79);
            this.stepButton.Name = "stepButton";
            this.stepButton.Size = new System.Drawing.Size(75, 23);
            this.stepButton.TabIndex = 3;
            this.stepButton.Text = "Step";
            this.stepButton.UseVisualStyleBackColor = true;
            this.stepButton.Click += new System.EventHandler(this.stepButton_Click);
            // 
            // updateArray
            // 
            this.updateArray.Location = new System.Drawing.Point(530, 130);
            this.updateArray.Name = "updateArray";
            this.updateArray.Size = new System.Drawing.Size(75, 23);
            this.updateArray.TabIndex = 4;
            this.updateArray.Text = "Clear Array";
            this.updateArray.UseVisualStyleBackColor = true;
            this.updateArray.Click += new System.EventHandler(this.updateArray_Click);
            // 
            // setPixel
            // 
            this.setPixel.Location = new System.Drawing.Point(530, 211);
            this.setPixel.Name = "setPixel";
            this.setPixel.Size = new System.Drawing.Size(75, 23);
            this.setPixel.TabIndex = 5;
            this.setPixel.Text = "SetPixel";
            this.setPixel.UseVisualStyleBackColor = true;
            this.setPixel.Click += new System.EventHandler(this.setPixel_Click);
            // 
            // clearPixel
            // 
            this.clearPixel.Location = new System.Drawing.Point(530, 240);
            this.clearPixel.Name = "clearPixel";
            this.clearPixel.Size = new System.Drawing.Size(75, 23);
            this.clearPixel.TabIndex = 6;
            this.clearPixel.Text = "Clear Pixel";
            this.clearPixel.UseVisualStyleBackColor = true;
            this.clearPixel.Click += new System.EventHandler(this.clearPixel_Click);
            // 
            // speedTrackBar
            // 
            this.speedTrackBar.Location = new System.Drawing.Point(530, 320);
            this.speedTrackBar.Name = "speedTrackBar";
            this.speedTrackBar.Size = new System.Drawing.Size(104, 45);
            this.speedTrackBar.TabIndex = 7;
            this.speedTrackBar.Scroll += new System.EventHandler(this.speedTrackBar_Scroll);
            // 
            // speedLabel
            // 
            this.speedLabel.AutoSize = true;
            this.speedLabel.Location = new System.Drawing.Point(530, 304);
            this.speedLabel.Name = "speedLabel";
            this.speedLabel.Size = new System.Drawing.Size(38, 13);
            this.speedLabel.TabIndex = 8;
            this.speedLabel.Text = "Speed";
            // 
            // generationNumLabel
            // 
            this.generationNumLabel.AutoSize = true;
            this.generationNumLabel.Location = new System.Drawing.Point(527, 511);
            this.generationNumLabel.Name = "generationNumLabel";
            this.generationNumLabel.Size = new System.Drawing.Size(68, 13);
            this.generationNumLabel.TabIndex = 11;
            this.generationNumLabel.Text = "Generation 0";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 535);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(644, 22);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(51, 17);
            this.toolStripStatusLabel1.Text = "Stopped";
            // 
            // loadPatternButton
            // 
            this.loadPatternButton.Location = new System.Drawing.Point(530, 159);
            this.loadPatternButton.Name = "loadPatternButton";
            this.loadPatternButton.Size = new System.Drawing.Size(102, 23);
            this.loadPatternButton.TabIndex = 13;
            this.loadPatternButton.Text = "Load Test Pattern";
            this.loadPatternButton.UseVisualStyleBackColor = true;
            this.loadPatternButton.Click += new System.EventHandler(this.loadPatternButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 557);
            this.Controls.Add(this.loadPatternButton);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.generationNumLabel);
            this.Controls.Add(this.speedLabel);
            this.Controls.Add(this.speedTrackBar);
            this.Controls.Add(this.clearPixel);
            this.Controls.Add(this.setPixel);
            this.Controls.Add(this.updateArray);
            this.Controls.Add(this.stepButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Conway\'s Game of Life";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedTrackBar)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button stepButton;
        private System.Windows.Forms.Button updateArray;
        private System.Windows.Forms.Button setPixel;
        private System.Windows.Forms.Button clearPixel;
        private System.Windows.Forms.TrackBar speedTrackBar;
        private System.Windows.Forms.Label speedLabel;
        private System.Windows.Forms.Label generationNumLabel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button loadPatternButton;
    }
}

