using System;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Reader reader;

        public Form1()
        {
            InitializeComponent();
        }

        // buttonDirectory
        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.reader = new Reader(progressBar);
                GetFilesFromDirectory();
                buttonStart.Enabled = true;
            }
            catch (FileNotFoundException ex)
            {
                System.Diagnostics.Trace.WriteLine("FileNotFound");
                MessageBox.Show("Haven't found any *.ASC files in Selected Directory \nTry other folder.", "FileFotFound Exception",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            } catch (InvalidDirectorySelected ex)
            {
                System.Diagnostics.Trace.WriteLine("InvalidDirectorySelected");
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.reader.readFiles();
            this.reader.GenerateLogFile();
        }

        void GetFilesFromDirectory()
            // Setting: SelectedDirectory & FilePaths
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "# Select Folder Directory #";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                reader.SelectedDirectory = fbd.SelectedPath;
                reader.FilePaths = Directory.GetFiles(reader.SelectedDirectory, "*.ASC");

                if (reader.FilePaths.Length == 0)
                {
                    throw new FileNotFoundException("Haven't found any *.ASC files in Selected Directory");
                }

                if (reader.SelectedDirectory == null)
                {
                    throw new InvalidDirectorySelected("No Directory Selected.");
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
