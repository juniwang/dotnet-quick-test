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
    }
}
