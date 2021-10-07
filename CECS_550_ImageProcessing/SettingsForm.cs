using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MosaicMason
{
    public partial class SettingsForm : Form
    {
        public int outputWidth = 0;
        public int outputHeight = 0;
        public float aspectRatio = 0;
        public int numThreads = 1;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }

        public void InitializeOutputFields(int masterWidth, int masterHeight)
        {
            int minDim = Math.Min(masterWidth, masterHeight);
            int height, width;
            aspectRatio = (1.0f * masterWidth) / masterHeight;
            if (minDim < 2000)
            {

                width = (int)Math.Round(2000 * aspectRatio);
                height = 2000;
            }
            else
            {
                width = masterWidth;
                height = masterHeight;
            }
            outputWidthTextBox.Text = "" + width;
            outputWidth = width;
            outputHeightTextBox.Text = "" + height;
            outputHeight = height;
            outputWidthTextBox.Enabled = true;
            outputHeightTextBox.Enabled = true;
        }

        private void outputWidthTextBox_TextChanged(object sender, EventArgs e)
        {
            if (outputWidthTextBox.Text.Length > 0)
            {
                try
                {
                    outputWidth = int.Parse(outputWidthTextBox.Text);
                    outputHeight = (int)Math.Round(outputWidth / aspectRatio);
                    outputHeightTextBox.Text = "" + outputHeight;
                }
                catch (FormatException)
                {
                    // do nothing.
                }
            }
        }

        private void outputHeightTextBox_TextChanged(object sender, EventArgs e)
        {
            if (outputHeightTextBox.Text.Length > 0)
            {
                try
                {
                    outputHeight = int.Parse(outputHeightTextBox.Text);
                    outputWidth = (int)Math.Round(outputHeight * aspectRatio);
                    outputWidthTextBox.Text = "" + outputWidth;
                }
                catch (FormatException)
                {
                    // do nothing.
                }
            }
        }

        private void numThreadsTextBox1_TextChanged(object sender, EventArgs e)
        {
            int processorCount = Environment.ProcessorCount;
            if (numThreadsTextBox1.Text.Length > 0)
            {
                try
                {
                    numThreads = int.Parse(numThreadsTextBox1.Text);
                    if (numThreads > processorCount)
                    {
                        string message = "You have "+ processorCount + " processors, are you sure you want to use " + numThreads + " threads?";
                        bool acceptTooManyThreads = MessageBox.Show(message, "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes;
                        numThreads = acceptTooManyThreads ? numThreads : processorCount; 
                    }
                    numThreads = (numThreads <= 0) ? 1 : numThreads;
                    numThreadsTextBox1.Text = numThreads.ToString();
                }
                catch (FormatException)
                {
                    // do nothing.
                }
            }
        }
    }
}
