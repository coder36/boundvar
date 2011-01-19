using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

/**
 * Author Mark Middleton
 * My Solution to http://www.boundvariable.org challenge
 * 
 */
namespace BoundVar
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: boundvar <file>");
                return 1;                
            }

            string f = args[0];

            if ( !File.Exists( f ) )
            {
                Console.Error.WriteLine("File not found: " + f);
                return 1;
            }

            run( f );
            return 0;
        }

        public static void run(string filename)
        {
            var stack = new Stack<uint>();
            var reg = new uint[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            uint pc = 0, op, a, b, c, m;
            var d = 0;
            var mem = new Dictionary<uint, uint[]>();
                        
            // load memory
            using (var fs = new FileStream(filename, FileMode.Open))
            {
                var v = new List<uint>();
                while ((d = fs.ReadByte()) != -1)
                {
                    for (int i = 0; i < 3; i++) d = (d << 8) + fs.ReadByte();
                    v.Add((uint)d);
                }
                mem.Add(0, v.ToArray());
            }
            
            // spin
            while (true)
            {
                m = mem[0][pc++];
                op = (m >> 28) & 0xF;
                c = m & 0x7;
                b = (m >> 3) & 0x7;
                a = (m >> 6) & 0x7;

                if (stack.Count == 0) stack.Push((uint)mem.Count);
                switch (op)
                {
                    case 0: if (reg[c] != 0) reg[a] = reg[b]; break;
                    case 1: reg[a] = mem[reg[b]][reg[c]]; break;
                    case 2: mem[reg[a]][reg[b]] = reg[c]; break;
                    case 3: reg[a] = reg[b] + reg[c]; break;
                    case 4: reg[a] = reg[b] * reg[c]; break;
                    case 5: reg[a] = reg[b] / reg[c]; break;
                    case 6: reg[a] = ~(reg[b] & reg[c]); break;
                    case 7: return;
                    case 8: uint s = reg[c]; mem.Add(reg[b] = stack.Pop(), new uint[s]); break;
                    case 9: mem.Remove(reg[c]); stack.Push(reg[c]); break;
                    case 10: Console.Write((char)reg[c]); break;
                    case 11: reg[c] = (uint)Console.Read(); break;
                    case 12: pc = reg[c];  if (reg[b] != 0) mem[0] = (uint[])mem[reg[b]].Clone(); break;
                    case 13: reg[(m >> 25) & 0x7] = m & 0x1FFFFFF; break;
                    default: Console.WriteLine("\nGuru Meditation"); return;
                }
            }
        }
    }
}