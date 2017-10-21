/**********************************************************
 * Demo for Standalone SDK.Created by Darcy on Dec.15 2009*
***********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using zkemkeeper;
namespace ConsoleMThreads.WorkThread
{
    class ReadLogThread
    {
        string sIP = "0.0.0.0";
        int iPort = 4370;
        int iPass;

        int iMachineNumber = 1;
        int iThreadID = 0;

        private static int iCounter = 0;
        private static int iConnectedCount = 0;

        public zkemkeeper.CZKEMClass sdk = new CZKEMClass();//create Standalone SDK class dynamicly
        private static Object myObject = new Object();//create a new Object for the database operation

        //work thread
        public ReadLogThread(ConnectInfo info)
        {
            sIP = info.Ip;
            iPort = info.Port;
            iPass = info.PassWord;
            iThreadID = ++iCounter;
        }


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
               
                int iLogCount = 0;
                int idwErrorCode = 0;

                sdk.EnableDevice(iMachineNumber, false);//disable the device
                if (sdk.ReadAllGLogData(iMachineNumber))
                {
                    string sdwEnrollNumber = "";
                    int idwVerifyMode = 0;
                    int idwInOutMode = 0;
                    int idwYear = 0;
                    int idwMonth = 0;
                    int idwDay = 0;
                    int idwHour = 0;
                    int idwMinute = 0;
                    int idwSecond = 0;
                    int idwWorkCode = 0;

                    //String connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=..\..\..\att.mdf";



                    AttDataClassesDataContext context = new AttDataClassesDataContext();


                    var table = context.GetTable<TFTAttLog>();
                    var a = (from TFTAttLog in table
                             select TFTAttLog).ToList();



                    //String connStringSql = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = E:\zkemsdk\zkemSource\脱机通讯开发包(64bit Ver6.2.4.11)\Demo\C#\IFace_x64\ConsoleMThreads\att.mdf; Integrated Security = True; Connect Timeout = 30";
                    //SqlConnection connSql = new SqlConnection(connStringSql);
                    //connSql.Open();


                    var newLogs = new LinkedList<TFTAttLog>();
                    while (sdk.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out idwVerifyMode, out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkCode))
                    {
                        iLogCount++;//increase the number of attendance records

                        lock (myObject)//make the object exclusive 
                        {


                            string sTime = idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString();
                                                                                                                                                                                                                                                                                  //OleDbConnection conn = new OleDbConnection(connString);
                            var log = new TFTAttLog();
                            log.Ip = sIP.ToString();
                            log.EnrollNumber = sdwEnrollNumber.ToString();
                            log.VerifyMode = idwVerifyMode.ToString();
                            log.InOutMode = idwInOutMode.ToString();
                            log.Time = sTime;
                            log.WorkCode = idwWorkCode.ToString();
                            try
                            {
                                if (!table.Contains(log))
                                {

                             
                                    Log.d("READLOG:" + log);
                                    newLogs.AddLast(log);
                                    SendToZHIREN_SaveInLOCAL(table, null, log);
                                }
                            }
                            catch (Exception e)
                            {
                                Log.e(e.Message);
                                if (Log.isDebugging())
                                {
                                    throw new Exception("ERROR");
                                }

                            }
                        }
                    }

                    //SendToZHIREN_SaveLOCAL(table, newLogs, null);

                }
                else
                {
                    sdk.GetLastError(ref idwErrorCode);
                    Log.e("ThreadID:" + iThreadID.ToString() + " General Log Data Count:0 ErrorCode=" + idwErrorCode.ToString());
                }

                sdk.EnableDevice(iMachineNumber, true);//enable the device
                sdk.Disconnect();
                iConnectedCount--;
                Log.i("*********Successfully DisConnect " + sIP);
            }
        }

        //newlogs和singlelogs参数二选一
        private void SendToZHIREN_SaveInLOCAL(System.Data.Linq.Table<TFTAttLog> table, LinkedList<TFTAttLog> newLogs, TFTAttLog singlelog)
        {

            //sqlCmd.ExecuteNonQuery();
            if (singlelog != null)
            {
                newLogs = new LinkedList<TFTAttLog>();
                newLogs.AddFirst(singlelog);
            }

            ZhiRenHandler.sendUserVerified(newLogs, response =>
            {
                if (response == null || response.Length == 0)
                {
                    return;
                }

                if (response[0].Code != null)
                {

                    //todo:调试模式下依然按照成功处理 等之人后端搞定之后去除isDebugging判断
                    if (!Log.isDebugging() && response[0].Code.Contains("error"))
                    {
                        Log.e("ZHIREN error" + response[0].ErrorMessage);
                        return;
                    }

                    lock (myObject)
                    {
                        foreach (var log in newLogs)
                        {
                            try
                            {
                                //success
                                table.InsertOnSubmit(log);
                                Log.d("SAVE:" + table.Contains(log));
                                table.Context.SubmitChanges();
                                Log.d("SAVED:" + log);
                            }
                            catch (Exception e)
                            {
                                Log.e(e.Message);
                                if (Log.isDebugging())
                                {
                                    throw new Exception("ERROR");
                                }

                            }
                        }
                    }
                }
            });
        }

        private void btnClearGLog_Click(object sender, EventArgs e)
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



                int idwErrorCode = 0;

                sdk.EnableDevice(iMachineNumber, false);//disable the device
                if (sdk.ClearGLog(iMachineNumber))
                {

                    Log.e("CLEAR ALL LOGS");
                    sdk.RefreshData(iMachineNumber);//the data in the device should be refreshed
                }
                else
                {
                    sdk.GetLastError(ref idwErrorCode);
                }
                sdk.EnableDevice(iMachineNumber, true);//enable the device
            }
        }

        public void DisConnect(Object oThreadContext)
        {
            //do nothing
                
        }


    }

}
