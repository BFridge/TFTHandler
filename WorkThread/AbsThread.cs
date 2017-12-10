using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zkemkeeper;


namespace ConsoleMThreads.WorkThread
{
    abstract class  AbsThread
    {
        protected string sIP = "0.0.0.0";
        protected int iPort = 4370;
        protected int iPass;

        protected int mMachineNumber = 1;
        protected int iThreadID = 0;

        protected static int iCounter = 0;
        protected static int iConnectedCount = 0;

        public zkemkeeper.CZKEMClass sdk = new CZKEMClass();//create Standalone SDK class dynamicly

        public AbsThread(ConnectInfo info)
        {
            sIP = info.Ip;
            iPort = info.Port;
            iPass = info.PassWord;
            iThreadID = ++iCounter;
        }

        protected abstract void Working();

        public void WakeUp(Object obj)
        {
            lock (sdk)
            {
                sdk.SetCommPassword(iPass);
                Log.i("*********Wake up and Start Connecting " + sIP);

                bool bResult = sdk.Connect_Net(sIP, iPort);

                if (!bResult)//Connecting device failed.
                {
                    Log.e("*********Connecting " + sIP + " Failed......Current Time:" + DateTime.Now.ToLongTimeString());
                    return;
                }
                iConnectedCount++;//count of connected devices

                Log.i("*********Successfully Connect IP:" + sIP + " " + "ThreadID:" + iThreadID.ToString() + " ConnectedCount:" + iConnectedCount.ToString() + " ConnectedTime:" + DateTime.Now.ToLongTimeString());


                sdk.EnableDevice(mMachineNumber, false);//disable the device
                Working();

                sdk.EnableDevice(mMachineNumber, true);//enable the device
                sdk.Disconnect();
                iConnectedCount--;
                Log.i("*********Successfully DisConnect " + sIP);
            }
        }


    }
}
