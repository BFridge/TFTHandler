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
        private LinkedList<TimeSync> timeSyncThreads;
        private LinkedList<ClearLog> clearLogThreads;

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
            threads = new LinkedList<ReadLogThread>();
            timeSyncThreads = new LinkedList<TimeSync>();
            clearLogThreads = new LinkedList<ClearLog>();

            //测试用例数据
            var infos = Log.isDebugging() ? AppUtils.GetConnectInfosTest() : AppUtils.GetConnectInfos();
            //var infos = AppUtils.GetConnectInfos();
            for (var i = 0; i < infos.Length; i++)
            {
                ReadLogThread thread = new ReadLogThread(infos[i]);
                threads.AddLast(thread);

                TimeSync sync = new TimeSync(infos[i]);
                timeSyncThreads.AddLast(sync);

                ClearLog clear = new ClearLog(infos[i]);
                clearLogThreads.AddLast(clear);
                
            }

            if (Log.isDebugging()) {
                while (true)
                {
                    ReadLogTask();
                    Thread.Sleep(10000);
                    ClearLogTask();
                    Thread.Sleep(3600000);

                }
            }
            else
            {
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Enabled = true;
                timer.Interval = 60000;//执行间隔时间,单位为毫秒 每分钟检查一次
                timer.Start();
                timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer1_Elapsed);

            }
          
        }

        private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            // 得到 hour minute second  如果等于某个值就开始执行某个程序。  
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intMinDate = e.SignalTime.Day;
            // 定制时间； 比如 在10：30 ：00 的时候执行某个函数  

            int readLogMinute = 40;

          

            int hourSyncTime = 1;
            int minSyncTime = 04;


            int dateClearLogTime = 11;
            int hourClearLogTime = 0;
            int minClearLogTime = 18;

            //if (dateClearLogTime == intMinDate && intHour == hourClearLogTime && intMinute == minClearLogTime) {
            //    Log.d("每月任务！");
            //    ReadLogTask();
            //    Thread.Sleep(10000);
            //    ClearLogTask();
            //}


            if (readLogMinute == intMinute)
            {
                Log.d("每小时任务！");
                ReadLogTask();
            }


            if (intHour == hourSyncTime && intMinute == minSyncTime)
            {
                Log.d("每日任务");
                SyncTimeTask();
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

        public void ReadLogTask() {
            ClearLog.resetError();
            foreach (var t in threads)
            {
                t.WakeUp(0);
                Thread.Sleep(10000);
            }
        }

        public void SyncTimeTask() {
            foreach (var t in timeSyncThreads)
            {
                t.WakeUp(0);
                Thread.Sleep(10000);
            }
        }

        public void ClearLogTask() {
            foreach (var t in clearLogThreads)
            {
                t.WakeUp(0);
                Thread.Sleep(10000);
            }
        }


    }
}
