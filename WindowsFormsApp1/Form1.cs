using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string selectedDirectory;
        string[] filePaths;
        decimal[] resultRange = { -2.5m, 2.5m, -1.2m, 1.2m };
        const int Coefficient = 600;
        decimal result;

        static Exception noFilesFound = new Exception("Selected folder does not containt any .ASC files!");
        static Exception invalidDirectory = new Exception("Invalid Directory!");

        public Form1()
        {
            InitializeComponent();
        }

        // buttonDirectory
        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                filePaths = GetFilesFromDirectory();

            } catch (FileNotFoundException ex)
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
            List<decimal> fileResults = ComputeFiles(filePaths);
            result = ComputeResults(fileResults);
            //MessageBox.Show(result.ToString());
            generateLogFile(fileResults);

        }

        private void generateLogFile(List<decimal> fileResults)
        {
            StringBuilder sb = new StringBuilder();
            foreach (decimal fileResult in fileResults)
            {
                sb.AppendLine(fileResult.ToString());
            }
            File.WriteAllText(selectedDirectory + "log.csv", sb.ToString());
        }

        private decimal ComputeResults(List<decimal> fileResults)
        {
            decimal resultSummary = fileResults.Sum();
            decimal endResult = decimal.Divide(resultSummary, fileResults.Count);
            return endResult;
        }

        private List<decimal> ComputeFiles(string[] filePaths)
        {
            List<decimal> resultList = new List<decimal>();
            progressBar.Maximum = filePaths.Count();

            foreach (string filePath in filePaths)
            {
                progressBar.Increment(1);
                StreamReader reader = new StreamReader(filePath);

                string line;
                string[] rowResult = new string[3];
                string[] timeSplit = new string[2];

                decimal valueSummary = 0;
                int valueCounter = 0;

                // Skipping initial lines
                for (int i = 0; i < 9; i++)
                {
                    line = reader.ReadLine();
                }

                // From 1st result
                while ((line = reader.ReadLine()) != null)
                {
                    //rowResult: [ time, X, Y ]
                    //timeSplit: [ hh:mm:ss, ms ]
                    rowResult = line.Split(';');
                    timeSplit = rowResult[0].Split(',');

                    //convert  ',' to '.' @numberFormatException
                    string dataValue = rowResult[2];
                    dataValue = dataValue.Replace(',', '.');

                    decimal value = Decimal.Parse(dataValue);
                    if (value > resultRange[0] && value < resultRange[1])
                    {
                        if (value < resultRange[2] || value > resultRange[3])
                        {
                            valueSummary += Math.Abs(value);
                            valueCounter++;
                        }
                    }
                }
                decimal valueSummaryAvg = decimal.Divide(valueSummary, valueCounter);
                decimal result = decimal.Divide(valueSummaryAvg, Coefficient);
                resultList.Add(result);
            }
            return resultList;
        }

        private string[] GetFilesFromDirectory()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "# Select Folder Directory #";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                selectedDirectory = Directory.GetCurrentDirectory();
                string[] files = Directory.GetFiles(fbd.SelectedPath, "*.ASC");
                if (files.Length > 0)
                {
                    buttonStart.Enabled = true;
                    MessageBox.Show(fbd.SelectedPath);
                    return files;
                }
                throw new FileNotFoundException("Haven't found any *.ASC files in Selected Directory");
            }
            throw new InvalidDirectorySelected("No Directory Selected.");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
