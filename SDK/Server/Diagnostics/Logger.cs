using System;
using FGMM.SDK.Core.Diagnostics;

namespace FGMM.SDK.Server.Diagnostics
{
    public class Logger : ILogger
    {
        public string Prefix { get; }

        public Logger(string prefix)
        {
            Prefix = prefix;
        }

        public void Debug(string message) => Log(message, LogLevel.DEBUG);
        public void Info(string message) => Log(message, LogLevel.INFO);
        public void Warning(string message) => Log(message, LogLevel.WARNING);
        public void Error(string message) => Log(message, LogLevel.ERROR);

        public void Log(string message, LogLevel level)
        {
            string output = $"[{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}][{level}]";
            if (!string.IsNullOrEmpty(Prefix))
                output += $"[{Prefix}]";

            CitizenFX.Core.Debug.Write($"{output} {message}{Environment.NewLine}");
        }
    }
}