using System;
using System.IO;
using System.Threading.Tasks;
using SpartanExtensions.Strings;
namespace SpartanLogging
{
    public class Logging : ILogging
    {
        private string _path;
        public Logging(string path)
        {
            _path = path;
        }

        public Logging()
        {
            _path = Directory.GetCurrentDirectory();
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
                var directoryPath = Path.Combine(_path, "Logs", traceType.ToString());
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
