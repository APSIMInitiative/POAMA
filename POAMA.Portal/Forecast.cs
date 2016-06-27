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
    using System.ServiceModel.Activation;
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Forecast : IForecast
    {
        /// <summary>Get a rainfall forecast for the specified station number.</summary>
        /// <param name="stationNumber">The SILO station number.</param>
        /// <returns>A datatable with date and rain as columns.</returns>
        public DataTable GetDataTable(int stationNumber, DateTime nowDate, bool rainOnly)
        {
            Stream inStream = Get(stationNumber, nowDate, rainOnly);
            if (inStream != null)
            {
                // Read in the forecast data.
                ApsimTextFile f = new ApsimTextFile();
                f.Open(inStream);
                return f.ToTable();
            }
            else
                return null;
        }

        /// <summary>Get a rainfall forecast for the specified station number.</summary>
        /// <param name="stationNumber">The SILO station number.</param>
        /// <returns>Stream of data.</returns>
        public Stream Get(int stationNumber, DateTime nowDate, bool rainOnly)
        {
            // Get SILO data and write to a temporary file.
            DateTime startDate = new DateTime(1981, 1, 1);           
            MemoryStream dataStream = WeatherFile.ExtractMetStreamFromSILO(stationNumber, startDate, DateTime.Now);
            string siloFileName = GetTemporaryFileName();
            string forecastFileName = null;

            try
            {
                File.WriteAllBytes(siloFileName, dataStream.ToArray());

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

                forecastFileName = siloFileName.Replace(".sim", "") + "_" + nowDate.Year + ".sim";

                // Get rid of temporary file.
                File.Delete(siloFileName);

                if (WebOperationContext.Current != null && WebOperationContext.Current.OutgoingRequest != null)
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
            finally
            {
                if (File.Exists(siloFileName))
                    File.Delete(siloFileName);
                if (forecastFileName != null && File.Exists(forecastFileName))
                    File.Delete(forecastFileName);
            }
        }

        /// <summary>Get a temporary file on E drive - the SSD drive on the server.</summary>
        /// <returns></returns>
        public string GetTemporaryFileName()
        {
            string tempFileName = Path.GetTempFileName();
            File.Delete(tempFileName);
            return @"E:\Data\" + Path.GetFileNameWithoutExtension(tempFileName) + ".sim";
        }

        /// <summary>Update the NetCDF files behind the forecast system.</summary>
        public void Update()
        {
            ChangeToWorkingDirectory();
            POAMAforecast.Class1 forecast = new POAMAforecast.Class1();
            forecast.update_e24_2014on();
        }

        /// <summary>Change to current working directory to the NetCDF directory.</summary>
        private static void ChangeToWorkingDirectory()
        {
            string workingDirectory = @"E:\Data";
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
