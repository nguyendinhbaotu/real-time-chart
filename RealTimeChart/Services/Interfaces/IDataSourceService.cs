using System;
using System.Collections.Generic;

namespace RealTimeChart.Services
{
    public interface IDataSourceService
    {
        List<decimal> GenerateNewRecord(string key);
        decimal[][][] GetData(string key = null);
        decimal[][][] GetData(DateTime? startTime, DateTime? endTime, string key = null);
    }
}
