using System;
using zkemkeeper;

namespace ConsoleMThreads
{
    class WorkThread
    {
        ConnectInfo connectInfo;
        private bool bIsConnected = false;

        int iMachineNumber = 1; //In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.

        int iThreadID = 0;
        private static int iCounter = 0;
        public zkemkeeper.CZKEMClass axCZKEM1 = new CZKEMClass(); //create Standalone SDK class dynamicly
        private static Object myObject = new Object(); //create a new Object for the database operation

        //work thread
        public WorkThread(ConnectInfo info)
        {
            this.connectInfo = info;
            iThreadID = ++iCounter;
        }

        //call back function of threadpool
        public void Start(Object oThreadContext)
        {

            axCZKEM1.SetCommPassword(connectInfo.PassWord);
            int retryCount = 0;
            while (retryCount < 2 && bIsConnected == false) {
                retryCount++;
                ConsoleWriteLine("Start connect...");


                bIsConnected = axCZKEM1.Connect_Net(connectInfo.Ip, connectInfo.Port);
                if (bIsConnected)
                {
                    ConsoleWriteLine("Connect success!");
                    if (axCZKEM1.RegEvent(iMachineNumber, 65535)) //Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                    {
                        this.axCZKEM1.OnFinger += new zkemkeeper._IZKEMEvents_OnFingerEventHandler(axCZKEM1_OnFinger);
                        this.axCZKEM1.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
                        this.axCZKEM1.OnAttTransactionEx +=
                            new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx);
                        this.axCZKEM1.OnFingerFeature +=
                            new zkemkeeper._IZKEMEvents_OnFingerFeatureEventHandler(axCZKEM1_OnFingerFeature);
                        this.axCZKEM1.OnEnrollFingerEx +=
                            new zkemkeeper._IZKEMEvents_OnEnrollFingerExEventHandler(axCZKEM1_OnEnrollFingerEx);
                        this.axCZKEM1.OnDeleteTemplate +=
                            new zkemkeeper._IZKEMEvents_OnDeleteTemplateEventHandler(axCZKEM1_OnDeleteTemplate);
                        this.axCZKEM1.OnNewUser += new zkemkeeper._IZKEMEvents_OnNewUserEventHandler(axCZKEM1_OnNewUser);
                        this.axCZKEM1.OnHIDNum += new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(axCZKEM1_OnHIDNum);
                        this.axCZKEM1.OnAlarm += new zkemkeeper._IZKEMEvents_OnAlarmEventHandler(axCZKEM1_OnAlarm);
                        this.axCZKEM1.OnDoor += new zkemkeeper._IZKEMEvents_OnDoorEventHandler(axCZKEM1_OnDoor);
                        this.axCZKEM1.OnWriteCard +=
                            new zkemkeeper._IZKEMEvents_OnWriteCardEventHandler(axCZKEM1_OnWriteCard);
                        this.axCZKEM1.OnEmptyCard +=
                            new zkemkeeper._IZKEMEvents_OnEmptyCardEventHandler(axCZKEM1_OnEmptyCard);
                        ConsoleWriteLine("Register success!");

                      
                    }
                    else {

                        ConsoleError();
                    }
                }
                else

