using System;
using System.Collections.Generic;

namespace RealTimeChart
{
    public class ChartModel
    {
        public List<ChartEnty> Series;

        public ChartModel()
        {
            Series = new List<ChartEnty>();
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
