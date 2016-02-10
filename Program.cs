using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POAMA
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(@"G:\");
            POAMAforecast.Class1 d = new POAMAforecast.Class1();
            MWArray metFile = new MWCharArray(@"Burnie.sim");
            MWArray startMonth = new MWNumericArray((double) 6.0);
            MWArray startDay = new MWNumericArray((double) 1.0);
            MWArray rainOnly = new MWNumericArray((double)1.0);
            MWArray writeFiles = new MWNumericArray((double)1.0);
            MWArray lastYearOnly = new MWNumericArray((double)1.0);
            d.calsite(metFile, startMonth, startDay, rainOnly, writeFiles, lastYearOnly);
        }
    }
}