                {
                        ConsoleError();

                }

            }
        }


        public void DisConnect(Object oThreadContext)
        {
            axCZKEM1.Disconnect();
            this.axCZKEM1.OnFinger -= new zkemkeeper._IZKEMEvents_OnFingerEventHandler(axCZKEM1_OnFinger);
            this.axCZKEM1.OnVerify -= new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
            this.axCZKEM1.OnAttTransactionEx -=
                new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx);
            this.axCZKEM1.OnFingerFeature -=
                new zkemkeeper._IZKEMEvents_OnFingerFeatureEventHandler(axCZKEM1_OnFingerFeature);
            this.axCZKEM1.OnEnrollFingerEx -=
                new zkemkeeper._IZKEMEvents_OnEnrollFingerExEventHandler(axCZKEM1_OnEnrollFingerEx);
            this.axCZKEM1.OnDeleteTemplate -=
                new zkemkeeper._IZKEMEvents_OnDeleteTemplateEventHandler(axCZKEM1_OnDeleteTemplate);
            this.axCZKEM1.OnNewUser -= new zkemkeeper._IZKEMEvents_OnNewUserEventHandler(axCZKEM1_OnNewUser);
            this.axCZKEM1.OnHIDNum -= new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(axCZKEM1_OnHIDNum);
            this.axCZKEM1.OnAlarm -= new zkemkeeper._IZKEMEvents_OnAlarmEventHandler(axCZKEM1_OnAlarm);
            this.axCZKEM1.OnDoor -= new zkemkeeper._IZKEMEvents_OnDoorEventHandler(axCZKEM1_OnDoor);
            this.axCZKEM1.OnWriteCard -= new zkemkeeper._IZKEMEvents_OnWriteCardEventHandler(axCZKEM1_OnWriteCard);
            this.axCZKEM1.OnEmptyCard -= new zkemkeeper._IZKEMEvents_OnEmptyCardEventHandler(axCZKEM1_OnEmptyCard);
            ConsoleWriteLine("UnRegister success!");

        }

        /*************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first.*
        * This part is for demonstrating the RealTime Events that triggered  by your operations          *
        **************************************************************************************************/

        #region RealTime Events

        //When you place your finger on sensor of the device,this event will be triggered
        private void axCZKEM1_OnFinger()
        {
            ConsoleWriteLine("RTEvent OnFinger Has been Triggered");
        }

//After you have placed your finger on the sensor(or swipe your card to the device),this event will be triggered.
//If you passes the verification,the returned value userid will be the user enrollnumber,or else the value will be -1;
        private void axCZKEM1_OnVerify(int iUserID)
        {
            ConsoleWriteLine("RTEvent OnVerify Has been Triggered,Verifying...");
            if (iUserID != -1)
            {
                ConsoleWriteLine("Verified OK,the UserID is " + iUserID.ToString());
            }

            else
            {
                ConsoleWriteLine("Verified Failed... ");
            }
        }

//If your fingerprint(or your card) passes the verification,this event will be triggered
        private void axCZKEM1_OnAttTransactionEx(string sEnrollNumber, int iIsInValid, int iAttState, int iVerifyMethod,
            int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond, int iWorkCode)
        {
            ConsoleWriteLine("RTEvent OnAttTrasactionEx Has been Triggered,Verified OK");
            ConsoleWriteLine("...UserID:" + sEnrollNumber);
            ConsoleWriteLine("...isInvalid:" + iIsInValid.ToString());
            ConsoleWriteLine("...attState:" + iAttState.ToString());
            ConsoleWriteLine("...VerifyMethod:" + iVerifyMethod.ToString());
            ConsoleWriteLine("...Workcode:" +
                             iWorkCode
                                 .ToString()); //the difference between the event OnAttTransaction and OnAttTransactionEx
            ConsoleWriteLine("...Time:" + iYear.ToString() + "-" + iMonth.ToString() + "-" + iDay.ToString() + " " +
                             iHour.ToString() + ":" + iMinute.ToString() + ":" + iSecond.ToString());
            string time = String.Format("{0}-{1}-{2} {3}:{4}", iYear, iMonth, iDay, iHour, iMinute);
            ZhiRenHandler.sendUserVerified(sEnrollNumber,time);
        }

//When you have enrolled your finger,this event will be triggered and return the quality of the fingerprint you have enrolled
        private void axCZKEM1_OnFingerFeature(int iScore)
        {
            if (iScore < 0)
            {
                ConsoleWriteLine("The quality of your fingerprint is poor");
            }
            else
            {
                ConsoleWriteLine("RTEvent OnFingerFeature Has been Triggered...Score:　" + iScore.ToString());
            }
        }

