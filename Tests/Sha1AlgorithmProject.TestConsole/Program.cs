using Sha1AlgorithmProject.Math;
using System.Text;

namespace Sha1AlgorithmProject.TestConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var message = "abc"; // 3
            //var message = "абс";

            //var message = "All variables are unsigned 32-bit quantities and wrap modulo 232"; // 64
            //var message = "Все переменные являются беззнаковыми 32-битными величинами и пер";

            //var message = "All variables are unsigned 32-bit values into values tes"; // 56
            //var message = "Все переменные являются беззнаковыми 32-битными величины";

            var message = "Some exciting news and a brand new Internet home for IW — Hello writers, readers, and ity! What"; // 95
            //var message = "Несколько интересных новостей и совершенно новый Интернет-дом для IW — Здравствуйте, пиасатели,";  

            var sha1 = new Sha1();
            Console.WriteLine(sha1.GetHash(message));
        }
    }
}

