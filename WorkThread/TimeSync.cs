using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMThreads.WorkThread
{
    class TimeSync : AbsThread
    {
        public TimeSync(ConnectInfo info) : base(info) {
        }

      
        override protected void Working() {
            int idwErrorCode = 0;

         
            if (sdk.SetDeviceTime(mMachineNumber))
            {
                sdk.RefreshData(mMachineNumber);//the data in the device should be refreshed
                Log.i("Successfully set the time of the machine and the terminal to sync PC!");
                int idwYear = 0;
                int idwMonth = 0;
                int idwDay = 0;
                int idwHour = 0;
                int idwMinute = 0;
                int idwSecond = 0;
                if (sdk.GetDeviceTime(mMachineNumber, ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond))//show the time
                {
                    string sTime = idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString();

                    Log.i("Now time" + sTime);
                }
            }
            else
            {
                sdk.GetLastError(ref idwErrorCode);
                Log.e("Operation failed,ErrorCode=" + idwErrorCode.ToString());
            }
        }

    }
}
