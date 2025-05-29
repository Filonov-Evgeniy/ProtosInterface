using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkGraph.models;

namespace NetworkGraph
{
    public class OperationTiming
    {
        public Operation Operation { get; set; }
        public OperationVariant Variant { get; set; }
        public double EarlyStart { get; set; }
        public double EarlyFinish { get; set; }
        public double LateStart { get; set; }
        public double LateFinish { get; set; }

        public bool IsCritical
        {
            get => Math.Abs(EarlyStart - LateStart) < 0.001 &&
                   Math.Abs(EarlyFinish - LateFinish) < 0.001;
        }
    }
}
