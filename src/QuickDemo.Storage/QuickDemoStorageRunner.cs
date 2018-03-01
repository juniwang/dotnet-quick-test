using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Security.Cryptography;
using QuickDemo.Azure;

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

        public void ClientEncryption4Queue()
        {
            var kv = new KeyVaultTester("624c0e2f-6122-4b26-a229-06431f82e6b3", "C35CBFF9FA6C51E51E1DE97B6D1E246F27661301", "https://kvsignalrdeva.vault.azure.net");
            var name = "AzSignalR-Storage-ClientEncryptionKey";
            var rsaValue = kv.GetSecretAsync(name).Result;
            string kid = "signalrkeyid";

            StorageRunnerContext.RunOnQueue(storage,
                "jwtestqueuea" + Guid.NewGuid().ToString().Substring(0, 8),
                (cloudQueue) =>
                {
                    var csp1 = new RSACryptoServiceProvider(); 
                    csp1.FromXmlString(rsaValue);
                    var rsa1 = new RsaKey(kid, csp1);
                    QueueEncryptionPolicy policy = new QueueEncryptionPolicy(rsa1, null);
                    QueueRequestOptions options = new QueueRequestOptions() { EncryptionPolicy = policy };
                    cloudQueue.AddMessage(new CloudQueueMessage("messageContentABC"), null, null, options, null);

                    // Retrieve message
                    var csp2 = new RSACryptoServiceProvider();
                    csp2.FromXmlString(rsaValue);
                    var rsa2 = new RsaKey(kid, csp2);
                    QueueEncryptionPolicy policy2 = new QueueEncryptionPolicy(rsa2, null);
                    QueueRequestOptions options2 = new QueueRequestOptions() { EncryptionPolicy = policy2 };
                    CloudQueueMessage retrMessage = cloudQueue.GetMessage(null, options2, null);
                    Console.WriteLine(retrMessage.AsString);
                });
        }

        public void ClientEncryption4Table()
        {
            StorageRunnerContext.RunOnTable(storage,
              "jwtesttablea" + Guid.NewGuid().ToString().Substring(0, 8),
              (cloudTable) =>
              {
                  RsaKey key = new RsaKey(Guid.NewGuid().ToString());
                  TableEncryptionPolicy policy = new TableEncryptionPolicy(key, null);
                  TableRequestOptions options = new TableRequestOptions()
                  {
                      EncryptionPolicy = policy,
                      EncryptionResolver = (pk, rk, propName) =>
                      {
                          if (propName == "Name")
                              return true;
                          return false;
                      }
                  };

                  TableOperation retrieveOperation = TableOperation.Retrieve<DemoTableEntity>("PartitionKey", "Tomcat");
                  TableResult retrieveResult = cloudTable.Execute(retrieveOperation, options);
                  DemoTableEntity result = (DemoTableEntity)retrieveResult.Result;
                  if (result != null)
                      Console.WriteLine("name=" + result.Name);
                  else
                  {
                      Console.WriteLine("It's null.");
                  }

                  result = new DemoTableEntity() { PartitionKey = "PartitionKey", RowKey = "Tomcat", Rate = 1.55, Name = "originalname" };
                  TableOperation insertOrUpdateOp = TableOperation.Insert(result);
                  cloudTable.Execute(insertOrUpdateOp, options);

                  result = (DemoTableEntity)cloudTable.Execute(retrieveOperation, options).Result;
                  Console.WriteLine(result.Timestamp.ToString());

                  // https://github.com/Azure/azure-storage-net/blob/c1966e1cd64d5eb8e40733ebd6b07b6b8e8fb136/Lib/ClassLibraryCommon/Table/Protocol/TableOperationHttpRequestFactory.cs
                  // Merge or InsertOrMerge are not supported.
                  result.Max = 1000;
                  TableOperation mergeOps = TableOperation.Merge(result);
                  result = (DemoTableEntity)cloudTable.Execute(mergeOps).Result;

                  result = (DemoTableEntity)cloudTable.Execute(retrieveOperation, options).Result;
                  Console.WriteLine(result.Timestamp.ToString());

                  var segment = cloudTable.ExecuteQuerySegmented(cloudTable.CreateQuery<DemoTableEntity>(), null, options);
                  var list = segment.Results;
                  Console.WriteLine(list.Count);
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

                    result = new DemoTableEntity() { PartitionKey = "PartitionKey", RowKey = "Tomcat", Rate = 1.55 };
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
