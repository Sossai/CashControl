using Shared.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consolidation.Application.Responses
{
    public class ConsolidationResponse : Response
    {
        public DateOnly Date{ get; set; }
        public decimal Amount { get; set; }
        public DateTime UpdatedAt{ get; set; }

    }
}
