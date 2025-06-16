using CommonLib.Interfaces;
using Serilog;
using System;

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

        public void WriteToFile(Exception ex)
        {
            string template = "\r\n-----Message-----\r\n{0}\r\n-----Source ---\r\n{1}\r\n-----StackTrace ---\r\n{2}\r\n-----TargetSite ---\r\n{3}";
            _logger.Error(template, ex.Message, ex.Source, ex.StackTrace, ex.TargetSite);
        }
    }
}
