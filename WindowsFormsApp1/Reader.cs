using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
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
        public const decimal Radius = 0.0175m;
        public const int Coefficient = 600;

        //program generated results
        private decimal chartCoefficient;
        public List<decimal> LinesResults;
        public List<decimal> ChartCoefficientResults;
        public List<decimal> AllResults;
        public decimal EndResult;


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
            this.AllResults = new List<decimal>();
            this.ChartCoefficientResults = new List<decimal>();
            this.ProgressBar.Maximum = this.FilePaths.Length;

            //for each file in SelectedDirectory
            foreach (string filePath in this.FilePaths)
            {
                this.ProgressBar.Increment(1);
                StreamReader reader = new StreamReader(filePath);

                string line;
                string[] rowResult = new string[3];
                string[] timeSplit = new string[2];

                this.LinesResults = new List<decimal>();
                decimal valueSummaryForChartCoefficient = 0;
                decimal valueSummary = 0;
                int valueCounter = 0;

                // Skipping initial lines
                for (int i = 0; i < 9; i++)
                {
                    reader.ReadLine();
                }

                // for each line in a file, From 1st result
                while ((line = reader.ReadLine()) != null)
                {
                    //rowResult: [ time, X, Y ]
                    //timeSplit: [ hh:mm:ss, ms ] - timeSplit = rowResult[0].Split(',');
                    rowResult = line.Split(';');
                    string dataValue = rowResult[1];

                    //convert  ',' to '.' @NumberFormatException
                    dataValue = dataValue.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    decimal value = decimal.Parse(dataValue, CultureInfo.CurrentCulture);

                    //add line result
                    valueSummaryForChartCoefficient += value;
                    LinesResults.Add(value);
                    valueCounter++;
                }

                // compute ChartCoefficient
                chartCoefficient = decimal.Divide(valueSummaryForChartCoefficient, valueCounter);
                chartCoefficient = Math.Abs(chartCoefficient);
                ChartCoefficientResults.Add(chartCoefficient);

                // LINQ - for each element in LinesResults Add chartCoefficient to this element
                LinesResults = LinesResults.Select(x => x + chartCoefficient).ToList();

                foreach (decimal result in LinesResults)
                {
                    valueSummary += Math.Abs(result);
                }

                try
                {
                    //FileResult
                    decimal valueSummaryAvg = decimal.Divide(valueSummary, valueCounter);
                    valueSummaryAvg = decimal.Divide(valueSummaryAvg, Radius);
                    decimal result = decimal.Divide(valueSummaryAvg, Coefficient);
                    this.AllResults.Add(result);

                } catch (DivideByZeroException ex)
                {
                    MessageBox.Show("I haven't found any results in .ASC file(s)!", "ResultsNotFound Exception",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

        }

        private void ComputeFinalResult()
        {
            if (this.AllResults.Count > 0)
            {
                this.EndResult = decimal.Divide(this.AllResults.Sum(), this.AllResults.Count);
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

            foreach (decimal fileResult in this.AllResults)
            {
                sb.AppendLine(fileResult.ToString(CultureInfo.CurrentCulture));
            }
            sb.AppendLine("\r\n" + "Folder Average Result: " + this.EndResult.ToString(CultureInfo.CurrentCulture));

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
