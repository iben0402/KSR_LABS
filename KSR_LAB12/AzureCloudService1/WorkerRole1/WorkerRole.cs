using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

public class WorkerRole : RoleEntryPoint
{
    private CloudQueue queue;
    private CloudBlobContainer inputContainer;
    private CloudBlobContainer outputContainer;

    public override bool OnStart()
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

        return base.OnStart();
    }

    public override void Run()
    {
        Trace.WriteLine("WorkerRole entry point called", "Information");

        while (true)
        {
            var message = queue.GetMessage();
            if (message != null)
            {
                var blobName = message.AsString;
                var blob = inputContainer.GetBlockBlobReference(blobName);
                using (var stream = new MemoryStream())
                {
                    blob.DownloadToStream(stream);
                    var content = Encoding.UTF8.GetString(stream.ToArray());
                    var encodedContent = ROT13(content);

                    var outputBlob = outputContainer.GetBlockBlobReference(blobName);
                    using (var outputStream = new MemoryStream(Encoding.UTF8.GetBytes(encodedContent)))
                    {
                        outputBlob.UploadFromStream(outputStream);
                    }

                    queue.DeleteMessage(message);
                }
            }
            Thread.Sleep(1000);
        }
    }

    private string ROT13(string input)
    {
        return new string(Array.ConvertAll(input.ToCharArray(), c =>
        {
            if (!char.IsLetter(c)) return c;
            var d = char.IsUpper(c) ? 'A' : 'a';
            return (char)((((c + 13) - d) % 26) + d);
        }));
    }
}
