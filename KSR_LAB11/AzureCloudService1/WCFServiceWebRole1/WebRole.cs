using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Data.Services.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Concurrent;

namespace WCFServiceWebRole1
{
    public class TestEntity : TableEntity
    {
        public TestEntity(string rk, string pk)
        {
            this.PartitionKey = pk; // ustawiamy klucz partycji
            this.RowKey = rk; // ustawiamy klucz g³ówny
                              // this.Timestamp;// jest tylko do odczytu
        }
        public TestEntity() { }
        public string login { get; set; }
        public string haslo { get; set; }
    }

    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
