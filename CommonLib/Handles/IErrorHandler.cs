using Serilog;
using System.Data;

namespace CommonLib.Handles
{
    public interface IErrorHandler
    {
        ILogger Logger { get; }

        void WriteStringToFile(string spName);
        void WriteStringToFile(string spName, IDbDataParameter?[] parameters);
        void WriteToFile(Exception ex);
        void WriteStringToFuncion(string funcName, string ControllerName);
        void    WriteStringToFile(string funcName, object data);
    }
}