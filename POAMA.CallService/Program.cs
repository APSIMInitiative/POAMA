using System;
using System.IO;

namespace POAMA.CallService
{
    class Program
    {
        /// <summary>Main entry point.</summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            POAMA.Service.Forecast forecast = new POAMA.Service.Forecast();
            if (args.Length == 1 && args[0] == "/Update")
                forecast.Update();
            else
            {
                Stream data = forecast.Get(91009, DateTime.Today, true);
                StreamReader reader = new StreamReader(data);
                Console.Write(reader.ReadToEnd());
            }
        }
    }
}
