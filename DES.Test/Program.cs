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
            Console.OutputEncoding = Encoding.Unicode;
            DES dES = new DES("aaaa");
            string en = dES.Encode(File.ReadAllText(@"text put here"));
            Console.WriteLine("Encoded:" + en);
            string dc = dES.Decode(en);
            Console.WriteLine("Decoded:" + dc);
            Console.ReadKey();
        }
    }
}
