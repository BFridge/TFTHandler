using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMThreads.WorkThread
{
    /// <summary>
    /// 每次ReadLogThread都会读取数据库，为了防止数据库过大，降低读写速度，给同步带来困扰，过一段时间就会清空数据。
    /// </summary>
    class ClearLog : AbsThread
    {

        /// <summary>
        /// 如果上次运行同步过程中有错误，那么就不同步
        /// </summary>
        static Boolean lastTimeHasError;
        public static void reportError() {
            lastTimeHasError = true;
        }

        public static void resetError()
        {
            lastTimeHasError = false;
        }

        public ClearLog(ConnectInfo info) : base(info)
        {
        }

        override
        protected void Working() {
            if (lastTimeHasError) {
                Log.e("last time sync error, wait for next time sync without error");
                return;
            }

            int idwErrorCode = 0;
            if (sdk.ClearGLog(mMachineNumber))
            {
                Log.i("CLEAR ALL LOGS success");
                sdk.RefreshData(mMachineNumber);//the data in the device should be refreshed
            }
            else
            {
                sdk.GetLastError(ref idwErrorCode);
                Log.e("CLEAR LOGS ERROR "  + idwErrorCode);
            }        }

    }
}
