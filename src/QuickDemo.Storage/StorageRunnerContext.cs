using Microsoft.WindowsAzure.Storage;
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

            action(cloudTable);

            cloudTable.DeleteIfExists();
        }
    }
}
