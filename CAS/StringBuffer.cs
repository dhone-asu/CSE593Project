using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class StringBuffer: IBuffer
    {
        public string buffer = "";
        public StringBuffer(string str) 
        {
            UnGetString(str);
        }

        public char GetChar()
        {
            if (!EndOfInput())
            {
                char c = buffer[0];
                buffer = buffer.Remove(0, 1);
                return c;
            }
            else
                return '\0';
        }

        public char PeekChar()
        {
            if (!EndOfInput())
                return buffer[0];
            else
                return '\0';
        }

        public void UnGetChar(char c)
        {
            buffer = c + buffer;
        }

        public void UnGetString(string str)
        {
            buffer = str + buffer;
        }

        public bool EndOfInput()
        {
            return buffer.Length == 0;
        }
    }
}
