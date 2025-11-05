using Serilog;

namespace CommonLib.Interfaces
{
    public interface ISerilogProvider
    {
        ILogger Logger { get; }
    }
}