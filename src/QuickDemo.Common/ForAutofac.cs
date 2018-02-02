using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Common
{
    public interface IForAutofac
    {
        string Name { get; }
    }

    public class ForAutofac : IForAutofac
    {
        public string Name => "Testing";
    }

}
