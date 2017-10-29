using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleMThreads.WorkThread
{
    class ThreadManager
    {
        private LinkedList<ReadLogThread> threads;
        int iLastTry = 0;//the former trying time(in mimutes) 
        int iDelay = 2;//to control the times connecting device 

        //Service Start must return in 30 sec, so we do this in thread
        public void Start()
        {

            // TODO: Add code here to start your service.
            bool bSetMaxThread = ThreadPool.SetMaxThreads(200, 500);
            if (!bSetMaxThread)
            {
                Log.e("Setting max threads of the threadpool failed!");
            }
            ThreadPool.QueueUserWorkItem(StartThreads);//Put the method into the queue to implement.
        }

        public void Stop()
        {
            ThreadPool.QueueUserWorkItem(StopThreads);
        }

        private void StartThreads(Object obj)
        {

            iLastTry = 0;
            iDelay = 2;
            threads = new LinkedList<ReadLogThread>();
            var infos = AppUtils.GetConnectInfos();
            for (var i = 0; i < infos.Length; i++)
            {
                ReadLogThread thread = new ReadLogThread(infos[i]);
                threads.AddLast(thread);
            }

            int iCurrentTime = 0;
            iLastTry = GetTimeInMinute();
            while (true)
            {

                //iCurrentTime = GetTimeInMinute();
                ////sleep until the minute ticks
                //while (iLastTry == iCurrentTime)
                //{
                //    Thread.Sleep(30);
                //    iCurrentTime = GetTimeInMinute();
                //}
                //iLastTry = iCurrentTime;

                //iDelay--;
                //if (iDelay == 1)
                //{
                //this.WakeUp();
                foreach (var t in threads)
                {
                    t.WakeUp(0);
                    Thread.Sleep(10000);
                }
                if (Log.isDebugging())
                {
                    Thread.Sleep(10000);
                }
                else {
                    //每小时更新一次
                    Thread.Sleep(3600000);
                }
                //}

            }
        }


        private void StopThreads(Object obj)
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            foreach (ReadLogThread thread in threads)
            {
                thread.DisConnect(0);//Put the method into the queue to implement.
            }
            Log.i("OnStop");
        }

        private int GetTimeInMinute()//return the time in mimutes
        {
            return ((DateTime.Now.Hour * 24) + DateTime.Now.Minute);
        }


    }
}
