using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class Reader
    {
        //initial data provided by user
        public string SelectedDirectory;
        public string[] FilePaths;
        public ProgressBar ProgressBar;

        //constance data
        public readonly decimal[] resultRange = { -2.5m, 2.5m, -1.2m, 1.2m };
        public const int Coefficient = 600;

        //program generated results
        public List<decimal> allResults;
        public decimal endResult;


        public Reader(ProgressBar progressBar)
        {
            this.ProgressBar = progressBar;
        }

        public void readFiles()
        {
            ComputeFilesResults();
            ComputeFinalResult();
        }

        public void ComputeFilesResults()
        {
            this.allResults = new List<decimal>();
            this.ProgressBar.Maximum = this.FilePaths.Length;

            foreach (string filePath in this.FilePaths)
            {
                this.ProgressBar.Increment(1);
                StreamReader reader = new StreamReader(filePath);

                string line;
                string[] rowResult = new string[3];
                string[] timeSplit = new string[2];

                decimal valueSummary = 0;
                int valueCounter = 0;

                // Skipping initial lines
                for (int i = 0; i < 9; i++)
                {
                    reader.ReadLine();
                }

                // From 1st result
                while ((line = reader.ReadLine()) != null)
                {
                    //rowResult: [ time, X, Y ]
                    //timeSplit: [ hh:mm:ss, ms ]
                    rowResult = line.Split(';');
                    timeSplit = rowResult[0].Split(',');
                    string dataValue = rowResult[2];

                    //convert  ',' to '.' @NumberFormatException
                    dataValue = dataValue.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    decimal value = decimal.Parse(dataValue, CultureInfo.CurrentCulture);

                    if (value > resultRange[0] && value < resultRange[1])
                    {
                        if (value < resultRange[2] || value > resultRange[3])
                        {
                            valueSummary += Math.Abs(value);
                            valueCounter++;
                        }
                    }
                }
                try
                {
                    decimal valueSummaryAvg = decimal.Divide(valueSummary, valueCounter);
                    decimal result = decimal.Divide(valueSummaryAvg, Coefficient);
                    allResults.Add(result);
                } catch (DivideByZeroException ex)
                {
                    MessageBox.Show("I haven't found any results in .ASC file(s)!", "ResultsFotFound Exception",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void ComputeFinalResult()
        {
            if (this.allResults.Count > 0)
            {
                this.endResult = decimal.Divide(this.allResults.Sum(), this.allResults.Count);
                MessageBox.Show("Read successful!");
            }
        }

        public void GenerateLogFile()
        {
            StringBuilder sb = new StringBuilder();
            string currentDate = GetCurrentDate();
            sb.AppendLine("Friction analyzer LOG:" + "\r\n");
            sb.AppendLine(currentDate);
            sb.AppendLine("Selected Directory -> " + this.SelectedDirectory + "\r\n");
            sb.AppendLine("Each File Result: ");

            foreach (decimal fileResult in this.allResults)
            {
                sb.AppendLine(fileResult.ToString());
            }
            sb.AppendLine("\r\n" + "Folder Average Result: " + this.endResult.ToString());

            File.WriteAllText(this.SelectedDirectory + "/log.csv", sb.ToString());
        }

        public string GetCurrentDate()
        {
            DateTime localDate = DateTime.Now;
            var culture = new CultureInfo("en-GB");
            string date = "Creation Date: " + localDate.ToString(culture);
            return date;
        }

    }
}
