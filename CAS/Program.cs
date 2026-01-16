using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string line = Console.ReadLine();
            StringBuffer b = new StringBuffer(line);
            Tokenizer tokenizer = new Tokenizer(b);
            return;
        }
    }
}
