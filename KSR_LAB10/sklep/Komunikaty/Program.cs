using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komunikaty
{
    public class StartZamowienia
    {
        public Guid CorrelationId { get; set; } = Guid.NewGuid();
        public int amount { get; set; }
        public string ClientId { get; set; }
    }


    public class PytanieoPotwierdzenie
    {
        public Guid CorrelationId { get; set; }
        public int amount { get; set; }
        public string ClientId { get; set; }
    }

    public class Potwierdzenie
    {
        public Guid CorrelationId { get; set; }
        public int amount { get; set; }
    }

    public class BrakPotwierdzenia
    {
        public Guid CorrelationId { get; set; }
        public int amount { get; set; }
    }

    public class PytanieoWolne
    {
        public Guid CorrelationId { get; set; }
        public int amount { get; set; }
    }

    public class OdpowiedzWolne
    {
        public Guid CorrelationId { get; set; }
        public int amount { get; set; }
    }

    public class OdpowiedzWolneNegatywna
    {
        public Guid CorrelationId { get; set; }
        public int amount { get; set; }
    }

    public class AkceptacjaZamowienia
    {
        public Guid CorrelationId { get; set; }
        public int amount { get; set; }
        public string ClientId { get; set; }
    }

    public class OdrzucenieZamowienia
    {
        public Guid CorrelationId { get; set; }
        public int amount { get; set; }
        public string ClientId { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
