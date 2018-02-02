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

        public void IntricateQuery()
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
                        Count = 0,
                        Max = 0,
                        CloudEnvironment = CloudEnvironment.AzureChina.ToString()
                    };
                    TableOperation insertOperation = TableOperation.Insert(entity);
                    cloudTable.Execute(insertOperation);

                    var entity2 = new DemoTableEntity
                    {
                        PartitionKey = "PartitionKey",
                        RowKey = "RowKey2",
                        Name = "Tomcat2",
                        Count = 10,
                        Max = 5,
                        CloudEnvironment = CloudEnvironment.AzureChina.ToString()
                    };
                    TableOperation insertOperation2 = TableOperation.Insert(entity2);
                    cloudTable.Execute(insertOperation2);

                    // cannot compare columns in Azure table storage
                    TableQuery<DemoTableEntity> query = new TableQuery<DemoTableEntity>().Where(AndMultipleFilters(
                       TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "PartitionKey"),
                       TableQuery.GenerateFilterConditionForInt("Max", QueryComparisons.NotEqual, 1)
                   ));

                    var entities = cloudTable.ExecuteQuery(query);

                    Console.WriteLine("Count:" + entities.Count());
                    Console.WriteLine("name=" + entities.FirstOrDefault()?.Name);
                });
        }

        private string AndMultipleFilters(params string[] filters)
        {
            if (filters.Length <= 1)
                return filters.FirstOrDefault();

            string combined = filters[0];
            for (int i = 1; i < filters.Length; i++)
            {
                combined = TableQuery.CombineFilters(combined, TableOperators.And, filters[i]);
            }
            return combined;
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

        public void InsertOrUpdate()
        {
            StorageRunnerContext.RunOnTable(storage,
                "jwtesttablea" + Guid.NewGuid().ToString().Substring(0, 8),
                (cloudTable) =>
                {
                    TableOperation retrieveOperation = TableOperation.Retrieve<DemoTableEntity>("PartitionKey", "Tomcat");
                    TableResult retrieveResult = cloudTable.Execute(retrieveOperation);
                    DemoTableEntity result = (DemoTableEntity)retrieveResult.Result;
                    if (result != null)
                        Console.WriteLine("name=" + result.Name);
                    else
                    {
                        Console.WriteLine("It's null.");
                    }

                    result = new DemoTableEntity() { PartitionKey = "PartitionKey", RowKey = "Tomcat" };
                    TableOperation insertOrUpdateOp = TableOperation.InsertOrReplace(result);
                    cloudTable.Execute(insertOrUpdateOp);

                    result = (DemoTableEntity)cloudTable.Execute(retrieveOperation).Result;
                    Console.WriteLine(result.Timestamp.ToString());

                    TableOperation mergeOp = TableOperation.Merge(result);
                    cloudTable.Execute(mergeOp);

                    result = (DemoTableEntity)cloudTable.Execute(retrieveOperation).Result;
                    Console.WriteLine(result.Timestamp.ToString());
                });
        }

        public void EntityDoesntExistTest()
        {
            StorageRunnerContext.RunOnTable(storage,
                "jwtesttablea" + Guid.NewGuid().ToString().Substring(0, 8),
                (cloudTable) =>
                {
                    TableOperation retrieveOperation = TableOperation.Retrieve<DemoTableEntity>("PartitionKey", "Tomcat");
                    TableResult retrieveResult = cloudTable.Execute(retrieveOperation);
                    DemoTableEntity result = (DemoTableEntity)retrieveResult.Result;
                    if (result != null)
                        Console.WriteLine("name=" + result.Name);
                    else
                    {
                        Console.WriteLine("It's null.");
                    }
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
