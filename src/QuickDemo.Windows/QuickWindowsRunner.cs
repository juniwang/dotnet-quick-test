using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Windows
{
    public class QuickWindowsRunner
    {
        private static QuickWindowsRunner runner = new QuickWindowsRunner();
        private QuickWindowsRunner()
        {
        }

        public static QuickWindowsRunner Instance
        {
            get
            {
                return runner;
            }
        }

        public void LogEventTest()
        {
            QuickDemoEventSource.Current.Startup();
            Console.WriteLine("This is a message from QuickDemo. Starting up");

            QuickDemoEventSource.Current.DBQueryStart("Select * from MYTable");
            var url = "http://localhost";
            for (int i = 0; i < 10; i++)
            {
                QuickDemoEventSource.Current.PageStart(i, url);
                QuickDemoEventSource.Current.Mark(i);
                QuickDemoEventSource.Current.PageStop(i);
            }
            QuickDemoEventSource.Current.DBQueryStop();

            QuickDemoEventSource.Current.Failure("This is a failure 1");
            QuickDemoEventSource.Current.Failure("This is a failure 2");
            QuickDemoEventSource.Current.Failure("This is a failure 3");
        }
    }
}
