using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LMSAssembler;

namespace EvCppPlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (String a in args)
            {
                Console.WriteLine("{0}", a);
            }

            if (args[0].Equals("asmLms"))
            {
                if(args.Length != 3)
                {
                    Console.WriteLine("Error 0: No all params [see help]");
                    return;
                }
                Assembler asm = new Assembler();
                //FileStream fs = new FileStream(f, FileMode.Open, FileAccess.Read);
                FileStream fs = new FileStream(args[1], FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(args[2], FileMode.Create, FileAccess.Write);

                List<String> errors = new List<String>();
                asm.Assemble(fs, fout, errors);
                if (errors.Count != 0)
                {
                    Console.WriteLine("error in assembler for lms2012");
                    foreach (String a  in errors)
                    {
                        Console.WriteLine(a);
                    }
                }
            } else if (args[0].Equals("disasmRbf"))
            {
                if(args.Length != 2)
                {
                    Console.WriteLine("Error 0: No all params [see help]");
                    return;
                }

                Assembler asm = new Assembler();
                FileStream from = new FileStream(args[1], FileMode.Open, FileAccess.Read);

                asm.Disassemble(from, Console.Out);
            }
            Console.ReadKey();

        }
    }
}
