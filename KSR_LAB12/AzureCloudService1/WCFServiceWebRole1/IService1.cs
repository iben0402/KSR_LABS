using System.ServiceModel;
using System.ServiceModel.Web;

[ServiceContract]
public interface ICodingService
{
    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "/Koduj")]
    void Koduj(string nazwa, string tresc);

    [OperationContract]
    [WebGet(UriTemplate = "/Pobierz/{nazwa}")]
    string Pobierz(string nazwa);
}
