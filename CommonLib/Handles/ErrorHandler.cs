using CommonLib.Interfaces;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Data;
using System.Xml.Linq;

namespace CommonLib.Handles
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly ILogger _logger;

        public ILogger Logger => _logger;

        public ErrorHandler(ISerilogProvider serilogProvider)
        {
            _logger = serilogProvider.Logger;
        }
       public void WriteStringToFuncion(string funcName,string ControllerName)
        {
            Logger.Debug("Call funcion: {SpName} | method: {Params}", funcName, ControllerName);
        }
        public void WriteStringToFile(string spName, IDbDataParameter?[] parameters)
        {
            var joinedParams = string.Join(", ", parameters?.Select(p => $"{p.ParameterName}={p.Value}") ?? ["rỗng"]);
            Logger.Error("Call SP: {SpName} | Params: {Params}", spName, joinedParams);
        }
        public void WriteStringToFile(string spName)
        {

            Logger.Error("Call : {SpName} | Params: {Params}", spName);
        }
        public void WriteStringToFile(string funcName, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            Logger.Debug("Call Func: {FuncName} | Data: {JsonData}", funcName, json);
        }

        public void WriteToFile(Exception ex)
        {
            string template = "\r\n-----Message-----\r\n{0}\r\n-----Source ---\r\n{1}\r\n-----StackTrace ---\r\n{2}\r\n-----TargetSite ---\r\n{3}";
            _logger.Error(template, ex.Message, ex.Source, ex.StackTrace, ex.TargetSite);
        }
    }
}
