using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Storage
{
    public class DemoTableEntity : TableEntity
    {
        public DemoTableEntity() { }
        public DemoTableEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        public string Name { get; set; }
        public int Max { get; set; }
        public int Count { get; set; }
        public double Rate { get; set; }
        public string CloudEnvironment { get; set; }
    }

    public enum CloudEnvironment
    {
        Azure,
        AzureChina,
        AzureUSGovernment,
        AzureGermany,
    }
}
