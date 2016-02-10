using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using APSIM.Cloud.Shared;
using APSIM.Shared.Utilities;
using System.Data;

namespace test2
{
    class Program
    {
        /// <summary>Main entry point.</summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "/Update")
                UpdatePOAMA();
            else
            {
                // Get SILO data and write to a temporary file.
                DateTime startDate = new DateTime(1980, 1, 1);
                DateTime endDate = DateTime.Today;
                MemoryStream dataStream = WeatherFile.ExtractMetStreamFromSILO(91009, startDate, endDate);
                string siloFileName = Path.ChangeExtension(Path.GetTempFileName(), ".sim");
                File.WriteAllBytes(siloFileName, dataStream.GetBuffer());

                ChangeToWorkingDirectory();

                // Run the MatLab script over the temporary SILO file.           
                POAMAforecast.Class1 forecast = new POAMAforecast.Class1();

                MWArray metFile = new MWCharArray(siloFileName);
                MWArray startMonth = new MWNumericArray(Convert.ToDouble(DateTime.Today.Month));
                MWArray startDay = new MWNumericArray(Convert.ToDouble(DateTime.Today.Day));
                MWArray rainOnly = new MWNumericArray((double)1.0);
                MWArray writeFiles = new MWNumericArray((double)1.0);
                MWArray lastYearOnly = new MWNumericArray((double)1.0);
                MWArray[] returnData = forecast.calsite(5, metFile, startMonth, startDay, rainOnly, writeFiles, lastYearOnly);



                string forecastFileName = siloFileName.Replace(".sim", "") + "_" + DateTime.Today.Year + ".sim";
                if (File.Exists(forecastFileName))
                {
                    // Read in the forecast data.
                    ApsimTextFile f = new ApsimTextFile();
                    f.Open(forecastFileName);
                    DataTable data = f.ToTable();

                    // Add a date column and remove unwanted columns
                    AddDateColumn(data);
                    RemoveColumn(data, "year");
                    RemoveColumn(data, "day");
                    RemoveColumn(data, "radn");
                    RemoveColumn(data, "maxt");
                    RemoveColumn(data, "mint");
                }
            }
        }

        /// <summary>Update POAMA</summary>
        private static void UpdatePOAMA()
        {
            POAMAforecast.Class1 forecast = new POAMAforecast.Class1();
            ChangeToWorkingDirectory();
            forecast.update_e24_2014on();
        }

        /// <summary>Change to current working directory to the NetCDF directory.</summary>
        private static void ChangeToWorkingDirectory()
        {
            // Working directory will be our bin directory + 'Working'
            string workingDirectory = @"C:\inetpub\wwwroot\Services\bin\NetCDF";
            if (Directory.Exists(workingDirectory))
                Directory.SetCurrentDirectory(workingDirectory);  // On Bob.
            else
                Directory.SetCurrentDirectory(@"G:\");            // testing folder on Deans computer.
        }

        /// <summary>Remove a column from the datatable if it exists.</summary>
        /// <param name="data">The data table.</param>
        /// <param name="columnName">The column to remove.</param>
        private static void RemoveColumn(DataTable data, string columnName)
        {
            int column = data.Columns.IndexOf(columnName);
            if (column != -1)
                data.Columns.RemoveAt(column);
        }

        /// <summary>Add a date column as the first column of a datatable.</summary>
        /// <param name="data">The data table to add the date to.</param>
        private static void AddDateColumn(DataTable data)
        {
            List<DateTime> dates = new List<DateTime>();
            foreach (DataRow Row in data.Rows)
                dates.Add(DataTableUtilities.GetDateFromRow(Row));
            DataTableUtilities.AddColumnOfObjects(data, "Date", dates.ToArray());
            data.Columns["Date"].SetOrdinal(0);
        }
    }
}
