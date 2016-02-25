// -----------------------------------------------------------------------
// <copyright file="Forecast.cs" company="APSIM Initiative">
//     Copyright (c) APSIM Initiative
// </copyright>
// -----------------------------------------------------------------------
namespace POAMA.Service
{
    using System.Data;
    using System.IO;
    using MathWorks.MATLAB.NET.Arrays;
    using System.Reflection;
    using APSIM.Cloud.Shared;
    using System;
    using APSIM.Shared.Utilities;
    using System.Collections.Generic;
    using System.ServiceModel.Web;

    public class Forecast : IForecast
    {
        /// <summary>Get a rainfall forecast for the specified station number.</summary>
        /// <param name="stationNumber">The SILO station number.</param>
        /// <returns>A datatable with date and rain as columns.</returns>
        public DataTable GetRainfallForecast(int stationNumber, DateTime nowDate)
        {
            // Get SILO data and write to a temporary file.
            DateTime startDate = new DateTime(1980, 1, 1);
            MemoryStream dataStream = WeatherFile.ExtractMetStreamFromSILO(stationNumber, startDate, nowDate);
            string siloFileName = Path.ChangeExtension(Path.GetTempFileName(), ".sim");
            File.WriteAllBytes(siloFileName, dataStream.GetBuffer());

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
                return data;
            }
            else
                return null;

        }

        /// <summary>Get a rainfall forecast for the specified station number.</summary>
        /// <param name="stationNumber">The SILO station number.</param>
        /// <returns>A datatable with date and rain as columns.</returns>
        public Stream GetRainfallForecastFile(int stationNumber, DateTime nowDate, bool rainOnly)
        {
            // Get SILO data and write to a temporary file.
            DateTime startDate = new DateTime(1980, 1, 1);
            MemoryStream dataStream = WeatherFile.ExtractMetStreamFromSILO(stationNumber, startDate, nowDate);
            string siloFileName = Path.ChangeExtension(Path.GetTempFileName(), ".sim");
            File.WriteAllBytes(siloFileName, dataStream.GetBuffer());

            using (StreamWriter writer = new StreamWriter(@"D:\Websites\Test.txt"))
            {
                ChangeToWorkingDirectory();

                // Run the MatLab script over the temporary SILO file.           
                POAMAforecast.Class1 forecast = new POAMAforecast.Class1();

                MWArray metFile = new MWCharArray(siloFileName);
                MWArray rainfallOnly = new MWNumericArray((double)1);
                if (!rainOnly)
                    rainfallOnly = new MWNumericArray((double)0);
                MWArray writeFiles = new MWNumericArray((double)1.0);
                MWArray startDay = new MWNumericArray(Convert.ToDouble(nowDate.Day));
                MWArray startMonth = new MWNumericArray(Convert.ToDouble(nowDate.Month));
                MWArray startYear = new MWNumericArray(Convert.ToDouble(nowDate.Year));
                forecast.calsite(metFile, rainfallOnly, writeFiles, startDay, startMonth, startYear);

                string forecastFileName = siloFileName.Replace(".sim", "") + "_" + nowDate.Year + ".sim";

                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
                if (File.Exists(forecastFileName))
                {
                    // Read in the forecast data.
                    byte[] bytes = File.ReadAllBytes(forecastFileName);
                    return new MemoryStream(bytes);
                }
                else
                    return null;
            }

        }

        /// <summary>Get a rainfall forecast for the specified station number.</summary>
        /// <param name="stationNumber">The SILO station number.</param>
        /// <returns>A datatable with date and rain as columns.</returns>
        public int Test(int stationNumber)
        {
            return stationNumber;
        }

        /// <summary>Update the NetCDF files behind the forecast system.</summary>
        public void UpdatePOAMA()
        {
            ChangeToWorkingDirectory();
            POAMAforecast.Class1 forecast = new POAMAforecast.Class1();
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
