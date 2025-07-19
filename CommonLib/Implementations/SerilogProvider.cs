using CommonLib.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Implementations
{
    public class SerilogProvider : ISerilogProvider
    {
        public ILogger Logger { get; }

        public SerilogProvider()
        {
            Logger = Log.Logger;
        }
    }
}
