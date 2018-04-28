using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Framework
{
    public class QuickFrameworkRunner
    {
        private static QuickFrameworkRunner runner = new QuickFrameworkRunner();
        private QuickFrameworkRunner() { }
        public static QuickFrameworkRunner Instance
        {
            get
            {
                return runner;
            }
        }

        public void MultiCount()
        {
            MultiThreadTest.CountTest(30000);
        }

        public void HeartBeat()
        {
            MultiThreadTest.HeartBeat().Wait();
        }
    }
}
