using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuickDemo.Framework
{
    class MultiThreadTest
    {
        private static long total = 0;
        private static int samples = 0;
        private static Random random = new Random();

        private static void Count(long sampleData)
        {
            Interlocked.Add(ref total, sampleData);
            Interlocked.Increment(ref samples);
        }

        private static void CountWithoutLock(long sampleData)
        {
            total = total + 1;
            samples++;
        }

        public static void CountTest(int number)
        {
            DateTime start = DateTime.UtcNow;
            Task[] tasks = new Task[number];
            for (int i = 0; i < number; i++)
            {
                string ticks = GenerateTicks(100);
                tasks[i] = Task.Run(() =>
                {
                    UpdateLatency(ticks);
                });
            }
            Task.WaitAll(tasks);
            DateTime end = DateTime.UtcNow;
            Console.WriteLine((end - start).ToString());
            Console.WriteLine("Total={0}, Samples={1}", total.ToString(), samples.ToString());
        }

        private static string GenerateTicks(int length)
        {
            var str = "C" + DateTime.UtcNow.AddMilliseconds(random.Next(-2500, -1500)).Ticks.ToString()
                + "|E" + DateTime.UtcNow.AddMilliseconds(random.Next(-1500, -500)).Ticks.ToString() + "|";
            if (str.Length < length)
                str.PadRight(length, 'a');
            return str;
        }

        public static void UpdateLatency(string ticks, long maxTimeoutMs = 60 * 1000)
        {
            try
            {
                long now = DateTime.UtcNow.Ticks;
                long clientTicks = 0;
                long serverTicks = 0;
                //Console.WriteLine("N" + now.ToString() + "|" + ticks);
                foreach (var tick in ticks.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    char start = tick[0];
                    switch (start)
                    {
                        case 'C': // the timestamp before sending
                            clientTicks = long.Parse(tick.Substring(1));
                            break;
                        case 'E': // the timestamp before SignalR server echo it back
                        case 'B': // the timestamp before SignalR server broadcast it.
                            serverTicks = long.Parse(tick.Substring(1));
                            break;
                        default:
                            break;
                    }
                }

                long requestLatency = now - clientTicks;
                if (requestLatency > 0 && clientTicks > 0)
                {
                    Count(requestLatency);
                }
            }
            catch { }
        }
    }
}
