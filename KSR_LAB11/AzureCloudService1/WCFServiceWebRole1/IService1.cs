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

namespace WCFServiceWebRole1
{
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        [WebGet(UriTemplate = "CreateUser/{a}/{b}")]
        string CreateNewUser(string a, string b);

        [OperationContract]
        [WebGet(UriTemplate = "LoginUser/{a}/{b}")]
        Guid LoginUser(string a, string b);

        [OperationContract]
        [WebGet(UriTemplate = "LogOut/{id}")]
        string LogOut(string id);

        [OperationContract]
        [WebGet(UriTemplate = "Put/{nazwa}/{tresc}/{id}")]
        string Put(string nazwa, string tresc, string id);

        [OperationContract]
        [WebGet(UriTemplate = "Get/{nazwa}/{id}")]
        string Get(string nazwa, string id);
    }
}
