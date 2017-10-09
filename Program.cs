/**********************************************************
 * Demo for Standalone SDK.Created by Darcy on Dec.15 2009*
***********************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Data.OleDb;
using System.ServiceProcess;
using ConsoleMThreads.Service;

namespace ConsoleMThreads
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                MainService service1 = new MainService();
                service1.TestStartupAndStop(args);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new MainService()
                };
                ServiceBase.Run(ServicesToRun);
            }
           
           
        }
    }

}