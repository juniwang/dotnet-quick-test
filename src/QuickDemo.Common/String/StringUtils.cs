using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Common.String
{
    public class StringUtils
    {
        public static string Aggregate(params string[] inputs)
        {
            return inputs.Aggregate((left, right) =>
            {
                return $"{left}-{right}";
            });
        }
    }
}
