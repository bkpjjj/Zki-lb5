using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Crypto;

namespace DEST.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            DES dES = new DES("7bit");
            string en = dES.Encode("Hello World!");
            Console.WriteLine("Encoded:" + en);
            Console.ReadKey();
        }
    }
}
