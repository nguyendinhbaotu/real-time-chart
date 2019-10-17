using System;
using System.Collections.Generic;
using System.Linq;

namespace RealTimeChart.Services
{
    public class DataSourceService : IDataSourceService
    {
        Dictionary<string, ChartModel> _dict = new Dictionary<string, ChartModel>();

        public DataSourceService()
        {
            for (int key = 0; key < Constants.NUMBER_OF_CHARTS; key++)
            {
                _dict[key.ToString()] = new ChartModel();
                var chart = _dict[key.ToString()];
                for (int seriesIdx = 0; seriesIdx < Constants.NUMBER_OF_SERIES; seriesIdx++)
                {
                    chart.Series[seriesIdx] = new ChartEnty();
                    for (int i = Constants.NUMBER_OF_RECORDS; i >= 0; i--)
                    {
                        chart.Series[seriesIdx].Records.Add(GenerateNewRecord(i));
                    }
                }
                
            }
        }

        private ChartRecord GenerateNewRecord(int i = 0)
        {
            var record = new ChartRecord();
            record.Date = DateTime.UtcNow.AddSeconds(-i);
            record.Value = (new Random()).NextDouble();
            return record;
        }

        public List<decimal> GenerateNewRecord(string key)
        {
            for (int seriesIdx = 0; seriesIdx < Constants.NUMBER_OF_SERIES; seriesIdx++)
            {
                var newRecord = GenerateNewRecord();
                _dict[key.ToString()].Series[seriesIdx].Records.Add(newRecord);
                
            }
            return null;
        }

        public ChartModel GetData(string key = null)
        {
            //return _dict[key];
            return ScaleOutData(_dict[key]);
        }

        public List<List<decimal>> GetData(decimal startTime, decimal endTime, string key = null)
        {
            var data = _dict[key];
            var firstItem = data.First();
            var startIndex = (int)(startTime - firstItem[0]) / 1000;
            var count = (int)(endTime - startTime) / 1000;
            var maxRange = data.Count() - startIndex - 1;

            if (count > maxRange)
            {
                count = maxRange;
            }

            if (count < 0)
            {
                count = 0;
            }
            //return data.GetRange(startIndex, count);
            return ScaleOutData(data.GetRange(startIndex, count));
        }

        private List<List<decimal>> ScaleOutData(List<List<decimal>> data)
        {
            for (var i = 0; i < data.Count(); i += 1)
            {
                var date = new DateTime((long)data[i][0]);
            }


            var scaledData = new List<List<decimal>>();
            var scaleRatio = data.Count() / 100 + 1;
            for (var i = 0; i < data.Count(); i += scaleRatio)
            {
                scaledData.Add(data[i]);
            }
            return scaledData;
        }
    }
}
