using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class Tokenizer
    {
        public IBuffer buffer;
        public List<string> tokens = new List<string>();
        public Tokenizer(IBuffer buffer)
        {
            this.buffer = buffer;
        }


    }

    public interface IBuffer
    {
        char GetChar();
        char PeekChar();
        void UnGetChar(char c);
        void UnGetString(string s);
        bool EndOfInput();
    }
}
