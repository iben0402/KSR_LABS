using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komunikaty
{
    public interface Komunikat1
    {
        string tekst
        {
            get;
            set;
        }
    }
    public interface Komunikat2
    {
        int num { get; set; }
    }
    public interface Komunikat3 : Komunikat1, Komunikat2 { }
    public class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
