namespace CECS_550_ImageProcessing
{
    partial class MosaicMasonForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MosaicMasonForm));
            this.smiButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.createMosaicButton = new System.Windows.Forms.Button();
            this.scpButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imagesPerColumnTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.subdivisionLevelTextBox2 = new System.Windows.Forms.MaskedTextBox();
            this.grayscaleCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.aspectRatioLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.aspectRatioValueLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.candidateImageLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.CandidateImageValueLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toneMappingCheckbox = new System.Windows.Forms.CheckBox();
            this.CUDA_checkBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // smiButton
            // 
            this.smiButton.Location = new System.Drawing.Point(12, 38);
            this.smiButton.Name = "smiButton";
            this.smiButton.Size = new System.Drawing.Size(114, 23);
            this.smiButton.TabIndex = 0;
            this.smiButton.Text = "Select Master Image";
            this.smiButton.UseVisualStyleBackColor = true;
            this.smiButton.Click += new System.EventHandler(this.smiButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBox1.Location = new System.Drawing.Point(12, 81);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(640, 480);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // createMosaicButton
            // 
            this.createMosaicButton.Enabled = false;
            this.createMosaicButton.Location = new System.Drawing.Point(264, 38);
            this.createMosaicButton.Name = "createMosaicButton";
            this.createMosaicButton.Size = new System.Drawing.Size(84, 23);
            this.createMosaicButton.TabIndex = 2;
            this.createMosaicButton.Text = "Create Mosaic";
            this.createMosaicButton.UseVisualStyleBackColor = true;
            this.createMosaicButton.Click += new System.EventHandler(this.createMosaicButton_Click);
            // 
            // scpButton
            // 
            this.scpButton.Enabled = false;
            this.scpButton.Location = new System.Drawing.Point(132, 38);
            this.scpButton.Name = "scpButton";
            this.scpButton.Size = new System.Drawing.Size(126, 23);
            this.scpButton.TabIndex = 1;
            this.scpButton.Text = "Select Candidate Pool";
            this.scpButton.UseVisualStyleBackColor = true;
            this.scpButton.Click += new System.EventHandler(this.scpButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(663, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.advancedOptionsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // advancedOptionsToolStripMenuItem
            // 
            this.advancedOptionsToolStripMenuItem.Name = "advancedOptionsToolStripMenuItem";
            this.advancedOptionsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.advancedOptionsToolStripMenuItem.Text = "Advanced Options";
            this.advancedOptionsToolStripMenuItem.Click += new System.EventHandler(this.advancedOptionsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // imagesPerColumnTextBox1
            // 
            this.imagesPerColumnTextBox1.Location = new System.Drawing.Point(533, 29);
            this.imagesPerColumnTextBox1.Mask = "00000";
            this.imagesPerColumnTextBox1.Name = "imagesPerColumnTextBox1";
            this.imagesPerColumnTextBox1.PromptChar = ' ';
            this.imagesPerColumnTextBox1.Size = new System.Drawing.Size(25, 20);
            this.imagesPerColumnTextBox1.TabIndex = 8;
            this.imagesPerColumnTextBox1.Text = "32";
            this.imagesPerColumnTextBox1.ValidatingType = typeof(int);
            this.imagesPerColumnTextBox1.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.maskedTextBox1_MaskInputRejected);
            this.imagesPerColumnTextBox1.TextChanged += new System.EventHandler(this.maskedTextBox1_TextChanged);
            // 
            // subdivisionLevelTextBox2
            // 
            this.subdivisionLevelTextBox2.Location = new System.Drawing.Point(533, 55);
            this.subdivisionLevelTextBox2.Mask = "00000";
            this.subdivisionLevelTextBox2.Name = "subdivisionLevelTextBox2";
            this.subdivisionLevelTextBox2.PromptChar = ' ';
            this.subdivisionLevelTextBox2.Size = new System.Drawing.Size(25, 20);
            this.subdivisionLevelTextBox2.TabIndex = 9;
            this.subdivisionLevelTextBox2.Text = "3";
            this.subdivisionLevelTextBox2.ValidatingType = typeof(int);
            this.subdivisionLevelTextBox2.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.maskedTextBox2_MaskInputRejected);
            this.subdivisionLevelTextBox2.TextChanged += new System.EventHandler(this.maskedTextBox2_TextChanged);
            // 
            // grayscaleCheckBox
            // 
            this.grayscaleCheckBox.AutoSize = true;
            this.grayscaleCheckBox.Location = new System.Drawing.Point(572, 32);
            this.grayscaleCheckBox.Name = "grayscaleCheckBox";
            this.grayscaleCheckBox.Size = new System.Drawing.Size(79, 17);
            this.grayscaleCheckBox.TabIndex = 10;
            this.grayscaleCheckBox.Text = "Grayscale?";
            this.grayscaleCheckBox.UseVisualStyleBackColor = true;
            this.grayscaleCheckBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.UpdateProgressFormLocation);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(428, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Images per column:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(438, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Subdivision level:";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aspectRatioLabel,
            this.aspectRatioValueLabel,
            this.candidateImageLabel,
            this.CandidateImageValueLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 551);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(663, 22);
            this.statusStrip1.TabIndex = 13;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // aspectRatioLabel
            // 
            this.aspectRatioLabel.Name = "aspectRatioLabel";
            this.aspectRatioLabel.Size = new System.Drawing.Size(107, 17);
            this.aspectRatioLabel.Text = "Image aspect ratio:";
            // 
            // aspectRatioValueLabel
            // 
            this.aspectRatioValueLabel.Name = "aspectRatioValueLabel";
            this.aspectRatioValueLabel.Size = new System.Drawing.Size(17, 17);
            this.aspectRatioValueLabel.Text = "--";
            // 
            // candidateImageLabel
            // 
            this.candidateImageLabel.Name = "candidateImageLabel";
            this.candidateImageLabel.Size = new System.Drawing.Size(240, 17);
            this.candidateImageLabel.Text = "Candidate images w/ matching aspect ratio:";
            // 
            // CandidateImageValueLabel
            // 
            this.CandidateImageValueLabel.Name = "CandidateImageValueLabel";
            this.CandidateImageValueLabel.Size = new System.Drawing.Size(17, 17);
            this.CandidateImageValueLabel.Text = "--";
            // 
            // toneMappingCheckbox
            // 
            this.toneMappingCheckbox.AutoSize = true;
            this.toneMappingCheckbox.Location = new System.Drawing.Point(572, 57);
            this.toneMappingCheckbox.Name = "toneMappingCheckbox";
            this.toneMappingCheckbox.Size = new System.Drawing.Size(98, 17);
            this.toneMappingCheckbox.TabIndex = 14;
            this.toneMappingCheckbox.Text = "ToneMapping?";
            this.toneMappingCheckbox.UseVisualStyleBackColor = true;
            // 
            // CUDA_checkBox
            // 
            this.CUDA_checkBox.AutoSize = true;
            this.CUDA_checkBox.Location = new System.Drawing.Point(572, 9);
            this.CUDA_checkBox.Name = "CUDA_checkBox";
            this.CUDA_checkBox.Size = new System.Drawing.Size(84, 17);
            this.CUDA_checkBox.TabIndex = 15;
            this.CUDA_checkBox.Text = "Use CUDA?";
            this.CUDA_checkBox.UseVisualStyleBackColor = true;
            this.CUDA_checkBox.CheckedChanged += new System.EventHandler(this.CUDA_CheckedChanged);
            // 
            // MosaicMasonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 573);
            this.Controls.Add(this.CUDA_checkBox);
            this.Controls.Add(this.toneMappingCheckbox);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.grayscaleCheckBox);
            this.Controls.Add(this.subdivisionLevelTextBox2);
            this.Controls.Add(this.imagesPerColumnTextBox1);
            this.Controls.Add(this.scpButton);
            this.Controls.Add(this.createMosaicButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.smiButton);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MosaicMasonForm";
            this.Text = "Mosaic Mason";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.Move += new System.EventHandler(this.Form1_Move);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button smiButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button createMosaicButton;
        private System.Windows.Forms.Button scpButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advancedOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.MaskedTextBox imagesPerColumnTextBox1;
        private System.Windows.Forms.MaskedTextBox subdivisionLevelTextBox2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.CheckBox grayscaleCheckBox;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel aspectRatioLabel;
        private System.Windows.Forms.ToolStripStatusLabel aspectRatioValueLabel;
        private System.Windows.Forms.ToolStripStatusLabel candidateImageLabel;
        private System.Windows.Forms.ToolStripStatusLabel CandidateImageValueLabel;
        private System.Windows.Forms.CheckBox toneMappingCheckbox;
        private System.Windows.Forms.CheckBox CUDA_checkBox;
    }
}

