namespace CECS_550_ImageProcessing
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.outputWidthTextBox = new System.Windows.Forms.MaskedTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.outputHeightTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numThreadsTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // outputWidthTextBox
            // 
            this.outputWidthTextBox.Enabled = false;
            this.outputWidthTextBox.Location = new System.Drawing.Point(128, 22);
            this.outputWidthTextBox.Mask = "00000";
            this.outputWidthTextBox.Name = "outputWidthTextBox";
            this.outputWidthTextBox.PromptChar = ' ';
            this.outputWidthTextBox.Size = new System.Drawing.Size(40, 20);
            this.outputWidthTextBox.TabIndex = 0;
            this.outputWidthTextBox.ValidatingType = typeof(int);
            this.outputWidthTextBox.TextChanged += new System.EventHandler(this.outputWidthTextBox_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.outputHeightTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.outputWidthTextBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 93);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output Options";
            // 
            // outputHeightTextBox
            // 
            this.outputHeightTextBox.Enabled = false;
            this.outputHeightTextBox.Location = new System.Drawing.Point(128, 60);
            this.outputHeightTextBox.Mask = "00000";
            this.outputHeightTextBox.Name = "outputHeightTextBox";
            this.outputHeightTextBox.PromptChar = ' ';
            this.outputHeightTextBox.Size = new System.Drawing.Size(40, 20);
            this.outputHeightTextBox.TabIndex = 3;
            this.outputHeightTextBox.ValidatingType = typeof(int);
            this.outputHeightTextBox.TextChanged += new System.EventHandler(this.outputHeightTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Output Image Height:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Output Image Width:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numThreadsTextBox1);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(12, 112);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(268, 76);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Advanced Options";
            // 
            // numThreadsTextBox1
            // 
            this.numThreadsTextBox1.Location = new System.Drawing.Point(128, 20);
            this.numThreadsTextBox1.Mask = "00000";
            this.numThreadsTextBox1.Name = "numThreadsTextBox1";
            this.numThreadsTextBox1.PromptChar = ' ';
            this.numThreadsTextBox1.Size = new System.Drawing.Size(21, 20);
            this.numThreadsTextBox1.TabIndex = 1;
            this.numThreadsTextBox1.Text = "1";
            this.numThreadsTextBox1.ValidatingType = typeof(int);
            this.numThreadsTextBox1.TextChanged += new System.EventHandler(this.numThreadsTextBox1_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Number of Threads:";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 200);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.Text = "Advanced Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.MaskedTextBox outputHeightTextBox;
        public System.Windows.Forms.MaskedTextBox outputWidthTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MaskedTextBox numThreadsTextBox1;
    }
}