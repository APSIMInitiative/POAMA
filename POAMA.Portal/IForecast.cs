// -----------------------------------------------------------------------
// <copyright file="IPOAMA.cs" company="APSIM Initiative">
//     Copyright (c) APSIM Initiative
// </copyright>
// -----------------------------------------------------------------------
namespace POAMA.Service
{
    using System;
    using System.Data;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    [ServiceContract]
    public interface IForecast
    {

        /// <summary>Get a rainfall forecast for the specified station number.</summary>
        /// <param name="stationNumber">The SILO station number.</param>
        /// <returns>A datatable with date and rain as columns.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/GetDataTable?stationNumber={stationNumber}&date={nowDate}&rainOnly={rainOnly}", BodyStyle = WebMessageBodyStyle.Bare)]
        DataTable GetDataTable(int stationNumber, DateTime nowDate, bool rainOnly);

        /// <summary>Get a rainfall forecast for the specified station number.</summary>
        /// <param name="stationNumber">The SILO station number.</param>
        /// <returns>A datatable with date and rain as columns.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/Get?stationNumber={stationNumber}&date={nowDate}&rainOnly={rainOnly}", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream Get(int stationNumber, DateTime nowDate, bool rainOnly);
    }
}
