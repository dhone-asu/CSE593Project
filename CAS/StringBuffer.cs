using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class StringBuffer: IBuffer
    {
        public Stack<char> buffer = new Stack<char>();
        public StringBuffer(string str) 
        {
            UnGetString(str);
        }

        public char GetChar()
        {
            return buffer.Pop();
        }

        public char PeekChar()
        {
            return buffer.Peek(); 
        }

        public void UnGetChar(char c)
        {
            buffer.Push(c);
        }

        public void UnGetString(string str)
        {
            while (str.Any())
            {
                buffer.Push(str.Last());
                str = str.Remove(str.Length - 1);
            }
        }

        public bool EndOfInput()
        {
            return !buffer.Any();
        }
    }
}
