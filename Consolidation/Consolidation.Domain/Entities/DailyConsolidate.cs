using System;
using System.Collections.Generic;
using System.Text;

namespace Consolidation.Domain.Entities
{
    public class DailyConsolidate
    {
        public DateOnly Date { get; set; }
        public decimal AccumulatedBalance { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
