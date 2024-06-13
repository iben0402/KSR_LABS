using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WCFServiceWebRole1
{
    public class Service1 : ICodingService
    {
        private readonly CloudQueue queue;
        private readonly CloudBlobContainer inputContainer;
        private readonly CloudBlobContainer outputContainer;

        public Service1()
        {
            var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            var queueClient = storageAccount.CreateCloudQueueClient();
            queue = queueClient.GetQueueReference("codingqueue");
            queue.CreateIfNotExists();

            var blobClient = storageAccount.CreateCloudBlobClient();
            inputContainer = blobClient.GetContainerReference("inputblobs");
            inputContainer.CreateIfNotExists();
            outputContainer = blobClient.GetContainerReference("outputblobs");
            outputContainer.CreateIfNotExists();
        }

        public void Koduj(string nazwa, string tresc)
        {
            var rnd = new Random();
            var num = rnd.Next(0, 3);
            if (num == 0)
            {
                throw new Exception("test");
            }
            var blob = inputContainer.GetBlockBlobReference(nazwa);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(tresc)))
            {
                blob.UploadFromStream(stream);
            }

            var message = new CloudQueueMessage(nazwa);
            queue.AddMessage(message);
        }

        public string Pobierz(string nazwa)
        {
            var blob = outputContainer.GetBlockBlobReference(nazwa);
            if (blob.Exists())
            {
                using (var stream = new MemoryStream())
                {
                    blob.DownloadToStream(stream);
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            return null;
        }
    }

}
