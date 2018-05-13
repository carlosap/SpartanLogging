using System;
using System.IO;
using System.Threading.Tasks;
using SpartanExtensions.Strings;
using SpartanSettings;
namespace SpartanLogging
{
    public class Logging : ILogging
    {
        private static ISetting _setting;
        private string _path;

        /// <summary>
        /// Generates the UUID programmatically
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The Name of the Logging Message
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Type of Logging Message
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// A string representation of the application domain in which the exception occurred.
        /// </summary>
        public string AppDomain { get; set; }

        /// <summary>
        /// A string representation of the assembly name that contains the method in which the exception occurred.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// The name of the class in which the exception occurred.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// The Exception string format
        /// </summary>
        public string ExceptionString { get; set; }

        /// <summary>
        /// The Exception InnerException property
        /// </summary>
        public string InnerException { get; set; }

        /// <summary>
        /// The name of the machine on which the exception occurred. 
        /// If you use multiple servers for service hosting, 
        /// this property shows which machine experienced the exception.
        /// </summary>
        public string Machine { get; set; }

        /// <summary>
        /// The name of the method that threw the exception
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// The process ID under which the service was running 
        /// when the exception occurred.
        /// </summary>
        public string ProcessId { get; set; }

        /// <summary>
        /// The process name, related to the process ID, under which 
        /// the service was running when the exception occurred.
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// The reason for the error.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// The thread ID of the thread that threw the exception.
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// The Line Number of the Exception
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// The DateTime of the Exception
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Creates Logging folders to save
        /// Exceptions, Error, Info and Warning Messages
        /// </summary>
        public Logging()
        {
            _setting = new Setting();
            _path = _setting.CreateFolderSetting("Logs");

        }


        public Task Exception(string name, Exception exception, bool save=false) => Task.Run(async () =>
        {
            await Task.Run(async () =>
            {
                if (save)
                {                   
                    SaveLog(name, exception.ToString(), LogType.Exception);
                }
                WriteException($"Exception: {name} -> {exception.ToString()}");
            });
        });

        public Task Error(string name, string message, bool save=false) => Task.Run(async () =>
        {
            await Task.Run(async () =>
            {
                if (save)
                {
                    SaveLog(name, message, LogType.Error);
                }
                WriteError($"Error: {name} -> {message}");
            });
        });

        public Task Info(string name, string message, bool save=false) => Task.Run(async () =>
        {
            await Task.Run(async () =>
            {
                if (save)
                {
                    SaveLog(name, message, LogType.Info);
                }
                WriteMsg($"Info: {name} -> {message}");
            });
        });

        public Task Warn(string name, string message, bool save=false) => Task.Run(async () =>
        {
            await Task.Run(async () =>
            {
                if (save)
                {
                    SaveLog(name, message, LogType.Warning);
                }
                WriteWarn($"Warning: {name} -> {message}");

            });
        });

        private static Task<string> AddDatemeStringValues(string strName) => Task.Run(() => { return $"{strName}-{DateTime.Now:yyyy-MM-dd_hh-mm-ss-tt}"; });

        private async Task<string> GetFilePath(string traceName, LogType traceType)
        {
            return await Task.Run(async () =>
            {
                var traceExtension = await GetFileExtension(traceType);
                var directoryPath = Path.Combine(_path, traceType.ToString());
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                traceName = traceName.ReplaceInvalidCharsForUnderscore();
                traceName = await AddDatemeStringValues(traceName);
                traceName = $"{traceName}{traceExtension}";
                var filePath = Path.Combine(directoryPath, traceName);
                return filePath;
            });
        }

        private  Task<string> GetFileExtension(LogType traceType)
        {
            return Task.Run(() =>
            {
                string traceExtension;
                switch (traceType)
                {
                    case LogType.Exception:
                        traceExtension = ".ex";
                        break;
                    case LogType.Error:
                        traceExtension = ".err";
                        break;
                    case LogType.Info:
                        traceExtension = ".info";
                        break;
                    case LogType.Warning:
                        traceExtension = ".warn";
                        break;
                    default:
                        traceExtension = ".unknown";
                        break;
                }
                return traceExtension;
            });
        }

        static void WriteError(string value)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + value.PadRight(Console.WindowWidth - 1));
            Console.ResetColor();
        }

        private void WriteWarn(string value)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Warn: " + value.PadRight(Console.WindowWidth - 1));
            Console.ResetColor();
        }

        private void WriteMsg(string value)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Info: " + value.PadRight(Console.WindowWidth - 1));
            Console.ResetColor();
        }

        private void WriteException(string value)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("*Exception: " + value.PadRight(Console.WindowWidth - 1));
            Console.ResetColor();
        }

        private async Task SaveLog(string name, string message, LogType logtype)
        {
            var content = $"{logtype.ToString()}:{name}{Environment.NewLine}{DateTime.Now.ToString()}{Environment.NewLine}{message}";
            var filePath = await GetFilePath(name, logtype);
            File.WriteAllText(filePath, content);
        }
    }
}
