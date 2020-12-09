using Caliburn.Micro;
using System;
using System.Diagnostics;

namespace Treadmill.Ui.Droid
{
    public class CaliburnMicroLogger : ILog
    {
        public CaliburnMicroLogger(Type type)
        {
        }

        private string Message_(string format, params object[] args)
        {
            return $"[{DateTime.Now:o}] {string.Format(format, args)}";
        }

        #region ILog Members
        public void Error(Exception exception)
        {
            Debug.WriteLine(Message_(exception.ToString()), "ERROR");
        }

        public void Info(string format, params object[] args)
        {
            Debug.WriteLine(Message_(format, args), "INFO");
        }

        public void Warn(string format, params object[] args)
        {
            Debug.WriteLine(Message_(format, args), "WARN");
        }
        #endregion
    }
}
