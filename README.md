# The Cult of the Bound Variable

A while back I came accross a programming challenge - [The Cult of the Bound Variable](http://www.boundvariable.org/task.shtml)  The challenge was described as an [ancient specification written in sandstone for a universal machine](http://www.boundvariable.org/um-spec.txt).  This really sparked my interest  - the task was to write an emulator!  The question to myself, was how many lines of code can it take to write an emulator ?

## Java
I first tried a java solution.  The problem with java is does not natively support unsigned 32 bit integers.  You can emulate an unsigned 32 bit integer using a java int, then apply bit masks when ever you do any operations on it, but this rapidly got messy and soon the code started to loose any sort of elegance.  It become ugly!  It could be done, but I couldn't find a way of keeping it simple.

## C #
In the end I settled on C#.  I was working on a C# job at the time (2010), so it seemed a good oppertunity to see how beautiful the language really is.  You can use c# in semi dynamic way (you don't have to declare variable types), which made the solution far less verbose and importantly it supports unsigned integers.

As a programmer, you get an instinct for when you are on the right track.  One thing that was true about my solution, was that as I refactored, it became simpler and simpler, till there was nothing more I could do.  I had obviously hit on a universal truth for the C# bound variable univeral machine!   Here is what I'm talking about:

    var stack = new Stack<uint>();
    var reg = new uint[] { 0, 0, 0, 0, 0, 0, 0, 0 };
    uint pc = 0, op, a, b, c, m;
    var mem = new Dictionary<uint, uint[]>();
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

## Conclusion

In C# it took about 33 lines of code to define a virtual machine.  Of course you still need to load the virtual machines memory with a program, and you need all the various boiler plate stuff a C# project needs, but as a pure language, C# is elegent and pleasant to work with.  See [here](https://github.com/coder36/boundvar/blob/master/BoundVar/Program.cs) for the complete solution.
