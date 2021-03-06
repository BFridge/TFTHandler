﻿using System;
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
            if (isDebugging()) {
                //Console.WriteLine(logMessage);
            }
            Debug.WriteLine(logMessage);
        }

        public static void i(string logMessage) {
            Debug.WriteLine(logMessage);

            if (evt != null)
            {
                evt.WriteEntry(logMessage);
            }
        }

        public static void e(string logMessage) {
            Debug.WriteLine("【FALTAL】:" + logMessage);
            ///
            WorkThread.ClearLog.reportError();
            if (evt != null)
            {
                evt.WriteEntry(logMessage, EventLogEntryType.Error,20);
            }
        }

        public static bool isDebugging()
        {
            bool debugging = false;

            WellAreWe(ref debugging);

            return debugging;
        }

        [Conditional("DEBUG")]
        private static void WellAreWe(ref bool debugging)
        {
            debugging = true;
        }
    }
}
