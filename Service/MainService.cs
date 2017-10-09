using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleMThreads.Service
{
    partial class MainService : ServiceBase
    {

        public static string SERVICE_NAME = "tft_lisener";
        public static string SOURCE = SERVICE_NAME + "source";
        public static string LOG_NAME = SERVICE_NAME + "_log";
        private LinkedList<WorkThread> threads;

        public MainService()
        {
            InitializeComponent();
            if (!System.Diagnostics.EventLog.SourceExists(SOURCE))
            {
                System.Diagnostics.EventLog.CreateEventSource(SOURCE, LOG_NAME);
            }
            evt.Source = SOURCE;
            evt.Log = LOG_NAME;
            Log.setEventLogger(evt);
        }

        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            while (true) { 

            }
            this.OnStop();
        }


        protected override void OnStart(string[] args)
        {
            Log.d("OnStart");

            // TODO: Add code here to start your service.
            bool bSetMaxThread = ThreadPool.SetMaxThreads(200, 500);
            if (!bSetMaxThread)
            {
                Console.WriteLine("Setting max threads of the threadpool failed!");
            }
            // TODO: test
            ZhiRenHandler.init();
            //string time = String.Format("{0}-{1}-{2} {3}:{4}", 2017, 3, 22, 11, 06);
            //ZhiRenHandler.sendUserVerified("2",time);
            //ZhiRenHandler.GetSubCompany();
            threads = new LinkedList<WorkThread>();
            var infos = AppUtils.GetConnectInfos();
            for (var i = 0; i < infos.Length; i++)
            {
                WorkThread thread = new WorkThread(infos[i]);
                threads.AddLast(thread);
                //ThreadPool.QueueUserWorkItem(thread.Start);//Put the method into the queue to implement.
                thread.Start(null);
            }


        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            foreach (WorkThread thread in threads) {
                ThreadPool.QueueUserWorkItem(thread.DisConnect);//Put the method into the queue to implement.
            }
            Log.d("OnStop");

        }
    }
}