//When you are enrolling your finger,this event will be triggered.
        private void axCZKEM1_OnEnrollFingerEx(string sEnrollNumber, int iFingerIndex, int iActionResult,
            int iTemplateLength)
        {
            if (iActionResult == 0)
            {
                ConsoleWriteLine("RTEvent OnEnrollFigerEx Has been Triggered....");
                ConsoleWriteLine(".....UserID: " + sEnrollNumber + " Index: " + iFingerIndex.ToString() +
                                 " tmpLen: " + iTemplateLength.ToString());
            }
            else
            {
                ConsoleWriteLine("RTEvent OnEnrollFigerEx Has been Triggered Error,actionResult=" +
                                 iActionResult.ToString());
            }
        }

//When you have deleted one one fingerprint template,this event will be triggered.
        private void axCZKEM1_OnDeleteTemplate(int iEnrollNumber, int iFingerIndex)
        {
            ConsoleWriteLine("RTEvent OnDeleteTemplate Has been Triggered...");
            ConsoleWriteLine("...UserID=" + iEnrollNumber.ToString() + " FingerIndex=" + iFingerIndex.ToString());
        }

//When you have enrolled a new user,this event will be triggered.
        private void axCZKEM1_OnNewUser(int iEnrollNumber)
        {
            ConsoleWriteLine("RTEvent OnNewUser Has been Triggered...");
            ConsoleWriteLine("...NewUserID=" + iEnrollNumber.ToString());
        }

//When you swipe a card to the device, this event will be triggered to show you the card number.
        private void axCZKEM1_OnHIDNum(int iCardNumber)
        {
            ConsoleWriteLine("RTEvent OnHIDNum Has been Triggered...");
            ConsoleWriteLine("...Cardnumber=" + iCardNumber.ToString());
        }

//When the dismantling machine or duress alarm occurs, trigger this event.
        private void axCZKEM1_OnAlarm(int iAlarmType, int iEnrollNumber, int iVerified)
        {
            ConsoleWriteLine("RTEvnet OnAlarm Has been Triggered...");
            ConsoleWriteLine("...AlarmType=" + iAlarmType.ToString());
            ConsoleWriteLine("...EnrollNumber=" + iEnrollNumber.ToString());
            ConsoleWriteLine("...Verified=" + iVerified.ToString());
        }

//Door sensor event
        private void axCZKEM1_OnDoor(int iEventType)
        {
            ConsoleWriteLine("RTEvent Ondoor Has been Triggered...");
            ConsoleWriteLine("...EventType=" + iEventType.ToString());
        }

//When you have emptyed the Mifare card,this event will be triggered.
        private void axCZKEM1_OnEmptyCard(int iActionResult)
        {
            ConsoleWriteLine("RTEvent OnEmptyCard Has been Triggered...");
            if (iActionResult == 0)
            {
                ConsoleWriteLine("...Empty Mifare Card OK");
            }
            else
            {
                ConsoleWriteLine("...Empty Failed");
            }
        }

//When you have written into the Mifare card ,this event will be triggered.
        private void axCZKEM1_OnWriteCard(int iEnrollNumber, int iActionResult, int iLength)
        {
            ConsoleWriteLine("RTEvent OnWriteCard Has been Triggered...");
            if (iActionResult == 0)
            {
                ConsoleWriteLine("...Write Mifare Card OK");
                ConsoleWriteLine("...EnrollNumber=" + iEnrollNumber.ToString());
                ConsoleWriteLine("...TmpLength=" + iLength.ToString());
            }
            else
            {
                ConsoleWriteLine("...Write Failed");
            }
        }

//After function GetRTLog() is called ,RealTime Events will be triggered. 
//When you are using these two functions, it will request data from the device forwardly.
        private void rtTimer_Tick(object sender, EventArgs e)
        {
            if (axCZKEM1.ReadRTLog(iMachineNumber))
            {
                while (axCZKEM1.GetRTLog(iMachineNumber))
                {
                    ;
                }
            }
        }

        #endregion

        private void ConsoleWriteLine(string line)
        {
            //Console.WriteLine("IP :" + connectInfo.Ip + " " + line);
            Log.d("IP :" + connectInfo.Ip + " " + line);

        }

        private void ConsoleError() {
            int idwErrorCode = 0;
            axCZKEM1.GetLastError(ref idwErrorCode);
            ConsoleWriteLine("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString());
        }
    }
}