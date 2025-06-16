using Serilog;

namespace CommonLib.Interfaces
{
    public interface IErrorHandler
    {
        ILogger Logger { get; }

        void WriteToFile(Exception ex);
    }
}