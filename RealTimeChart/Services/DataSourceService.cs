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
                    chart.Series.Add(new ChartEnty());
                    for (int i = Constants.NUMBER_OF_RECORDS; i >= 0; i--)
                    {
                        double min = 0;
                        double max = 1;
                        if (seriesIdx == 0)
                        {
                            min = 0.6;
                            max = 1;
                        };

                        if (seriesIdx == 1)
                        {
                            min = 0.4;
                            max = 0.8;
                        };

                        if (seriesIdx == 2)
                        {
                            min = 0.2;
                            max = 0.6;
                        };

                        if (seriesIdx == 3)
                        {
                            min = 0;
                            max = 0.4;
                        };

                        chart.Series[seriesIdx].Records.Add(GenerateNewRecord(i, min, max));
                    }
                }

            }
        }

        private ChartRecord GenerateNewRecord(int i = 0, double minimum = 0, double maximum = 1)
        {
            var record = new ChartRecord();
            record.Date = DateTime.UtcNow.AddSeconds(-i);
            record.Value = GetRandomNumber(minimum, maximum);
            return record;
        }

        public double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
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

        public decimal[][][] GetData(string key = null)
        {
            return ScaleOutData(_dict[key]).ToArray();
        }

        public decimal[][][] GetData(DateTime? startTime = null, DateTime? endTime = null, string key = null)
        {
            return ScaleOutData(_dict[key], startTime, endTime).ToArray();
        }

        private ChartModel ScaleOutData(ChartModel data, DateTime? startTime = null, DateTime? endTime = null)
        {
            var result = new ChartModel();
            for (var i = 0; i < data.Series.Count; i += 1)
            {
                result.Series.Add(new ChartEnty());
                if (startTime != null && endTime != null && startTime.Value.Year == endTime.Value.Year && startTime.Value.Month == endTime.Value.Month && startTime.Value.Day == endTime.Value.Day)
                {
                    result.Series[i].Records = data.Series[i].Records;
                }
                else
                {
                    result.Series[i].Records = ScaleOutData(data.Series[i].Records, startTime, endTime);
                }
            }
            return result;
        }


        private List<ChartRecord> ScaleOutData(List<ChartRecord> records, DateTime? startTime = null, DateTime? endTime = null)
        {
            var results = records.Select(s => s).ToList();

            if (startTime != null)
                results = results.Where(r => r.Date < startTime.Value).ToList();

            if (endTime != null)
                results = results.Where(r => r.Date < endTime.Value).ToList();

            if (startTime != null && endTime != null && startTime.Value.Year == endTime.Value.Year && startTime.Value.Month == endTime.Value.Month && startTime.Value.Day == endTime.Value.Day)
            {
                results = records
                    .GroupBy(r => new DateTime(r.Date.Year, r.Date.Month, r.Date.Day, r.Date.Hour, r.Date.Minute, 0))
                    .Select(g => new ChartRecord { Date = g.Key, Value = g.Sum(s => s.Value) })
                    .ToList();
            }
            else
            {
                results = records
                    .GroupBy(r => new DateTime(r.Date.Year, r.Date.Month, r.Date.Day, r.Date.Hour, 0, 0))
                    .Select(g => new ChartRecord { Date = g.Key, Value = g.Sum(s => s.Value) })
                    .ToList();
            }


            return results;
        }
    }
}
