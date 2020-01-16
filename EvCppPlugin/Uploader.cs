using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LMSAssembler;
using EV3Communication;


namespace EvCppPlugin
{
    class Uploader
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
                if(args.Length != 3)
                {
                    Console.WriteLine("Error 0: No all params [see help]");
                    return;
                }

                Assembler asm = new Assembler();
                FileStream from = new FileStream(args[1], FileMode.Open, FileAccess.Read);

                asm.Disassemble(from, Console.Out);
            }else if (args[0].Contains("download"))
            {
                if(args.Length != 3)
                {
                    Console.WriteLine("Error 0: No all params [see help]");
                    return;
                }
                EV3Connection connection = ConnectionFinder.CreateConnection(true, true);
                Console.WriteLine("Connection {0}", connection.IsOpen());
                byte[] data = null;
                try
                {
                    data = connection.ReadEV3File(args[1]);
                } catch(Exception e)
                {
                    Console.WriteLine("{0}", e.ToString());
                }

                FileStream fileStream = new FileStream(args[2], FileMode.Create, FileAccess.Write);
                fileStream.Write(data, 0, data.Length);
            }else if (args[0].Contains("upload"))
            {
                if(args.Length != 3)
                {
                    Console.WriteLine("Error 0: No all params [see help]");
                    return;
                }
                EV3Connection connection = ConnectionFinder.CreateConnection(true, true);
                FileStream from = new FileStream(args[2], FileMode.Open, FileAccess.Read);
                byte[] data = new byte[from.Length];

                from.Read(data, 0, (int) from.Length);

                connection.CreateEV3File(args[1], data);
                connection.Close();
            } else if (args.Contains("execute"))
            {
                if(args.Length!= 2)
                {
                    Console.WriteLine("Error 0: No all params [see help]");
                    return;
                }
                EV3Connection connection = ConnectionFinder.CreateConnection(true, true);
                Console.WriteLine("Trying to start: " + args[1]);

                ByteCodeBuffer c = new ByteCodeBuffer();
                // load and start it
                c.OP(0xC0);       // opFILE
                c.CONST(0x08);    // CMD: LOAD_IMAGE = 0x08
                c.CONST(1);       // slot 1 = user program slot
                c.STRING(args[1]);
                c.GLOBVAR(0);
                c.GLOBVAR(4);
                c.OP(0x03);       // opPROGRAM_START
                c.CONST(1);       // slot 1 = user program slot
                c.GLOBVAR(0);
                c.GLOBVAR(4);
                c.CONST(0);

                connection.DirectCommand(c, 10, 0);
            }

            Console.ReadKey();

        }
    }
}
