using System;
using Microsoft.WindowsMobile.PocketOutlook;
using System.Threading;


namespace TrainStationAdvisor
{
    public class SMSSender
    {
        public event EventHandler<SMSEventArgs> SMSCompleted;

        public class SMSEventArgs : EventArgs
        {
            public string Error { get; private set; }
            public string Type { get; private set; }

            public SMSEventArgs(string AError, string AType)
            {
                Error = AError;
                Type = AType;
            }
        }

        protected virtual void OnCompleted(SMSEventArgs e)
        {
            if (null != SMSCompleted)
            {
                SMSCompleted(this, e);
            }
        }

        public void SendMessage(string AMessage, string APhoneNr)
        {
            SmsMessage _Sms = new SmsMessage(APhoneNr, AMessage);
            ThreadPool.QueueUserWorkItem(ThreadedSendMessage, _Sms);
        }

        private void ThreadedSendMessage(object AParam)
        {
            string _Error = string.Empty;
            string _Type = string.Empty;

            try
            {
                SmsMessage _Sms = (SmsMessage)AParam;
                _Sms.Send();
            }
            catch (SmsException ex)
            {
                _Error = ex.Message;
                _Type = "SMS Error";
            }
            catch (Exception ex)
            {
                _Error = ex.Message;
                _Type = "General Error";
            }

            SMSEventArgs _args = new SMSEventArgs(_Error, _Type);
            OnCompleted(_args);
        }
    }
}
