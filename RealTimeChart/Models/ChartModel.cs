using System;
using System.Collections.Generic;
using System.Linq;

namespace RealTimeChart
{
    public class ChartModel
    {
        public List<ChartEnty> Series;

        public ChartModel()
        {
            Series = new List<ChartEnty>();
        }

        public decimal[][][] ToArray()
        {
            return Series.Select(s => s.Records.Select(r => new decimal[] { new DateTimeOffset(r.Date).ToUnixTimeMilliseconds(), (decimal) r.Value }).ToArray()).ToArray();
        }
    }


    public class ChartEnty
    {
        public List<ChartRecord> Records;

        public ChartEnty()
        {
            Records = new List<ChartRecord>();
        }
    }

    public class ChartRecord
    {
        public DateTime Date;
        public double Value;
    }
}
