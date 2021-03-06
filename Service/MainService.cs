﻿using ConsoleMThreads.WorkThread;
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
        private ThreadManager manager;

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
            manager = new ThreadManager();
            AppDomain.CurrentDomain.UnhandledException
             += new UnhandledExceptionEventHandler(ErrorHandler.HandleException);
        }

        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);

            Console.ReadLine();
           
        }


        protected override void OnStart(string[] args)
        {
            Log.i("OnStart");

          
            // TODO: test
            ZhiRenHandler.init();
            //string time = String.Format("{0}-{1}-{2} {3}:{4}", 2017, 3, 22, 11, 06);
            //ZhiRenHandler.sendUserVerified("2",time);
            //ZhiRenHandler.GetSubCompany();

            manager.Start();
            Log.i("OnStart End");
        }


        protected override void OnStop()
        {
            manager.Stop();
        }
    }
}
