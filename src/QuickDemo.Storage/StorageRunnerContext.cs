using Microsoft.Azure.KeyVault;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Storage
{
    public static class StorageRunnerContext
    {
        public static void RunOnTable(CloudStorageAccount storage, string tableName, Action<CloudTable> action)
        {
            var cloudTable = storage.CreateCloudTableClient().GetTableReference(tableName);
            cloudTable.CreateIfNotExists();

            try
            {
                action(cloudTable);
            }
            finally
            {
                cloudTable.DeleteIfExists();
            }
        }

        public static void RunOnQueue(CloudStorageAccount storage, string queueName, Action<CloudQueue> action)
        {
            var queueClient = storage.CreateCloudQueueClient();

            //RsaKey key = new RsaKey(Guid.NewGuid().ToString());
            //queueClient.DefaultRequestOptions.EncryptionPolicy = new QueueEncryptionPolicy(key, null);

            var cloudQueue = queueClient.GetQueueReference(queueName);

            cloudQueue.CreateIfNotExists();

            try
            {
                action(cloudQueue);
            }
            finally
            {
                cloudQueue.DeleteIfExists();
            }
        }
    }
}
