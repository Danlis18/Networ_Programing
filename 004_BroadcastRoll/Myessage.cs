using System;
using System.Collections.Generic;
using System.Text;

namespace _003_BroadcastChat
{
    public class Myessage
    {
        public string ComputerName { get; set; } = SystemInformation.ComputerName;
        public bool IsOnline { get; set; }

        public string Message { get; set; }
        
         public override string ToString()
        {
            return ComputerName;
        }
    }
}
