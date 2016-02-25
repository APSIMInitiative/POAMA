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

                DateTime nowDate = new DateTime(2015, 6, 1);
                ChangeToWorkingDirectory();

                // Run the MatLab script over the temporary SILO file.           
                POAMAforecast.Class1 forecast = new POAMAforecast.Class1();

                MWArray metFile = new MWCharArray(siloFileName);
                MWArray rainOnly = new MWNumericArray((double)1.0);
                MWArray writeFiles = new MWNumericArray((double)1.0);
                MWArray startDay = new MWNumericArray(Convert.ToDouble(nowDate.Day));
                MWArray startMonth = new MWNumericArray(Convert.ToDouble(nowDate.Month));
                MWArray startYear = new MWNumericArray(Convert.ToDouble(nowDate.Year));
                forecast.calsite(metFile, rainOnly, writeFiles, startDay, startMonth, startYear);
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
            string workingDirectory = @"D:\Websites\POAMA.Service\Data";
            Directory.SetCurrentDirectory(workingDirectory);
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
