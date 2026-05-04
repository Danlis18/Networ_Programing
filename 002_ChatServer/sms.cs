using System;
using System.Collections.Generic;
using System.Text;

namespace _003_SetrverClient
{
    public class Sms
    {
        public string Nickname { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
