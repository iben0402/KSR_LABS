using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Services.Description;
using System.Xml;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using System.Security.Policy;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {

        public string CreateNewUser(string login, string haslo)
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            CloudTableClient cl = account.CreateCloudTableClient();
            var table = cl.GetTableReference("table1");

            table.CreateIfNotExists();
            
            var user = new TestEntity(login, "passwordBase");
            user.login = login;
            user.haslo = haslo;
            TableOperation op = TableOperation.Insert(user);


            TableOperation opCheck = TableOperation.Retrieve<TestEntity>(rowkey: login, partitionKey: "passwordBase");
            var res = table.Execute(opCheck);
            TestEntity e = (TestEntity)res.Result;

            if (e == null)
            {
                table.Execute(op);
                return "registered sucsesfull";
            }
            else
            {
                return "login present ERRROR";
            }
        }

        public Guid LoginUser(string login, string haslo)
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            CloudTableClient cl = account.CreateCloudTableClient();
            var table = cl.GetTableReference("table1");
            table.CreateIfNotExists();

            TableOperation opCheck = TableOperation.Retrieve<TestEntity>(rowkey: login, partitionKey: "passwordBase");
            var res = table.Execute(opCheck);

            TestEntity e = (TestEntity)res.Result;
            if (e != null && e.haslo == haslo)
            {
                var accountSession = CloudStorageAccount.DevelopmentStorageAccount;
                CloudTableClient clS = accountSession.CreateCloudTableClient();
                var tableS = clS.GetTableReference("Sessions");
                tableS.CreateIfNotExists();

                Guid sesGuid = Guid.NewGuid();
                var newSession = new TestEntity(sesGuid.ToString(), "sessionBase");
                newSession.login = login;
                newSession.haslo = haslo;
                TableOperation op = TableOperation.Insert(newSession);
                tableS.Execute(op);
                return sesGuid;
            }
            return Guid.Empty;
        }
        private TestEntity ifSessiaPResent(string guid)
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            CloudTableClient cl = account.CreateCloudTableClient();
            var table = cl.GetTableReference("Sessions");
            table.CreateIfNotExists();

            TableOperation opCheck = TableOperation.Retrieve<TestEntity>(rowkey: guid, partitionKey: "sessionBase");
            var res = table.Execute(opCheck);
            return (TestEntity)res.Result;
        }
        public string LogOut(string a)
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            CloudTableClient cl = account.CreateCloudTableClient();
            var table = cl.GetTableReference("Sessions");
            table.CreateIfNotExists();
            TestEntity e = ifSessiaPResent(a);
            if (e == null)
            {

                return "Wylogowanie nie powiodło się! nieoczekiwany błąd";
            }
            else
            {
                TableOperation op = TableOperation.Delete(e);
                table.Execute(op);
                return "WYLOGOWANO POMYŚLNIE";
            }
        }

        public string Put(string nazwa, string tresc, string id)
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            CloudBlobClient client = account.CreateCloudBlobClient();


            TestEntity e = ifSessiaPResent(id);
            if (e == null)
            {
                return "brak sesji";
            }
            CloudBlobContainer container = client.GetContainerReference(e.login);
            container.CreateIfNotExists();
            var blob = container.GetBlockBlobReference(nazwa);

            var bytes = new System.Text.ASCIIEncoding().GetBytes(tresc);
            var s = new System.IO.MemoryStream(bytes);
            blob.UploadFromStream(s);



            return "PUT SUCCESSFUL";


        }

        public string Get(string nazwa, string id)
        {
            TestEntity e = ifSessiaPResent(id);
            if (e == null)
            {
                return "brak sesji";
            }

            var account = CloudStorageAccount.DevelopmentStorageAccount;
            CloudBlobClient client = account.CreateCloudBlobClient();
            if (!client.GetContainerReference(e.login).Exists())
            {
                return "nieoczekiwany błąd";
            }
            CloudBlobContainer container = client.GetContainerReference(e.login);
            container.CreateIfNotExists();
            if (!container.GetBlockBlobReference(nazwa).Exists())
            {
                return "NO BLOB LIKE THIS EXISTS";
            }
            var blob = container.GetBlockBlobReference(nazwa);
            var s2 = new System.IO.MemoryStream();
            blob.DownloadToStream(s2);
            string content = System.Text.Encoding.UTF8.GetString(s2.ToArray());
            return content;

        }
    }
}
