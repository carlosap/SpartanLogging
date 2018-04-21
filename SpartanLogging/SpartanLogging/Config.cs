using System.IO;
namespace SpartanLogging
{
    public partial class Config
    {
        public static string AppPath { get; set; } = Directory.GetCurrentDirectory();
    }
}
