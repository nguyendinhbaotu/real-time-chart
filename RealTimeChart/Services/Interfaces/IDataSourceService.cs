using System.Collections.Generic;

namespace RealTimeChart.Services
{
    public interface IDataSourceService
    {
        List<decimal> GenerateNewRecord(string key);
        List<List<decimal>> GetData(string key = null);
        List<List<decimal>> GetData(decimal startTime, decimal endTime, string key = null);
    }
}
