using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Infrastructure.Payments.Mpesa
{
    public class MpesaException : Exception
    {
        public MpesaException(string message) : base(message) { }
    }
}
