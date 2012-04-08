using System;
using System.Diagnostics;
using Caliburn.Micro;

namespace TrainShareApp.Logger
{
    public class DebugLogger :ILog 
    {
        public void Info(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
        }

        public void Warn(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
        }

        public void Error(Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }
}