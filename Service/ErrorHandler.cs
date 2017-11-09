using System;

namespace ConsoleMThreads.Service
{
    public class ErrorHandler
    {
        public static void HandleException(object sender, UnhandledExceptionEventArgs e) {
            Log.e("FATAL ERROR:" + e.ExceptionObject );
        }
    }
}