using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Responses
{
    public abstract class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ErrorCodes ErrorCode { get; set; }
    }
}
