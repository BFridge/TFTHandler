using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMThreads
{
    class Log
    {

        private static System.Diagnostics.EventLog evt;

        public  static void setEventLogger(EventLog eventlog) {
            evt = eventlog;
        }


        public static void d(string logMessage)
        {

            Debug.WriteLine(logMessage);
            Console.WriteLine(logMessage);
            if (evt != null) {
                evt.WriteEntry(logMessage);
            }
           
           
        }
    }
}
