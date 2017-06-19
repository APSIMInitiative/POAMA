using System;
using System.Data;
using System.IO;

namespace POAMA.CallService
{
    class Program
    {
        /// <summary>Main entry point.</summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            DataTable data;
            POAMA.Service.Forecast forecast = new POAMA.Service.Forecast();
            if (args.Length == 1 && args[0] == "/Update")
                forecast.Update();
            else
            {
                ServiceReference1.ForecastClient forecastClient = new ServiceReference1.ForecastClient();
                data = forecastClient.GetDataTable(41023, new DateTime(2016, 08, 03), true);
                Console.WriteLine(data.TableName.ToString());
            }
        }
    }
}
