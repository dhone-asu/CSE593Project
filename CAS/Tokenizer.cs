using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class Tokenizer
    {
        public Stack<char> buffer;
        public Tokenizer(string str)
        {
            foreach (char c in str)
            {
                buffer.Push(c);
            }
        }
    }
}
