using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkGraph
{
    public class OperationSchedule
    {
        public int OperationId { get; set; }  // Исходный ID операции
        public int LocalIndex { get; set; }    // Локальный индекс (1, 2, 3...)
        public double EarlyStart { get; set; } // Трн
        public double Duration { get; set; }   // Тдл
        public double LateFinish { get; set; } // Тпо
        public double ActualStart { get; set; } // Тнач (будет вычислено)
        public double ActualFinish { get; set; } // Токонч (будет вычислено)
    }
}
