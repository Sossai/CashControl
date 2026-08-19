using System;
using System.Collections.Generic;
using System.Text;

namespace Transactions.Infrastructure.Enums
{
    public enum OutboxStatus
    {
        Pending = 0,
        Published = 1
    }
}
