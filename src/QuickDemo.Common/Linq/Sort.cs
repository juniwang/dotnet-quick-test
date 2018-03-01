using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Common.Linq
{
    public class SE
    {
        public int ACount { get; set; }
        public int ICount { get; set; }
    }

    public static class SEE
    {
        public static int Max(this SE se)
        {
            return se.ACount * 10;
        }

        public static double Percent(this SE se)
        {
            double max = se.Max();
            if (max == 0)
                return double.MaxValue;

            return se.ICount / max;
        }
    }

    public class Sort
    {
        public static void TrySort()
        {
            Random r = new Random();
            List<SE> ses = new List<SE>();
            for (int i = 0; i < 20; i++)
            {
                ses.Add(new SE
                {
                    ACount = r.Next(0, 4),
                    ICount = r.Next(0, 20),
                });
            }
            Print("Before Sort", ses);
            Print("After Sort", ses.OrderBy(p => p.Percent()).ToList());
        }

        private static void Print(string message, IEnumerable<SE> ses)
        {
            Console.WriteLine($"==============={message}===============:");
            foreach (var se in ses)
            {
                Console.WriteLine($"Acount: {se.ACount}, Icount: {se.ICount}, Max: {se.Max()} ,Percent: {se.Percent()}");
            }
        }
    }
}
