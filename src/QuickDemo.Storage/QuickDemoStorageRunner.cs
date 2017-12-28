using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Storage
{
    public class QuickDemoStorageRunner
    {
        private static QuickDemoStorageRunner runner = new QuickDemoStorageRunner();
        private CloudStorageAccount storage = null;
        private QuickDemoStorageRunner()
        {
            var conn = ConfigurationManager.AppSettings["StorageConnectionString"];
            if (string.IsNullOrWhiteSpace(conn))
                throw new ArgumentNullException("StorageConnectionString");
            storage = CloudStorageAccount.Parse(conn);
        }
        public static QuickDemoStorageRunner Instance
        {
            get
            {
                return runner;
            }
        }

        public void TableCaseQuery()
        {
            // The conclusion is that: `Equals()` using OrdinalIgnoreCase is that working at all.

            StorageRunnerContext.RunOnTable(storage,
                "jwtesttablea" + Guid.NewGuid().ToString().Substring(0, 8),
                (cloudTable) =>
               {
                   var entity = new DemoTableEntity
                   {
                       PartitionKey = "PartitionKey",
                       RowKey = "RowKey",
                       Name = "Tomcat",
                       CloudEnvironment = CloudEnvironment.AzureChina.ToString()
                   };
                   TableOperation insertOperation = TableOperation.Insert(entity);
                   cloudTable.Execute(insertOperation);

                   var query = cloudTable.CreateQuery<DemoTableEntity>()
                   .Where(p => p.PartitionKey == "PartitionKey"
                   && p.Name.Equals("Tomcat", StringComparison.OrdinalIgnoreCase)).ToArray();

                   Console.WriteLine("name=" + query.FirstOrDefault()?.Name);
               });
        }

        public void SumTest()
        {
            StorageRunnerContext.RunOnTable(storage,
                  "jwtesttablea" + Guid.NewGuid().ToString().Substring(0, 8),
                  (cloudTable) =>
                  {
                      var entity = new DemoTableEntity
                      {
                          PartitionKey = "PartitionKey",
                          RowKey = "RowKey",
                          Name = "Tomcat",
                          Count = 3,
                      };
                      TableOperation insertOperation = TableOperation.Insert(entity);
                      cloudTable.Execute(insertOperation);

                      entity = new DemoTableEntity
                      {
                          PartitionKey = "PartitionKey",
                          RowKey = "RowKey2",
                          Name = "jerry",
                          Count = 100,
                      };
                      insertOperation = TableOperation.Insert(entity);
                      cloudTable.Execute(insertOperation);

                      var query = cloudTable.CreateQuery<DemoTableEntity>()
                    .Where(p => p.PartitionKey == "PartitionKey").AsEnumerable();

                      Console.WriteLine("Total count:" + query.Sum(p => p.Count));
                  });
        }
    }
}
