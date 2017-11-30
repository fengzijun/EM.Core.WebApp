using System;
using System.Collections.Generic;

namespace EM
{
    /// <summary>
    /// Logger interface
    /// </summary>
    public partial interface ILogger
    {
        void WriteLog(string message);

        void WriteErrorLog(string message, Exception ex);

        void WriteErrorLog(Exception ex);

        void WriteWarn(string message);
    }
}