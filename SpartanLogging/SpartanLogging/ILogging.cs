using System;
using System.Threading.Tasks;

namespace SpartanLogging
{
    public interface ILogging
    {
        Task Exception(string name, Exception exception, bool save=false);
        Task Error(string name, string message, bool save=false);
        Task Info(string name, string message, bool save=false);
        Task Warn(string name, string message, bool save=false);
    }
}
