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
            if (buffer.Any())
                return buffer.Pop();
            else
                return '\0';
        }

        public char PeekChar()
        {
            if (buffer.Any())
                return buffer.Peek();
            else
                return '\0';
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
