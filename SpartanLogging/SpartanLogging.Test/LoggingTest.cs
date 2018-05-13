using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpartanLogging.Test
{
    [TestClass]
    public class LoggingTest
    {
        [TestMethod]
        public void TestLogs()
        {
            //please verify your C:\Users\userprofile\.spartan\Logs\   folder
            var _logging = new Logging();
            _logging.Error("Test:ERROR", "some error just happened", true);
            _logging.Warn("Test:Warning", "some warnning message was save", true);
            _logging.Info("Test:Info", "Some important info was save", true);

        }
    }
}
