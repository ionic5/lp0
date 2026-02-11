using System.Runtime.CompilerServices;

namespace LikeLion.LH1.Client.Core
{
    public interface ILogger
    {
        void Info(string msg, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0);
        void Warn(string msg, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0);
        void Fatal(string msg, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0);
    }
}
