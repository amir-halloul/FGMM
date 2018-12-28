namespace FGMM.SDK.Core.Diagnostics
{
    public interface ILogger
    {
        string Prefix { get; }

        void Debug(string message);

        void Info(string message);

        void Warning(string message);

        void Error(string message);

        void Log(string message, LogLevel level);
    }
}
