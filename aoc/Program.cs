using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Experiments;
using MoreLinq.Extensions;

namespace aoc
{
    class Program
    {
        static void Main(string[] args)
        {
            var line = File.ReadAllText("/Users/spaceorc/Downloads/input.txt");
            var program = line.Split(',').Select(long.Parse).ToArray();
            
            string lastOutput = "";
            
            var computer = new Computer("prog", program)
            {
                Output = value =>
                {
                    lastOutput += (char) (int) value;
                    Console.Out.Write((char) (int) value);
                }
            };

            // var tiles = new Dictionary<string, string[]>();
            // var tileItems = new Dictionary<string, string[]>();
            // var passages = new Dictionary<(string, string), string>();
            //
            // string pos = null;
            // string command = null;
            //
            // string InvCmd(string cmd)
            // {
            //     switch (cmd)
            //     {
            //         case "north": return "south";
            //         case "south": return "north";
            //         case "east": return "west";
            //         case "west": return "east";
            //         default: throw new Exception($"Bad inv: {cmd}");
            //     }
            // }

            //WriteState();

            computer.Input.OnWait += () =>
            {
                var output = lastOutput.Split('\n');
                var doors = output.SkipWhile(x => x != "Doors here lead:").Skip(1).TakeWhile(x => x.StartsWith("- ")).Select(x => x.Substring(2)).ToArray();
                var items = output.SkipWhile(x => x != "Items here:").Skip(1).TakeWhile(x => x.StartsWith("- ")).Select(x => x.Substring(2)).ToArray();
                lastOutput = "";
                
                // tiles[room] = doors;
                // tileItems[room] = items;
                //
                // if (command != null)
                // {
                //     passages[(pos, command)] = room;
                //     passages[(room, InvCmd(command))] = pos;
                // }
                //
                // pos = room;
                //
                // //WriteState();
                //
                // foreach (var door in doors)
                // {
                //     if (!passages.ContainsKey((room, door)))
                //     {
                //         Console.Out.WriteLine($"Command: {door}");
                //         command = door;
                //         computer.Input.Send((command + "\n").Select(x => (long) x).ToArray());
                //         return;
                //     }
                // }
                //
                // var queue = new Queue<string>();
                // queue.Enqueue(pos);
                // var used = new Dictionary<string, string> {{pos, pos}};
                //
                // while (queue.Count > 0)
                // {
                //     var cur = queue.Dequeue();
                //     var curDoors = tiles[cur];
                //     foreach (var door in curDoors)
                //     {
                //         if (passages.TryGetValue((cur, door), out var next))
                //         {
                //             if (used.ContainsKey(next))
                //                 continue;
                //             used.Add(next, cur);
                //             queue.Enqueue(next);
                //         }
                //         else
                //         {
                //             while (true)
                //             {
                //                 var prev = used[cur];
                //                 if (prev == pos)
                //                 {
                //                     command = passages.Single(p => p.Key.Item1 == pos && p.Value == cur).Key.Item2;
                //                     pos = cur;
                //                     Console.Out.WriteLine($"Command: {command}");
                //                     computer.Input.Send((command + "\n").Select(x => (long) x).ToArray());
                //                     return;
                //                 }
                //             
                //                 cur = prev;
                //             }
                //         }
                //     }
                // }
                //
                // throw new Exception("WTF! Couldn't find path");
                //
                //
                Console.Write("> ");
                var cmd = Preprocess(Console.ReadLine());
                Console.WriteLine("COMMAND: " + cmd);
                var inp = (cmd + '\n').Select(x => (long) x).ToArray();
                computer.Input.Send(inp);
                
                string Preprocess(string s)
                {
                    switch (s)
                    {
                        case "n":
                            return "north";
                        case "s":
                            return "south";
                        case "w":
                            return "west";
                        case "e":
                            return "east";
                        default:
                            if (s.StartsWith("t "))
                            {
                                var its = items.Where(x => x.Contains(s.Substring(2))).ToList();
                                if (its.Count == 1)
                                    return "take " + its[0];
                                return "take " + s.Substring(2);
                            }

                            return s;
                    }
                }
            };            

            var task = computer.Run();
            if (task.IsCompleted)
                task.Wait();
            else
                throw new Exception("Didn't complete!");


            // foreach (var kvp in tileItems)
            // {
            //     foreach (var s in kvp.Value)
            //     {
            //         Console.Out.WriteLine(s);
            //     }
            // }
            
        }

        static void Main24(string[] args)
        {
            var lines = File.ReadAllLines("/Users/spaceorc/Downloads/input.txt");
//             var lines = @"
// ....#
// #..#.
// #..##
// ..#..
// #....
//
// ".Trim().Split('\n').Select(x => x.Trim()).ToArray();            
            int bugs = 0;
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (lines[y][x] == '#')
                        bugs |= 1 << (y * 5 + x);
                }
            }

            var levels = new int[205];
            var next = new int[205];
            levels[101] = bugs;

            for (int i = 0; i < 200; i++)
            {
                Array.Fill(next, 0);
                for (int level = 1; level <= levels.Length - 2; level++)
                    next[level] = Iteration(levels, level);

                var tmp = levels;
                levels = next;
                next = tmp;
            }

            var n = 0;
            foreach (var v in levels)
            {
                for (int y = 0; y < 5; y++)
                {
                    for (int x = 0; x < 5; x++)
                    {
                        var b = y * 5 + x;
                        if ((v & (1 << b)) != 0)
                            n++;
                    }
                }
            }

            Console.Out.WriteLine(n);
            
            // var set = new HashSet<int>();
            // while (set.Add(bugs))
            // {
            //     for (int y = 0; y < 5; y++)
            //     {
            //         for (int x = 0; x < 5; x++)
            //         {
            //             var b = y * 5 + x;
            //             if ((bugs & (1 << b)) == 0)
            //                 Console.Write('.');
            //             else
            //                 Console.Write('#');
            //         }
            //
            //         Console.WriteLine();
            //     }
            //     Console.WriteLine();
            //     
            //     bugs = Iteration(bugs);
            //
            // }

            //Console.Out.WriteLine(bugs);

            int Iteration(int[] levels, int level)
            {
                var old = levels[level];
                int next = 0;
                for (int y = 0; y < 5; y++)
                {
                    for (int x = 0; x < 5; x++)
                    {
                        if (y == 2 && x == 2)
                            continue;
                        
                        var b = y * 5 + x;
                        
                        var n = 0;
                        if (y > 0)
                            n += (old >> (b - 5)) & 1;
                        else
                        {
                            var prev = levels[level - 1];
                            if ((prev & (1 << 7)) != 0)
                                n++;
                        }

                        if (y < 5 - 1)
                            n += (old >> (b + 5)) & 1;
                        else
                        {
                            var prev = levels[level - 1];
                            if ((prev & (1 << 17)) != 0)
                                n++;
                        }

                        if (x > 0)
                            n += (old >> (b - 1)) & 1;
                        else
                        {
                            var prev = levels[level - 1];
                            if ((prev & (1 << 11)) != 0)
                                n++;
                        }

                        if (x < 5 - 1)
                            n += (old >> (b + 1)) & 1;
                        else
                        {
                            var prev = levels[level - 1];
                            if ((prev & (1 << 13)) != 0)
                                n++;
                        }

                        if (x == 2 && y == 1)
                        {
                            var prev = levels[level + 1];
                            for (int i = 0; i < 5; i++)
                            {
                                if ((prev & (1 << i)) != 0)
                                    n++;    
                            }
                        }
                        if (x == 2 && y == 3)
                        {
                            var prev = levels[level + 1];
                            for (int i = 0; i < 5; i++)
                            {
                                if ((prev & (1 << (i + 20))) != 0)
                                    n++;    
                            }
                        }
                        if (x == 1 && y == 2)
                        {
                            var prev = levels[level + 1];
                            for (int i = 0; i < 5; i++)
                            {
                                if ((prev & (1 << (i * 5))) != 0)
                                    n++;    
                            }
                        }
                        if (x == 3 && y == 2)
                        {
                            var prev = levels[level + 1];
                            for (int i = 0; i < 5; i++)
                            {
                                if ((prev & (1 << (i * 5 + 4))) != 0)
                                    n++;    
                            }
                        }

                        if ((old & (1 << b)) == 0 && (n == 1 || n == 2))
                            next |= 1 << b;
                        if ((old & (1 << b)) != 0 && n == 1)
                            next |= 1 << b;
                    }
                }

                return next;
            }
        }
        
        static void Main23(string[] args)
        {
            var line = File.ReadAllText("/Users/spaceorc/Downloads/input.txt");

            var program = line.Split(',').Select(long.Parse).ToArray();

            var computers = new List<Computer>();
            var natX = 0L;
            var natY = 0L;
            var natReady = false;
            for (int i = 0; i < 50; i++)
            {
                var computer = new Computer(i.ToString(), program);
                computer.Input.Send(i);
                var state = 0;
                var target = 0;
                var x = 0L;
                var y = 0L;
                computer.Output = value =>
                {
                    switch (state)
                    {
                        case 0:
                            target = (int) value;
                            state = 1;
                            break;
                        case 1:
                            x = value;
                            state = 2;
                            break;
                        case 2:
                            y = value;
                            state = 0;
                            if (target == 255)
                            {
                                natReady = true;
                                natX = x;
                                natY = y;
                                break;
                                
                            }
                            computers[target].Input.Send(x, y);
                            break;
                        
                    }
                };
                computers.Add(computer);
            }

            var tasks = new List<Task>(); 
            foreach (var computer in computers)
            {
                tasks.Add(computer.Run());
            }

            var lastNatY = long.MinValue;
            while (tasks.Any(t => !t.IsCompleted))
            {
                var faulted = tasks.FirstOrDefault(t => t.IsFaulted);
                faulted?.Wait();

                if (computers.All(c => !c.Input.DataReady) && natReady)
                {
                    Console.Out.WriteLine(natY);
                    if (natY == lastNatY)
                        break;

                    lastNatY = natY;
                    computers[0].Input.Send(natX, natY);
                }
                else
                {
                    foreach (var computer in computers)
                    {
                        if (!computer.Input.DataReady)
                            computer.Input.Send(-1);
                    }
                }
            }
            
            
            
        }

        static void Main22(string[] args)
        {
            var lines = File.ReadAllLines("/Users/spaceorc/Downloads/input.txt");

            //var pos = 2020L;
            //26713960706268

            // for (int i = 0; i < times; i++)
            // {
            //     if (i % 1000000 == 0)
            //         Console.Out.WriteLine(i);
            //     pos = Solve(pos, count);
            //     if (pos == 2020L)
            //     {
            //         Console.Out.WriteLine($"Cycle size = {i + 1}");
            //         break;
            //     }
            // }
//             Array.Reverse(lines);
//
            // const int INVERT = 0;
            // const int CUT = 1;
            // const int INCREMENT = 2;
            // var ops = new List<(int op, long operand)>();
            // foreach (var line in lines)
            // {
            //     if (line == "deal into new stack")
            //     {
            //         //ops.Add((INVERT, 0));
            //         ops.Add((INCREMENT, -1));
            //         ops.Add((CUT, 1));
            //         // pos = count - pos - 1;
            //         // Console.Out.WriteLine("pos = count - pos - 1;");
            //     }
            //     else if (line.StartsWith("cut "))
            //     {
            //         var cut = int.Parse(line.Substring("cut ".Length));
            //         ops.Add((CUT, cut));
            //         // pos = (pos + cut + count) % count;
            //         // Console.Out.WriteLine($"pos = (pos + {cut} + count) % count;");
            //     }
            //     else if (line.StartsWith("deal with increment "))
            //     {
            //         var increment = int.Parse(line.Substring("deal with increment ".Length));
            //         ops.Add((INCREMENT, increment));
            //         // var solved = false;
            //         // for (int n = 0; n <= increment; n++)
            //         // {
            //         //     if ((pos + n * count) % increment == 0)
            //         //     {
            //         //         solved = true;
            //         //         pos = (pos + n * count) / increment;
            //         //         break;
            //         //     }
            //         // }
            //         //
            //         // if (!solved)
            //         //     throw new Exception("WTF?");
            //     }
            //     else
            //         throw new Exception("WTF");
            // }
            //
            // for (int i = 0; i < ops.Count; i++)
            // {
            //     for (int k = 0; k < ops.Count - 1; k++)
            //     {
            //         if (ops[k].op == CUT)
            //         {
            //             if (ops[k + 1].op == INVERT)
            //             {
            //                 var t = ops[k];
            //                 ops[k] = ops[k + 1];
            //                 ops[k + 1] = (t.op, -t.operand);
            //             }
            //             else if (ops[k + 1].op == INCREMENT)
            //             {
            //                 var t = ops[k];
            //                 ops[k] = ops[k + 1];
            //                 ops[k + 1] = (t.op, (long) ((BigInteger)t.operand * ops[k].operand % count));
            //             }
            //         }
            //         // else if (ops[k].op == INVERT)
            //         // {
            //         //     if (ops[k + 1].op == INCREMENT)
            //         //     {
            //         //         var t = ops[k];
            //         //         ops[k] = ops[k + 1];
            //         //         ops[k + 1] = t;
            //         //         ops.Insert(k + 2, (CUT, 1));
            //         //     }
            //         // }
            //     }
            // }
            //
            // for (int i = ops.Count - 1; i >= 1; i--)
            // {
            //     if (ops[i].op == ops[i - 1].op && ops[i].op == CUT)
            //     {
            //         ops[i - 1] = (CUT, ops[i - 1].operand + ops[i].operand);
            //         ops.RemoveAt(i);
            //     }
            //     else if (ops[i].op == ops[i - 1].op && ops[i].op == INCREMENT && ops[i].operand == -1)
            //     {
            //         ops.RemoveAt(i);
            //     }
            //
            //     // else if (ops[i].op == ops[i - 1].op && ops[i].op == INCREMENT)
            //     // {
            //     //     ops[i - 1] = (INCREMENT, (long) (((BigInteger)ops[i - 1].operand * ops[i].operand % count + count) % count));
            //     //     ops.RemoveAt(i);
            //     // }
            // }
            //
            // for (int i = ops.Count - 1; i >= 1; i--)
            // {
            //     if (ops[i].op == ops[i - 1].op && ops[i].op == INCREMENT)
            //     {
            //         ops[i - 1] = (INCREMENT, (long) ((BigInteger) ops[i - 1].operand * ops[i].operand % count));
            //         ops.RemoveAt(i);
            //     }
            // }
            // // 7209477733074599936
            //
            // //Console.Out.WriteLine(ops.Count(o => o.op == INVERT));
            

            //var pos = 26713960706268;
            // foreach (var tuple in ops)
            // {
            //     Console.Out.WriteLine($"{tuple.op} {tuple.operand}");
            //     switch (tuple.op)
            //     {
            //         // case INVERT:
            //         //     //pos = count - pos - 1;
            //         //     pos = (-pos + count) % count;
            //         //     break;
            //         case CUT:
            //             pos = (pos - tuple.operand + count) % count;
            //             break;
            //         case INCREMENT:
            //             pos = (long) (((BigInteger)pos * tuple.operand + count) % count);
            //             break;
            //     }
            // }

            const long count = 119315717514047;
            const long times = 101741582076661;

            
            // Console.Out.WriteLine($"solv1 = {Solve(2020, count)}");
            // Console.Out.WriteLine($"solv2 = {Solve(Solve(2020, count), count)}");
            
            BigInteger a = 91244990107807;
            BigInteger b = 73268744725299;

            // Console.Out.WriteLine((Solve(2020, count) * a + b)%count);
            // Console.Out.WriteLine((Solve(Solve(2020, count), count) * a + b)%count);


            var at = Power(a, times, count);
            var b1 = at - a;
            //var b2 = a - 1;
            var div_1_a_minus_1 = 64449238758087;
            //var b3 = b1 / b2;
            var ob3 = b1 * div_1_a_minus_1;
            var b4 = ob3 + 1;
            var b5 = b4 * b;
            var bb = (b5 % count + count)%count;
            //bb = (a * b + b) % count;

            Console.Out.WriteLine($"at = {at}");
            Console.Out.WriteLine($"bb = {bb}");
            Console.Out.WriteLine($"bb2 = {(a * b + b) % count}");
            
            //Console.Out.WriteLine($"solv -> {}");

            //var y = (((2020 - bb) % count) + count) % count;

            var atrev = 66385277977328;
            //var atrev2 = 47283546981980;
            //var atrev1 = 29313613460272;

            var x = ((atrev * 2020 - atrev * bb) % count + count);

            Console.Out.WriteLine(x);

            BigInteger Power(BigInteger a, BigInteger power, BigInteger count)
            {
                BigInteger aPowerTimes = 1;
                BigInteger pp = 0;
                while (pp < power)
                {
                    BigInteger p = 1;
                    BigInteger at = a;
                    while (p * 2 <= power - pp)
                    {
                        at = at * at % count;
                        p *= 2;
                    }

                    pp += p;
                    aPowerTimes = aPowerTimes * at % count;
                }

                return (aPowerTimes + count) % count;
            }
        }

        static BigInteger Solve(BigInteger pos, BigInteger count)
        {
            pos = count - pos - 1;
            pos = (pos + -6969 + count) % count;
            {
                for (int n = 0; n < 45; n++)
                {
                    if ((pos + n * count) % 45 == 0)
                    {
                        pos = (pos + n * count) / 45;
                        break;
                    }
                }
            }
            pos = count - pos - 1;
            pos = (pos + 9677 + count) % count;
            {
                for (int n = 0; n < 10; n++)
                {
                    if ((pos + n * count) % 10 == 0)
                    {
                        pos = (pos + n * count) / 10;
                        break;
                    }
                }
            }
            pos = (pos + 3980 + count) % count;
            {
                for (int n = 0; n < 75; n++)
                {
                    if ((pos + n * count) % 75 == 0)
                    {
                        pos = (pos + n * count) / 75;
                        break;
                    }
                }
            }
            pos = count - pos - 1;
            pos = (pos + -4047 + count) % count;
            {
                for (int n = 0; n < 70; n++)
                {
                    if ((pos + n * count) % 70 == 0)
                    {
                        pos = (pos + n * count) / 70;
                        break;
                    }
                }
            }
            pos = (pos + 6489 + count) % count;
            {
                for (int n = 0; n < 45; n++)
                {
                    if ((pos + n * count) % 45 == 0)
                    {
                        pos = (pos + n * count) / 45;
                        break;
                    }
                }
            }
            pos = (pos + -7220 + count) % count;
            {
                for (int n = 0; n < 31; n++)
                {
                    if ((pos + n * count) % 31 == 0)
                    {
                        pos = (pos + n * count) / 31;
                        break;
                    }
                }
            }
            pos = (pos + -6509 + count) % count;
            {
                for (int n = 0; n < 30; n++)
                {
                    if ((pos + n * count) % 30 == 0)
                    {
                        pos = (pos + n * count) / 30;
                        break;
                    }
                }
            }
            pos = (pos + -3405 + count) % count;
            {
                for (int n = 0; n < 22; n++)
                {
                    if ((pos + n * count) % 22 == 0)
                    {
                        pos = (pos + n * count) / 22;
                        break;
                    }
                }
            }
            pos = (pos + -3893 + count) % count;
            {
                for (int n = 0; n < 72; n++)
                {
                    if ((pos + n * count) % 72 == 0)
                    {
                        pos = (pos + n * count) / 72;
                        break;
                    }
                }
            }
            pos = (pos + 915 + count) % count;
            {
                for (int n = 0; n < 75; n++)
                {
                    if ((pos + n * count) % 75 == 0)
                    {
                        pos = (pos + n * count) / 75;
                        break;
                    }
                }
            }
            pos = (pos + 3191 + count) % count;
            {
                for (int n = 0; n < 50; n++)
                {
                    if ((pos + n * count) % 50 == 0)
                    {
                        pos = (pos + n * count) / 50;
                        break;
                    }
                }
            }
            pos = (pos + 5367 + count) % count;
            pos = count - pos - 1;
            {
                for (int n = 0; n < 61; n++)
                {
                    if ((pos + n * count) % 61 == 0)
                    {
                        pos = (pos + n * count) / 61;
                        break;
                    }
                }
            }
            pos = (pos + -5180 + count) % count;
            pos = count - pos - 1;
            {
                for (int n = 0; n < 42; n++)
                {
                    if ((pos + n * count) % 42 == 0)
                    {
                        pos = (pos + n * count) / 42;
                        break;
                    }
                }
            }
            pos = (pos + -4097 + count) % count;
            {
                for (int n = 0; n < 51; n++)
                {
                    if ((pos + n * count) % 51 == 0)
                    {
                        pos = (pos + n * count) / 51;
                        break;
                    }
                }
            }
            pos = count - pos - 1;
            {
                for (int n = 0; n < 70; n++)
                {
                    if ((pos + n * count) % 70 == 0)
                    {
                        pos = (pos + n * count) / 70;
                        break;
                    }
                }
            }
            pos = (pos + -2966 + count) % count;
            {
                for (int n = 0; n < 56; n++)
                {
                    if ((pos + n * count) % 56 == 0)
                    {
                        pos = (pos + n * count) / 56;
                        break;
                    }
                }
            }
            pos = (pos + -5621 + count) % count;
            pos = count - pos - 1;
            pos = (pos + 4276 + count) % count;
            {
                for (int n = 0; n < 70; n++)
                {
                    if ((pos + n * count) % 70 == 0)
                    {
                        pos = (pos + n * count) / 70;
                        break;
                    }
                }
            }
            pos = (pos + 6867 + count) % count;
            {
                for (int n = 0; n < 56; n++)
                {
                    if ((pos + n * count) % 56 == 0)
                    {
                        pos = (pos + n * count) / 56;
                        break;
                    }
                }
            }
            pos = count - pos - 1;
            {
                for (int n = 0; n < 29; n++)
                {
                    if ((pos + n * count) % 29 == 0)
                    {
                        pos = (pos + n * count) / 29;
                        break;
                    }
                }
            }
            pos = (pos + -532 + count) % count;
            {
                for (int n = 0; n < 33; n++)
                {
                    if ((pos + n * count) % 33 == 0)
                    {
                        pos = (pos + n * count) / 33;
                        break;
                    }
                }
            }
            pos = count - pos - 1;
            pos = (pos + -6767 + count) % count;
            pos = count - pos - 1;
            {
                for (int n = 0; n < 4; n++)
                {
                    if ((pos + n * count) % 4 == 0)
                    {
                        pos = (pos + n * count) / 4;
                        break;
                    }
                }
            }
            pos = (pos + -8527 + count) % count;
            {
                for (int n = 0; n < 46; n++)
                {
                    if ((pos + n * count) % 46 == 0)
                    {
                        pos = (pos + n * count) / 46;
                        break;
                    }
                }
            }
            pos = (pos + 8999 + count) % count;
            {
                for (int n = 0; n < 43; n++)
                {
                    if ((pos + n * count) % 43 == 0)
                    {
                        pos = (pos + n * count) / 43;
                        break;
                    }
                }
            }
            pos = (pos + 5801 + count) % count;
            {
                for (int n = 0; n < 17; n++)
                {
                    if ((pos + n * count) % 17 == 0)
                    {
                        pos = (pos + n * count) / 17;
                        break;
                    }
                }
            }
            pos = (pos + 4776 + count) % count;
            pos = count - pos - 1;
            pos = (pos + 294 + count) % count;
            {
                for (int n = 0; n < 49; n++)
                {
                    if ((pos + n * count) % 49 == 0)
                    {
                        pos = (pos + n * count) / 49;
                        break;
                    }
                }
            }
            pos = (pos + -2409 + count) % count;
            {
                for (int n = 0; n < 13; n++)
                {
                    if ((pos + n * count) % 13 == 0)
                    {
                        pos = (pos + n * count) / 13;
                        break;
                    }
                }
            }
            pos = count - pos - 1;
            pos = (pos + 2329 + count) % count;
            {
                for (int n = 0; n < 56; n++)
                {
                    if ((pos + n * count) % 56 == 0)
                    {
                        pos = (pos + n * count) / 56;
                        break;
                    }
                }
            }
            pos = (pos + 3600 + count) % count;
            {
                for (int n = 0; n < 21; n++)
                {
                    if ((pos + n * count) % 21 == 0)
                    {
                        pos = (pos + n * count) / 21;
                        break;
                    }
                }
            }
            pos = (pos + -6026 + count) % count;
            {
                for (int n = 0; n < 45; n++)
                {
                    if ((pos + n * count) % 45 == 0)
                    {
                        pos = (pos + n * count) / 45;
                        break;
                    }
                }
            }
            pos = count - pos - 1;
            {
                for (int n = 0; n < 40; n++)
                {
                    if ((pos + n * count) % 40 == 0)
                    {
                        pos = (pos + n * count) / 40;
                        break;
                    }
                }
            }
            pos = (pos + -5258 + count) % count;
            pos = count - pos - 1;
            pos = (pos + 3842 + count) % count;
            {
                for (int n = 0; n < 14; n++)
                {
                    if ((pos + n * count) % 14 == 0)
                    {
                        pos = (pos + n * count) / 14;
                        break;
                    }
                }
            }
            pos = (pos + -7380 + count) % count;
            pos = count - pos - 1;
            {
                for (int n = 0; n < 62; n++)
                {
                    if ((pos + n * count) % 62 == 0)
                    {
                        pos = (pos + n * count) / 62;
                        break;
                    }
                }
            }
            pos = (pos + -7187 + count) % count;
            {
                for (int n = 0; n < 17; n++)
                {
                    if ((pos + n * count) % 17 == 0)
                    {
                        pos = (pos + n * count) / 17;
                        break;
                    }
                }
            }
            pos = (pos + 239 + count) % count;
            pos = count - pos - 1;
            pos = (pos + -864 + count) % count;
            {
                for (int n = 0; n < 71; n++)
                {
                    if ((pos + n * count) % 71 == 0)
                    {
                        pos = (pos + n * count) / 71;
                        break;
                    }
                }
            }
            pos = (pos + 7894 + count) % count;
            {
                for (int n = 0; n < 52; n++)
                {
                    if ((pos + n * count) % 52 == 0)
                    {
                        pos = (pos + n * count) / 52;
                        break;
                    }
                }
            }
            pos = (pos + -1086 + count) % count;
            {
                for (int n = 0; n < 63; n++)
                {
                    if ((pos + n * count) % 63 == 0)
                    {
                        pos = (pos + n * count) / 63;
                        break;
                    }
                }
            }
            pos = (pos + -5590 + count) % count;
            {
                for (int n = 0; n < 67; n++)
                {
                    if ((pos + n * count) % 67 == 0)
                    {
                        pos = (pos + n * count) / 67;
                        break;
                    }
                }
            }
            pos = count - pos - 1;
            {
                for (int n = 0; n < 13; n++)
                {
                    if ((pos + n * count) % 13 == 0)
                    {
                        pos = (pos + n * count) / 13;
                        break;
                    }
                }
            }
            pos = count - pos - 1;
            {
                for (int n = 0; n < 62; n++)
                {
                    if ((pos + n * count) % 62 == 0)
                    {
                        pos = (pos + n * count) / 62;
                        break;
                    }
                }
            }
            pos = (pos + 1984 + count) % count;
            pos = count - pos - 1;
            pos = (pos + 1712 + count) % count;
            pos = count - pos - 1;
            {
                for (int n = 0; n < 34; n++)
                {
                    if ((pos + n * count) % 34 == 0)
                    {
                        pos = (pos + n * count) / 34;
                        break;
                    }
                }
            }
            return pos;
        }

        static void Main21(string[] args)
        {
            //
            // bool Jump(bool a, bool b, bool c, bool d)
            // {
            //     var t = !a;
            //     t = !t;
            //     t = b && t;
            //     t = c && t;
            //     t = !t;
            //     t = d && t;
            //     t = !t;
            //     var j = !t;
            //     return j;
            // }
            //
            // Console.Out.WriteLine(Jump(true, false, true, true));
            //
            // return;


            var line = File.ReadAllText("/Users/spaceorc/Downloads/input.txt");

            var program = line.Split(',').Select(long.Parse).ToArray();

            while (true)
            {
                var computer = new Computer("prog", program)
                {
                    Output = value => Console.Out.Write((char) (int) value)
                };

                computer.Input.OnWait += () =>
                {
                    Console.Write(">> Edit prog.txt and press `Enter`");
                    Console.ReadLine();
                    var input = File.ReadAllText(FileHelper.PatchFilename("prog.txt"));
                    input = input.Replace("\r", "").Trim().Split('#')[0].Trim() + "\nRUN\n";
                    
                    computer.Input.Send(input.Select(x => (long)x).ToArray());
                };

                var task = computer.Run();
                if (task.IsCompleted)
                    task.Wait();
                else
                    throw new Exception("Didn't complete!");
                Console.Out.WriteLine(computer.Outputs.Last());
            }

            //
            // var res = 0;
            // foreach (var v in tiles.Keys)
            // {
            //     tiles.TryGetValue(v, out var o);
            //     tiles.TryGetValue(v + new V(1, 0), out var o1);
            //     tiles.TryGetValue(v + new V(-1, 0), out var o2);
            //     tiles.TryGetValue(v + new V(0, 1), out var o3);
            //     tiles.TryGetValue(v + new V(0, -1), out var o4);
            //     if (o == '#' && o1 == '#' && o2 == '#' && o3 == '#' && o4 == '#')
            //     {
            //         res += v.X * v.Y;
            //         Console.Out.WriteLine($"{v} {v.X * v.Y} {res}");
            //     }
            // }
            //
            // Console.Out.WriteLine(res);
        }

        static void Main20(string[] args)
        {
            var lines = File.ReadAllLines("/Users/spaceorc/Downloads/input.txt");

//             var lines = @"
//              Z L X W       C                 
//              Z P Q B       K                 
//   ###########.#.#.#.#######.###############  
//   #...#.......#.#.......#.#.......#.#.#...#  
//   ###.#.#.#.#.#.#.#.###.#.#.#######.#.#.###  
//   #.#...#.#.#...#.#.#...#...#...#.#.......#  
//   #.###.#######.###.###.#.###.###.#.#######  
//   #...#.......#.#...#...#.............#...#  
//   #.#########.#######.#.#######.#######.###  
//   #...#.#    F       R I       Z    #.#.#.#  
//   #.###.#    D       E C       H    #.#.#.#  
//   #.#...#                           #...#.#  
//   #.###.#                           #.###.#  
//   #.#....OA                       WB..#.#..ZH
//   #.###.#                           #.#.#.#  
// CJ......#                           #.....#  
//   #######                           #######  
//   #.#....CK                         #......IC
//   #.###.#                           #.###.#  
//   #.....#                           #...#.#  
//   ###.###                           #.#.#.#  
// XF....#.#                         RF..#.#.#  
//   #####.#                           #######  
//   #......CJ                       NM..#...#  
//   ###.#.#                           #.###.#  
// RE....#.#                           #......RF
//   ###.###        X   X       L      #.#.#.#  
//   #.....#        F   Q       P      #.#.#.#  
//   ###.###########.###.#######.#########.###  
//   #.....#...#.....#.......#...#.....#.#...#  
//   #####.#.###.#######.#######.###.###.#.#.#  
//   #.......#.......#.#.#.#.#...#...#...#.#.#  
//   #####.###.#####.#.#.#.#.###.###.#.###.###  
//   #.......#.....#.#...#...............#...#  
//   #############.#.#.###.###################  
//                A O F   N                     
//                A A D   M                     
// ".Split('\n', StringSplitOptions.RemoveEmptyEntries).ToArray();


            var tiles = new Dictionary<V, char>();
            for (int y = 0; y < lines.Length; y++)
            for (int x = 0; x < lines[0].Length; x++)
            {
                tiles[new V(x, y)] = lines[y][x];
            }

            var portals = new Dictionary<string, List<V>>();
            
            List<V> Portal(string s)
            {
                if (!portals.TryGetValue(s, out var p))
                    portals.Add(s, p = new List<V>());
                return p;
            }

            foreach (var tile in tiles)
            {
                if (!char.IsLetter(tile.Value))
                    continue;

                if (tiles.TryGetValue(tile.Key + new V(1, 0), out var next) && char.IsLetter(next))
                {
                    if (tiles.TryGetValue(tile.Key + new V(2, 0), out var next2) && next2 == '.')
                    {
                        Portal($"{tile.Value}{next}").Add(tile.Key + new V(2, 0));
                    }
                    else if (tiles.TryGetValue(tile.Key + new V(-1, 0), out next2) && next2 == '.')
                    {
                        Portal($"{tile.Value}{next}").Add(tile.Key + new V(-1, 0));
                    }
                }
                else if (tiles.TryGetValue(tile.Key + new V(0, 1), out next) && char.IsLetter(next))
                {
                    if (tiles.TryGetValue(tile.Key + new V(0, 2), out var next2) && next2 == '.')
                    {
                        Portal($"{tile.Value}{next}").Add(tile.Key + new V(0, 2));
                    }
                    else if (tiles.TryGetValue(tile.Key + new V(0, -1), out next2) && next2 == '.')
                    {
                        Portal($"{tile.Value}{next}").Add(tile.Key + new V(0, -1));
                    }
                }
            }

            var maxX = portals.SelectMany(x => x.Value).Max(x => x.X);
            var minX = portals.SelectMany(x => x.Value).Min(x => x.X);
            var minY = portals.SelectMany(x => x.Value).Max(x => x.Y);
            var maxY = portals.SelectMany(x => x.Value).Min(x => x.Y);

            bool IsOuter(V v)
            {
                return v.X == minX || v.X == maxX || v.Y == minY || v.Y == maxY;
            }

            var portalShifts = new Dictionary<V, (V next, int shift)>();
            foreach (var portal in portals)
            {
                if (portal.Value.Count == 2)
                {
                    portalShifts[portal.Value[0]] = (portal.Value[1], IsOuter(portal.Value[0]) ? -1 : 1);
                    portalShifts[portal.Value[1]] = (portal.Value[0], IsOuter(portal.Value[1]) ? -1 : 1);
                }
            }

            var enter = (portals["AA"].Single(), 0);
            var exit = (portals["ZZ"].Single(), 0);

            var queue = new Queue<(V pos, int level)>();
            var used = new Dictionary<(V pos, int level), int>();
            used.Add(enter, 0);
            queue.Enqueue(enter);
            var difs = new[] {new V(0, -1), new V(0, 1), new V(-1, 0), new V(1, 0)};
            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();
                if (cur == exit)
                {
                    Console.Out.WriteLine(used[cur]);
                    return;
                }

                foreach (var dif in difs)
                {
                    (V pos, int level) next = (cur.pos + dif, cur.level);
                    if (tiles.TryGetValue(next.pos, out var n))
                    {
                        if (n != '.')
                            continue;
                        if (used.ContainsKey(next))
                            continue;
                        used.Add(next, used[cur] + 1);
                        queue.Enqueue(next);
                    }
                }

                if (portalShifts.TryGetValue(cur.pos, out var portalShift))
                {
                    (V pos, int level) portalNext = (portalShift.next, cur.level + portalShift.shift);
                    if (portalNext.level < 0)
                        continue;
                    if (used.ContainsKey(portalNext))
                        continue;
                    used.Add(portalNext, used[cur] + 1);
                    queue.Enqueue(portalNext);
                }
            }


        }

        static void Main19(string[] args)
        {
            var line = File.ReadAllText("/Users/spaceorc/Downloads/input.txt");

            var program = line.Split(',').Select(long.Parse).ToArray();
            //
            //
            // for (int x = 0; x < 10000; x++)
            // {
            //     var computer = new Computer("prog", program);
            //     computer.Input.Send(x);
            //     computer.Input.Send(5000);
            //     computer.Output = v =>
            //     {
            //         if (v != 0)
            //             Console.Out.WriteLine(x);
            //     };
            //     computer.Run().Wait();
            // }
            //
            // return;

            var min = 0;
            var max = 10000;

            V result = default;
            
            while (min < max - 1)
            {
                var mid = (min + max) / 2;
                var res = Test(mid); 
                if (res == null)
                    min = mid;
                else
                {
                    max = mid;
                    result = res.Value;
                }
            }

            Console.Out.WriteLine(result);
            Console.Out.WriteLine(result.X * 10000 + result.Y);

            V? Test(int firstRow)
            {
                var min = firstRow * 3700 / 5000;
                var max = 10100;

                if (!IsSet(new V(min, firstRow)))
                    throw new Exception($"WTF??? min {firstRow}");
                if (IsSet(new V(max, firstRow)))
                    throw new Exception($"WTF??? max {firstRow}");

                while (min < max - 1)
                {
                    var mid = (min + max) / 2;
                    if (IsSet(new V(mid, firstRow)))
                        min = mid;
                    else
                        max = mid;
                }

                if (IsSet(new V(min - 99, firstRow + 99)))
                    return new V(min - 99, firstRow);

                return null;
            }

            bool IsSet(V target)
            {
                var computer = new Computer("prog", program);
                var result = false;
                computer.Input.Send(target.X);
                computer.Input.Send(target.Y);
                computer.Output = v => result = v == 1;
                computer.Run().Wait();
                return result;
            }

            
        }

        static void Main18(string[] args)
        {
            var lines = File.ReadAllLines("/Users/spaceorc/Downloads/input.txt");

//             var lines = @"
// #############
// #g#f.D#..h#l#
// #F###e#E###.#
// #dCba@#@BcIJ#
// #############
// #nK.L@#@G...#
// #M###N#H###.#
// #o#m..#i#jk.#
// #############
// ".Trim().Split('\n').Select(x => x.Trim()).ToArray();

            var points = new Dictionary<char, V>();
            var starts = 0;
            for (int y = 0; y < lines.Length; y++)
            for (int x = 0; x < lines[y].Length; x++)
            {
                var v = new V(x, y);
                if (lines[y][x] == '@')
                {
                    points[(char) ('0' + starts++)] = v;
                }
                else if (lines[y][x] != '#' && lines[y][x] != '.')
                    points[lines[y][x]] = v;
            }

            var sw = Stopwatch.StartNew();
            var graph = BuildGraph();

            var queue = new Heap();
            var init = (ulong) (((1 << 26) - 1) & ~((1 << (graph.Keys.Where(k => k < 30).Max() - 3)) - 1));
            init |= 1ul << 32;
            init |= 2ul << (32 + 6);
            init |= 3ul << (32 + 6 + 6);

            queue.Add(init);
            var used = new Dictionary<ulong, int>();
            used.Add(init, 0);

            while (queue.Count > 0)
            {
                var curQ = queue.DeleteMin();
                var cur = curQ & (1ul << (32 + 18)) - 1;
                var clen = used[cur];
                if (clen != (int) (curQ >> (32 + 18)))
                    continue;

                if ((cur & ((1 << 26) - 1)) == (1 << 26) - 1)
                {
                    Console.Out.WriteLine(sw.Elapsed);
                    Console.Out.WriteLine(clen);
                    break;
                }

                var cp0 = (cur >> 26) & 0b111111;
                var others = graph[(int) cp0];
                foreach (var other in others)
                {
                    if (other.next >= 30)
                    {
                        if ((cur & (1ul << (other.next - 30))) == 0)
                            continue;
                    }

                    var next = (ulong) other.next << 26
                               | cur & 0b111111_111111_111111_000000_11111111111111111111111111;

                    if (other.next < 30)
                        next |= 1ul << (other.next - 4);

                    var nlen = clen + other.len;
                    if (used.TryGetValue(next, out var plen) && plen <= nlen)
                        continue;

                    if (nlen >= 16384)
                        throw new Exception($"WTF {nlen}");

                    used[next] = nlen;
                    var value = (ulong) nlen << (32 + 18) | next;
                    queue.Add(value);
                }

                var cp1 = (cur >> 26 + 6) & 0b111111;
                others = graph[(int) cp1];
                foreach (var other in others)
                {
                    if (other.next >= 30)
                    {
                        if ((cur & (1ul << (other.next - 30))) == 0)
                            continue;
                    }

                    var next = (ulong) other.next << (26 + 6)
                               | cur & 0b111111_111111_000000_111111_11111111111111111111111111;

                    if (other.next < 30)
                        next |= 1ul << (other.next - 4);

                    var nlen = clen + other.len;
                    if (used.TryGetValue(next, out var plen) && plen <= nlen)
                        continue;

                    if (nlen >= 16384)
                        throw new Exception($"WTF {nlen}");

                    used[next] = nlen;
                    var value = (ulong) nlen << (32 + 18) | next;
                    queue.Add(value);
                }

                var cp2 = (cur >> 26 + 6 + 6) & 0b111111;
                others = graph[(int) cp2];
                foreach (var other in others)
                {
                    if (other.next >= 30)
                    {
                        if ((cur & (1ul << (other.next - 30))) == 0)
                            continue;
                    }

                    var next = (ulong) other.next << (26 + 6 + 6)
                               | cur & 0b111111_000000_111111_111111_11111111111111111111111111;

                    if (other.next < 30)
                        next |= 1ul << (other.next - 4);

                    var nlen = clen + other.len;
                    if (used.TryGetValue(next, out var plen) && plen <= nlen)
                        continue;

                    if (nlen >= 16384)
                        throw new Exception($"WTF {nlen}");

                    used[next] = nlen;
                    var value = (ulong) nlen << (32 + 18) | next;
                    queue.Add(value);
                }

                var cp3 = (cur >> 26 + 6 + 6 + 6) & 0b111111;
                others = graph[(int) cp3];
                foreach (var other in others)
                {
                    if (other.next >= 30)
                    {
                        if ((cur & (1ul << (other.next - 30))) == 0)
                            continue;
                    }

                    var next = (ulong) other.next << (26 + 6 + 6 + 6)
                               | cur & 0b000000_111111_111111_111111_11111111111111111111111111;

                    if (other.next < 30)
                        next |= 1ul << (other.next - 4);

                    var nlen = clen + other.len;
                    if (used.TryGetValue(next, out var plen) && plen <= nlen)
                        continue;

                    if (nlen >= 16384)
                        throw new Exception($"WTF {nlen}");

                    used[next] = nlen;
                    var value = (ulong) nlen << (32 + 18) | next;
                    queue.Add(value);
                }
            }

            Dictionary<int, List<(int next, int len)>> BuildGraph()
            {
                var positions = new[] {'0', '1', '2', '3'}
                    .Concat(Enumerable.Range(0, 26).Select(x => (char) ('a' + x)))
                    .Concat(Enumerable.Range(0, 26).Select(x => (char) ('A' + x)))
                    .Select((x, i) => new {x, i})
                    .ToDictionary(x => x.x, x => x.i);

                var graph = new Dictionary<int, List<(int next, int len)>>();
                var difs = new[] {new V(0, -1), new V(0, 1), new V(-1, 0), new V(1, 0)};
                foreach (var p in points)
                {
                    var pp = positions[p.Key];
                    var edges = new List<(int other, int len)>();
                    graph.Add(pp, edges);
                    var queue = new Queue<V>();
                    queue.Enqueue(p.Value);
                    var used = new Dictionary<V, int> {{p.Value, 0}};
                    while (queue.Count > 0)
                    {
                        var cur = queue.Dequeue();

                        foreach (var dif in difs)
                        {
                            var next = cur + dif;
                            if (next.X < 0 || next.X >= lines[0].Length
                                           || next.Y < 0 || next.Y >= lines.Length)
                                continue;

                            var np = lines[next.Y][next.X];
                            if (np == '#')
                                continue;

                            if (used.ContainsKey(next))
                                continue;

                            var nlen = used[cur] + 1;
                            used.Add(next, nlen);

                            if (np != '.' && np != '@')
                                edges.Add((positions[np], nlen));
                            else
                                queue.Enqueue(next);
                        }
                    }
                }

                return graph;
            }
        }

        static void Main17(string[] args)
        {
            var line = File.ReadAllText("/Users/spaceorc/Downloads/input.txt");

            var program = line.Split(',').Select(long.Parse).ToArray();
            program[0] = 2;
            var computer = new Computer("prog", program);
            //
            // var tiles = new Dictionary<V, char>();
            // tiles[default] = 'D';

            //WriteState();

            //Console.ReadLine();

            //V pos = default; 
            computer.Output = value =>
            {
                // if (value == 10)
                //     pos += new V(-pos.X, 1);
                // else
                // {
                //     tiles[pos] = (char) (int) value;
                //     pos += new V(1, 0);
                // }
                Console.Out.Write((char) (int) value);
            };

            computer.Input.OnWait += () =>
            {
                Console.Write("> ");
                var inp = (Console.ReadLine() + '\n').Select(x => (long) x).ToArray();
                computer.Input.Send(inp);
            };

            var task = computer.Run();
            if (task.IsCompleted)
                task.Wait();
            else
                throw new Exception("Didn't complete!");

            Console.Out.WriteLine(computer.Outputs.Last());

            //
            // var res = 0;
            // foreach (var v in tiles.Keys)
            // {
            //     tiles.TryGetValue(v, out var o);
            //     tiles.TryGetValue(v + new V(1, 0), out var o1);
            //     tiles.TryGetValue(v + new V(-1, 0), out var o2);
            //     tiles.TryGetValue(v + new V(0, 1), out var o3);
            //     tiles.TryGetValue(v + new V(0, -1), out var o4);
            //     if (o == '#' && o1 == '#' && o2 == '#' && o3 == '#' && o4 == '#')
            //     {
            //         res += v.X * v.Y;
            //         Console.Out.WriteLine($"{v} {v.X * v.Y} {res}");
            //     }
            // }
            //
            // Console.Out.WriteLine(res);
        }

        static void Main16(string[] args)
        {
            var line = File.ReadAllText("/Users/spaceorc/Downloads/input.txt").Trim();
            //var line = "80871224585914546619083218645595";

            var input = line.Select(x => (long) (x - '0')).ToArray();
            var skip = long.Parse(string.Join("", input.Take(7)));
            var count = input.Length * 10_000 - skip;

            var column = new long[101];
            var nextColumn = new long[column.Length];
            var last = new long[8];

            var index = input.Length - 1;
            for (int i = 0; i < count; i++)
            {
                nextColumn[0] = input[index];
                for (int k = 1; k < column.Length; k++)
                    nextColumn[k] = nextColumn[k - 1] % 10 + column[k];

                var tmp = column;
                column = nextColumn;
                nextColumn = tmp;

                var lastIndex = count - i - 1;
                if (lastIndex < last.Length)
                    last[lastIndex] = column.Last();

                index--;
                if (index < 0)
                    index = input.Length - 1;
            }

            Console.Out.WriteLine(string.Join("", last.Select(x => Math.Abs(x % 10))));

            // int[] Pattern(int len, int num)
            // {
            //     var result = new int[len];
            //     var pattern = new[] {0, 1, 0, -1};
            //     var cur = 0;
            //     var count = 1;
            //     for (int i = 0; i < len; i++)
            //     {
            //         if (count >= num)
            //         {
            //             count = 0;
            //             cur++;
            //             if (cur == pattern.Length)
            //                 cur = 0;
            //         }
            //         count++;
            //         result[i] = pattern[cur];
            //     }
            //
            //     return result;
            // }
        }

        static void Main15(string[] args)
        {
            var line = File.ReadAllText("/Users/spaceorc/Downloads/input.txt");

            var program = line.Split(',').Select(long.Parse).ToArray();
            var computer = new Computer("prog", program);

            var tiles = new Dictionary<V, char>();
            tiles[default] = 'D';

            V pos = default;
            V nextPos = default;

            WriteState();

            Console.ReadLine();

            computer.Output = value =>
            {
                switch (value)
                {
                    case 0:
                        tiles[nextPos] = '#';
                        break;
                    case 1:
                        tiles[pos] = pos == default ? 'S' : tiles[pos] == 'W' ? 'W' : '.';
                        tiles[nextPos] = 'D';
                        pos = nextPos;
                        break;
                    case 2:
                        tiles[pos] = pos == default ? 'S' : tiles[pos] == 'W' ? 'W' : '.';
                        tiles[nextPos] = 'W';
                        pos = nextPos;
                        break;
                    default:
                        throw new Exception($"WTF: {value}");
                }
            };
            computer.Input.OnWait += () =>
            {
                WriteState();
                //Console.ReadLine();
                Thread.Sleep(20);

                var difs = new[] {default, new V(0, -1), new V(0, 1), new V(-1, 0), new V(1, 0)};
                for (int cmd = 1; cmd <= 4; cmd++)
                {
                    nextPos = pos + difs[cmd];
                    if (!tiles.TryGetValue(nextPos, out _))
                    {
                        computer.Input.Send(cmd);
                        return;
                    }
                }

                var queue = new Queue<V>();
                queue.Enqueue(pos);
                var used = new Dictionary<V, V> {{pos, pos}};

                while (queue.Count > 0)
                {
                    var cur = queue.Dequeue();
                    if (!tiles.TryGetValue(cur, out _))
                    {
                        while (true)
                        {
                            var prev = used[cur];
                            if (prev == pos)
                            {
                                var cmd = Array.IndexOf(difs, cur - pos);
                                nextPos = cur;
                                computer.Input.Send(cmd);
                                return;
                            }

                            cur = prev;
                        }
                    }

                    for (int cmd = 1; cmd <= 4; cmd++)
                    {
                        var next = cur + difs[cmd];
                        if (used.ContainsKey(next))
                            continue;
                        if (tiles.TryGetValue(next, out var nc))
                        {
                            if (nc == '#')
                                continue;
                        }

                        used.Add(next, cur);
                        queue.Enqueue(next);
                    }
                }

                throw new Exception("WTF! Couldn't find path");


                // Console.WriteLine("Waiting command...");
                // while (true)
                // {
                //     while (!Console.KeyAvailable)
                //     {
                //         Thread.Sleep(10);
                //     }
                //
                //     var key = Console.ReadKey(true);
                //
                //     if (key.Key == ConsoleKey.LeftArrow)
                //     {
                //         computer.Input.Send(3);
                //         nextPos = pos + new V(-1, 0);
                //     }
                //     else if (key.Key == ConsoleKey.RightArrow)
                //     {
                //         computer.Input.Send(4);
                //         nextPos = pos + new V(1, 0);
                //     }
                //     else if (key.Key == ConsoleKey.UpArrow)
                //     {
                //         computer.Input.Send(1);
                //         nextPos = pos + new V(0, -1);
                //     }
                //     else if (key.Key == ConsoleKey.DownArrow)
                //     {
                //         computer.Input.Send(2);
                //         nextPos = pos + new V(0, 1);
                //     }
                //     else
                //         continue;
                //     break;
                // }
            };

            try
            {
                computer.Run().Wait();
            }
            catch (Exception e)
            {
            }

            WriteState();

            var target = tiles.Single(kvp => kvp.Value == 'W').Key;
            {
                var difs = new[] {default(V), new V(0, -1), new V(0, 1), new V(-1, 0), new V(1, 0)};
                var queue = new Queue<V>();
                queue.Enqueue(target);
                var used = new Dictionary<V, int> {{target, 0}};

                while (queue.Count > 0)
                {
                    var cur = queue.Dequeue();
                    for (int cmd = 1; cmd <= 4; cmd++)
                    {
                        var next = cur + difs[cmd];
                        if (used.ContainsKey(next))
                            continue;
                        if (tiles[next] == '#')
                            continue;

                        used.Add(next, used[cur] + 1);
                        queue.Enqueue(next);
                    }
                }

                Console.Out.WriteLine($"max = {used.Values.Max()}");
            }


            void WriteState()
            {
                Console.Clear();
                Console.WriteLine("------------------");
                var minx = tiles.Keys.Min(x => x.X);
                var miny = tiles.Keys.Min(x => x.Y);
                var maxx = tiles.Keys.Max(x => x.X);
                var maxy = tiles.Keys.Max(x => x.Y);

                for (int y = miny; y <= maxy; y++)
                {
                    for (int x = minx; x <= maxx; x++)
                    {
                        if (!tiles.TryGetValue(new V(x, y), out var ch))
                            Console.Write(' ');
                        else
                            Console.Write(ch);
                    }

                    Console.WriteLine();
                }
            }
        }

        static void Main14(string[] args)
        {
            var lines = File.ReadAllLines("/Users/spaceorc/Downloads/input.txt");

//             var lines = @"
// 157 ORE => 5 NZVS
// 165 ORE => 6 DCFZ
// 44 XJWVT, 5 KHKGT, 1 QDVJ, 29 NZVS, 9 GPVTF, 48 HKGWZ => 1 FUEL
// 12 HKGWZ, 1 GPVTF, 8 PSHF => 9 QDVJ
// 179 ORE => 7 PSHF
// 177 ORE => 5 HKGWZ
// 7 DCFZ, 7 PSHF => 2 XJWVT
// 165 ORE => 2 GPVTF
// 3 DCFZ, 7 NZVS, 5 HKGWZ, 10 PSHF => 8 KHKGT
// ".Trim().Split('\n').Select(x => x.Trim()).ToArray();

            var reactions = new Dictionary<string, (long count, List<(string source, long count)> sources)>();

            foreach (var line in lines)
            {
                var split = line.Split(new[] {' ', ',', '=', '>'}, StringSplitOptions.RemoveEmptyEntries);
                var target = split[split.Length - 1];
                var targetCount = int.Parse(split[split.Length - 2]);
                var sources = new List<(string source, long count)>();
                for (int i = 0; i < split.Length - 2; i += 2)
                    sources.Add((split[i + 1], int.Parse(split[i])));

                reactions.Add(target, (targetCount, sources));
            }

            long fuel = 1;
            while (true)
            {
                var ore = Ore(fuel);
                if (ore > 1000000000000)
                    break;
                fuel *= 2;
            }

            var min = fuel / 2;
            var max = fuel;

            while (min < max - 1)
            {
                var mid = (min + max) / 2;
                var ore = Ore(mid);
                if (ore > 1000000000000)
                    max = mid;
                else
                    min = mid;
            }

            Console.Out.WriteLine($"{min} <= {Ore(min)}");

            long Ore(long fuel)
            {
                long ore;
                var prods = reactions.Keys.ToDictionary(x => x, x => 0L);
                prods["ORE"] = 0;
                var useds = reactions.Keys.ToDictionary(x => x, x => 0L);
                useds["ORE"] = 0;
                var queue = new Queue<(string req, long count)>();
                queue.Enqueue(("FUEL", fuel));
                while (queue.Any())
                {
                    var cur = queue.Dequeue();
                    var prod = prods[cur.req];
                    var used = useds[cur.req];
                    if (cur.count <= prod - used)
                    {
                        useds[cur.req] += cur.count;
                        continue;
                    }

                    if (cur.req == "ORE")
                    {
                        prods[cur.req] += cur.count;
                        useds[cur.req] += cur.count;
                        continue;
                    }

                    var left = cur.count - (prod - used);
                    var reaction = reactions[cur.req];
                    var reactionsLeft = left / reaction.count + (left % reaction.count == 0 ? 0 : 1);
                    prods[cur.req] += reactionsLeft * reaction.count;
                    useds[cur.req] += cur.count;
                    foreach (var src in reaction.sources)
                    {
                        var sreq = src.count * reactionsLeft;
                        queue.Enqueue((src.source, sreq));
                    }
                }

                ore = prods["ORE"];
                return ore;
            }
        }

        static void Main13(string[] args)
        {
            var line = File.ReadAllText("/Users/spaceorc/Downloads/input.txt");

            var program = line.Split(',').Select(long.Parse).ToArray();
            program[0] = 2;

            var computer = new Computer("prog", program);

            var tiles = new Dictionary<V, int>();

            var state = 0;
            var score = 0L;
            var tx = 0;
            var ty = 0;
            V ball = default;
            V ballDiff = new V(1, 1);
            V paddle = default;
            computer.Output = value =>
            {
                switch (state)
                {
                    case 0:
                        tx = (int) value;
                        state = 1;
                        break;
                    case 1:
                        ty = (int) value;
                        state = 2;
                        break;
                    case 2:
                        if (tx == -1 && ty == 0)
                            score = value;
                        else
                        {
                            tiles[new V(tx, ty)] = (int) value;
                            if (value == 4)
                            {
                                var nextBall = new V(tx, ty);
                                if (!ball.Equals(default))
                                    ballDiff = nextBall - ball;
                                ball = nextBall;
                            }
                            else if (value == 3)
                                paddle = new V(tx, ty);
                        }

                        state = 0;
                        break;
                }
            };

            void WriteState()
            {
                Console.Clear();
                var maxx = tiles.Keys.Max(x => x.X);
                var maxy = tiles.Keys.Max(x => x.Y);

                for (int y = 0; y <= maxy; y++)
                {
                    for (int x = 0; x <= maxx; x++)
                    {
                        switch (tiles[new V(x, y)])
                        {
                            case 0:
                                Console.Write(' ');
                                break;
                            case 1:
                                Console.Write('|');
                                break;
                            case 2:
                                Console.Write('#');
                                break;
                            case 3:
                                Console.Write('-');
                                break;
                            case 4:
                                Console.Write('0');
                                break;
                        }
                    }

                    Console.WriteLine();
                }

                Console.WriteLine(score);
            }

            computer.Input.OnWait += () =>
            {
                WriteState();

                var chosedCmd = 0;
                for (int cmdd = 0; cmdd <= 2; cmdd++)
                {
                    var cmd = cmdd == 2 ? -1 : cmdd;
                    var npaddle = new V(paddle.X + cmd, paddle.Y);

                    var backup = new Dictionary<V, int>();
                    backup[paddle] = tiles[paddle];
                    backup[npaddle] = tiles[npaddle];

                    tiles[paddle] = 0;
                    tiles[npaddle] = 3;

                    var bdif = ballDiff;
                    while (true)
                    {
                        var nextBall = ball + bdif;
                        var nextBallY = ball + new V(0, bdif.Y);
                        var nextBallX = ball + new V(bdif.X, 0);
                        if (tiles[nextBallX] != 0)
                        {
                            if (tiles[nextBallX] == 2)
                            {
                                backup[nextBallX] = tiles[nextBallX];
                                tiles[nextBallX] = 0;
                            }

                            bdif = new V(-bdif.X, bdif.Y);
                            continue;
                        }

                        if (tiles[nextBallY] != 0)
                        {
                            if (tiles[nextBallY] == 2)
                            {
                                backup[nextBallY] = tiles[nextBallY];
                                tiles[nextBallY] = 0;
                            }

                            bdif = new V(bdif.X, -bdif.Y);
                            continue;
                        }

                        if (tiles[nextBall] != 0)
                        {
                            if (tiles[nextBall] == 2)
                            {
                                backup[nextBall] = tiles[nextBall];
                                tiles[nextBall] = 0;
                            }

                            bdif = new V(-bdif.X, -bdif.Y);
                            continue;
                        }

                        break;
                    }

                    foreach (var kvp in backup)
                        tiles[kvp.Key] = kvp.Value;

                    var nball = ball + bdif;
                    if (Math.Abs(nball.X - npaddle.X) <= 1)
                    {
                        chosedCmd = cmd;
                        break;
                    }
                }

                Console.Out.WriteLine($"cmd={chosedCmd}");

                // while (!Console.KeyAvailable)
                // {
                //     Thread.Sleep(10);
                // }
                // var key = Console.ReadKey();
                Thread.Sleep(20);
                computer.Input.Send(chosedCmd);


                // if (key.Key == ConsoleKey.LeftArrow)
                //     computer.Input.Send(-1);
                // else if (key.Key == ConsoleKey.RightArrow)
                //     computer.Input.Send(1);
                // else
                //     computer.Input.Send(0);
            };
            Console.ReadLine();
            computer.Run().Wait();
            WriteState();
        }

        static void Main12(string[] args)
        {
            var lines = File.ReadAllLines("/Users/spaceorc/Downloads/input.txt");
//            var lines = @"
//<x=-8, y=-10, z=0>
//<x=5, y=5, z=10>
//<x=2, y=-7, z=3>
//<x=9, y=-8, z=-3>
//
//".Trim().Split('\n').Select(x => x.Trim()).ToArray();


            var positions = lines.Select(line =>
            {
                var split = line.Split(new[] {',', '<', '>', ' ', '=', 'x', 'y', 'z'},
                    StringSplitOptions.RemoveEmptyEntries);
                return new V3(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
            }).ToArray();

            var velocities = positions.Select(_ => new V3()).ToArray();

            var x = Solve(p => p.X);
            var y = Solve(p => p.Y);
            var z = Solve(p => p.Z);

            Console.Out.WriteLine($"{x} {y} {z} -> {Lcm(x, y, z)}");

            int Solve(Func<V3, int> getCoord)
            {
                Console.Out.WriteLine("START");
                var coords = positions.Select(getCoord).ToArray();
                var vels = velocities.Select(getCoord).ToArray();
                var ocoords = coords.ToArray();

                var sims = 0;
                while (true)
                {
                    for (int i = 0; i < vels.Length - 1; i++)
                    for (int k = i + 1; k < vels.Length; k++)
                    {
                        if (coords[i] < coords[k])
                        {
                            vels[i]++;
                            vels[k]--;
                        }
                        else if (coords[i] > coords[k])
                        {
                            vels[i]--;
                            vels[k]++;
                        }
                    }

                    var velzero = true;
                    var ceq = true;
                    for (int i = 0; i < vels.Length; i++)
                    {
                        coords[i] += vels[i];
                        if (vels[i] != 0)
                            velzero = false;
                        if (coords[i] != ocoords[i])
                            ceq = false;
                    }

                    ++sims;
                    if (velzero && ceq)
                        break;

                    if (sims % 1000 == 0)
                        Console.Out.WriteLine(sims);
                }

                return sims;
            }

//
//
//
//            for (int k = 0; k < positions.Length; k++)
//            {
//                Console.Out.WriteLine($"{positions[k]}; {velocities[k]}; {Energy()}");
//            }
//            Console.WriteLine();
//            
//            for (int i = 0; i < 1000; i++)
//            {
//                Simulate();
//                
//                for (int k = 0; k < positions.Length; k++)
//                {
//                    Console.Out.WriteLine($"{positions[k]}; {velocities[k]}; {Energy()}");
//                }
//                Console.WriteLine();
//            }
//
//            var energy = Energy();
//
//            Console.Out.WriteLine(energy);
//
//            void Simulate()
//            {
//                for (int i = 0; i < velocities.Length - 1; i++)
//                for (int k = i + 1; k < velocities.Length; k++)
//                {
//                    if (positions[i].X < positions[k].X)
//                    {
//                        velocities[i] = new V3(velocities[i].X + 1, velocities[i].Y, velocities[i].Z);
//                        velocities[k] = new V3(velocities[k].X - 1, velocities[k].Y, velocities[k].Z);
//                    }
//                    else if (positions[i].X > positions[k].X)
//                    {
//                        velocities[i] = new V3(velocities[i].X - 1, velocities[i].Y, velocities[i].Z);
//                        velocities[k] = new V3(velocities[k].X + 1, velocities[k].Y, velocities[k].Z);
//                    }
//                    if (positions[i].Y < positions[k].Y)
//                    {
//                        velocities[i] = new V3(velocities[i].X, velocities[i].Y + 1, velocities[i].Z);
//                        velocities[k] = new V3(velocities[k].X, velocities[k].Y - 1, velocities[k].Z);
//                    }
//                    else if (positions[i].Y > positions[k].Y)
//                    {
//                        velocities[i] = new V3(velocities[i].X, velocities[i].Y - 1, velocities[i].Z);
//                        velocities[k] = new V3(velocities[k].X, velocities[k].Y + 1, velocities[k].Z);
//                    }
//                    if (positions[i].Z < positions[k].Z)
//                    {
//                        velocities[i] = new V3(velocities[i].X, velocities[i].Y, velocities[i].Z + 1);
//                        velocities[k] = new V3(velocities[k].X, velocities[k].Y, velocities[k].Z - 1);
//                    }
//                    else if (positions[i].Z > positions[k].Z)
//                    {
//                        velocities[i] = new V3(velocities[i].X, velocities[i].Y, velocities[i].Z - 1);
//                        velocities[k] = new V3(velocities[k].X, velocities[k].Y, velocities[k].Z + 1);
//                    }
//                }
//
//                for (int i = 0; i < positions.Length; i++)
//                    positions[i] = positions[i] + velocities[i];
//            }
//
//            long Energy()
//            {
//                return positions.Select((p, i) => p.MLen() * velocities[i].MLen()).Sum();
//            }
        }

        static long Lcm(params long[] values)
        {
            return values.Aggregate(1L, Lcm);
        }

        static long Lcm(long a, long b)
        {
            return a / Gcd(a, b) * b;
        }

        static long Gcd(long a, long b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a + b;
        }

        static void Main11(string[] args)
        {
            var line = File.ReadAllText("/Users/spaceorc/Downloads/input.txt");

            //var line = "104,1125899906842624,99";

            var program = line.Split(',').Select(long.Parse).ToArray();
            var computer = new Computer("prog", program);

            var field = new Dictionary<V, long>
            {
                {default, 1}
            };
            V pos = default;
            var dir = 0;
            var dirs = new[] {new V(0, -1), new V(1, 0), new V(0, 1), new V(-1, 0)};

            var state = 0;
            computer.Output = value =>
            {
                if (state == 0)
                {
                    field[pos] = value;
                    state = 1;
                }
                else
                {
                    if (value == 0)
                        dir = (dir + 3) % 4;
                    else
                        dir = (dir + 1) % 4;
                    pos += dirs[dir];
                    state = 0;
                    Send();
                }
            };

            Send();

            void Send()
            {
                field.TryGetValue(pos, out var cur);
                computer.Input.Send(cur);
            }

            var task = computer.Run();
            if (!task.IsCompleted)
                throw new Exception("Task didn't terminate");

            task.Wait();

            var xmin = field.Keys.Select(x => x.X).Min();
            var xmax = field.Keys.Select(x => x.X).Max();
            var ymin = field.Keys.Select(x => x.Y).Min();
            var ymax = field.Keys.Select(x => x.Y).Max();

            for (int y = ymin; y <= ymax; y++)
            {
                for (int x = xmin; x <= xmax; x++)
                {
                    field.TryGetValue(new V(x, y), out var value);
                    Console.Write(value == 1 ? '#' : ' ');
                }

                Console.WriteLine();
            }
        }

        static void Main10(string[] args)
        {
            var lines = File.ReadAllLines("/Users/spaceorc/Downloads/input.txt");
//            var lines = @"
//.#..##.###...#######
//##.############..##.
//.#.######.########.#
//.###.#######.####.#.
//#####.##.#.##.###.##
//..#####..#.#########
//####################
//#.####....###.#.#.##
//##.#################
//#####.##.###..####..
//..######..##.#######
//####.##.####...##..#
//.#####..#.######.###
//##...#.##########...
//#.##########.#######
//.####.#.###.###.#.##
//....##.##.###..#####
//.#.#.###########.###
//#.#.#.#####.####.###
//###.##.####.##.#..##
//
//".Trim().Split('\n').Select(x => x.Trim()).ToArray();

            var asteroids = new List<V>();
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] == ' ')
                        break;
                    if (lines[y][x] == '#')
                        asteroids.Add(new V(x, y));
                }
            }

            var maxCount = int.MinValue;
            var maxTarget = new V();
            foreach (var target in asteroids)
            {
                var count = 0;
                var used = new HashSet<V> {target};
                foreach (var other in asteroids)
                {
                    if (!used.Add(other))
                        continue;

                    count++;

                    foreach (var candidate in asteroids)
                    {
                        if (used.Contains(candidate))
                            continue;

                        if (V.XProd(other - target, candidate - target) == 0
                            && V.DProd(other - target, candidate - target) > 0)
                            used.Add(candidate);
                    }
                }

                if (count > maxCount)
                {
                    maxCount = count;
                    maxTarget = target;
                }
            }

            Console.Out.WriteLine($"{maxCount} at {maxTarget}");

            var rights = asteroids
                .Where(a => a != maxTarget &&
                            ((a - maxTarget).X > 0 || (a - maxTarget).X == 0 && (a - maxTarget).Y < 0))
                .Select(x => x - maxTarget).ToArray();
            var lefts = asteroids
                .Where(a => a != maxTarget &&
                            ((a - maxTarget).X < 0 || (a - maxTarget).X == 0 && (a - maxTarget).Y > 0))
                .Select(x => x - maxTarget).ToArray();

            Array.Sort(rights, (y, x) =>
            {
                var prod = V.XProd(x, y);
                if (prod != 0)
                    return prod;
                return Comparer<int>.Default.Compare(y.MLen(), x.MLen());
            });
            Array.Sort(lefts, (y, x) =>
            {
                var prod = V.XProd(x, y);
                if (prod != 0)
                    return prod;
                return Comparer<int>.Default.Compare(y.MLen(), x.MLen());
            });

            var ordered = rights.Concat(lefts).ToArray();
            var scans = new List<List<V>>();
            while (ordered.Length > 0)
            {
                V prev = default;
                var next = new List<V>();
                var scan = new List<V>();
                scans.Add(scan);
                foreach (var v in ordered)
                {
                    if (prev != default && V.XProd(v, prev) == 0)
                        next.Add(v);
                    else
                        scan.Add(v);
                    prev = v;
                }

                ordered = next.ToArray();
            }

            ordered = scans.SelectMany(x => x).ToArray();

            for (int i = 0; i < ordered.Length; i++)
            {
                Console.Out.WriteLine($"{i + 1}: {ordered[i] + maxTarget}");
            }

            var answer = ordered[200 - 1] + maxTarget;
            Console.Out.WriteLine(answer.X * 100 + answer.Y);
        }

        static void Main9(string[] args)
        {
            var line = File.ReadAllText("/Users/spaceorc/Downloads/input.txt");

            //var line = "104,1125899906842624,99";

            var program = line.Split(',').Select(long.Parse).ToArray();
            var computer = new Computer("prog", program);

            computer.Input.Send(2);

            computer.Run().Wait();
        }

        static void Main8(string[] args)
        {
            var line = File.ReadAllText("/Users/spaceorc/Downloads/input.txt").Trim();
            var layers = new List<string>();
            var width = 25;
            var height = 6;
            for (int i = 0; i < line.Length; i += width * height)
                layers.Add(line.Substring(i, width * height));

            for (int i = 0; i < height; i++)
            {
                for (int k = 0; k < width; k++)
                {
                    var ch = layers
                        .Select(l => l[i * width + k])
                        .First(x => x != '2');
                    Console.Write(ch == '1' ? 'X' : ' ');
                }

                Console.WriteLine();
            }


//            var minc0 = int.MaxValue;
//            var minc1 = int.MaxValue;
//            var minc2 = int.MaxValue;
//            foreach (var layer in layers)
//            {
//                var c0 = layer.Count(c => c == '0');
//                var c1 = layer.Count(c => c == '1');
//                var c2 = layer.Count(c => c == '2');
//                if (c0 < minc0)
//                {
//                    minc0 = c0;
//                    minc1 = c1;
//                    minc2 = c2;
//                }
//            }
//
//            Console.Out.WriteLine(minc1 * minc2);
        }

        static void Main7(string[] args)
        {
            //var line = File.ReadAllText("/Users/spaceorc/Downloads/input.txt");
            var line =
                "3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,-5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10";

            /* phase = read() - 4
             * for (var i = 0; i < 5; ++i)
             *    write(read() * 2 + phase)
             */

            var program = line.Split(',').Select(long.Parse).ToArray();

            var max = long.MinValue;
            int[] maxPh = null;
            foreach (var phases in Phases())
            {
                var computers = new Computer[phases.Length];

                for (int i = 0; i < computers.Length; i++)
                    computers[i] = new Computer(i.ToString(), program);

                for (int i = 0; i < computers.Length; i++)
                {
                    var c = i;
                    computers[c].RedirectOutput(computers[(c + 1) % computers.Length].Input);
                }

                for (int i = 0; i < computers.Length; i++)
                    computers[i].Input.Send(phases[i]);
                computers[0].Input.Send(0);

                var tasks = new List<Task>();
                for (int i = 0; i < computers.Length; i++)
                    tasks.Add(computers[i].Run());

                if (tasks.Any(t => t.IsFaulted))
                    throw new AggregateException(tasks.Where(x => x.IsFaulted).Select(x => x.Exception));

                if (tasks.Any(t => !t.IsCompleted))
                    throw new Exception("Some tasks are not completed");

                var output = computers.Last().Outputs.Last();
                if (output > max)
                {
                    max = output;
                    maxPh = phases;
                }
            }

            Console.Out.WriteLine($"{max} {string.Join(",", maxPh)}");
        }

        private static IEnumerable<int[]> Phases()
        {
            for (int a = 0; a <= 4; a++)
            for (int b = 0; b <= 4; b++)
            {
                if (b == a)
                    continue;
                for (int c = 0; c <= 4; c++)
                {
                    if (c == a || c == b)
                        continue;
                    for (int d = 0; d <= 4; d++)
                    {
                        if (d == a || d == b || d == c)
                            continue;
                        for (int e = 0; e <= 4; e++)
                        {
                            if (e == a || e == b || e == c || e == d)
                                continue;
                            yield return new[] {a + 5, b + 5, c + 5, d + 5, e + 5};
                        }
                    }
                }
            }
        }

        static void Main6(string[] args)
        {
            var lines = File.ReadAllLines("/Users/spaceorc/Downloads/input.txt");
//            var lines = new[]
//            {
//                "COM)B",
//                "B)C",
//                "C)D",
//                "D)E",
//                "E)F",
//                "B)G",
//                "G)H",
//                "D)I",
//                "E)J",
//                "J)K",
//                "K)L",
//                "K)YOU",
//                "I)SAN"
//            };

            var tree = new Dictionary<string, List<string>>();
            var parents = new Dictionary<string, string>();
            var depths = new Dictionary<string, int>();

            foreach (var line in lines)
            {
                var split = line.Split(')');
                var parent = split[0];
                var child = split[1];
                if (!tree.TryGetValue(parent, out var children))
                    tree.Add(parent, children = new List<string>());
                children.Add(child);
                parents[child] = parent;
            }

            var queue = new Queue<(string id, int depth)>();
            queue.Enqueue(("COM", 0));
            depths["COM"] = 0;
            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();
                if (tree.TryGetValue(cur.id, out var children))
                {
                    foreach (var child in children)
                    {
                        queue.Enqueue((child, cur.depth + 1));
                        depths[child] = cur.depth + 1;
                    }
                }
            }

            var total = 0;
            var youDepth = depths["YOU"];
            var sanDepth = depths["SAN"];
            var youNode = "YOU";
            while (youDepth > sanDepth)
            {
                youNode = parents[youNode];
                total++;
                youDepth--;
            }

            var sanNode = "SAN";
            while (sanDepth > youDepth)
            {
                sanNode = parents[sanNode];
                total++;
                sanDepth--;
            }

            while (youNode != sanNode)
            {
                youNode = parents[youNode];
                sanNode = parents[sanNode];
                total += 2;
            }

            Console.Out.WriteLine(total - 2);
        }

        static void Main5(string[] args)
        {
            //var line = File.ReadAllText("/Users/spaceorc/Downloads/input.txt");
            var line =
                "3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99";
            var program = line.Split(',').Select(long.Parse).ToArray();
            var computer = new Computer("prog", program);
            computer.Run();

            while (!computer.Terminated)
            {
                var value = long.Parse(Console.ReadLine());
                computer.Input.Send(value);
            }

//            var res = Run(program);
//            if (res == long.MinValue)
//                Console.Out.WriteLine("FAILED!");
        }

        static void Main4(string[] args)
        {
            var min = 367479;
            var max = 893698;

            var count = 0;
            for (int psw = min; psw <= max; psw++)
            {
                if (Valid(psw))
                    count++;
            }

            Console.Out.WriteLine(count);

            bool Valid(int psw)
            {
                var digits = psw.ToString();
                var prev = -1;
                var repeat = 1;
                var doubles = false;
                for (int i = 0; i < digits.Length; i++)
                {
                    var digit = digits[i] - '0';
                    if (digit < prev)
                        return false;
                    if (digit == prev)
                        repeat++;
                    else
                    {
                        if (repeat == 2)
                            doubles = true;
                        repeat = 1;
                    }

                    prev = digit;
                }

                return doubles || repeat == 2;
            }
        }

        static void Main3(string[] args)
        {
            var lines = File.ReadAllLines("/Users/spaceorc/Downloads/input.txt");
//            var lines = new[]
//            {
//                "R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51",
//                "U98,R91,D20,R16,D67,R40,U7,R15,U6,R7",
//            };

            var used = new Dictionary<V, int>();

            var min = int.MaxValue;

            var cur = new V();
            var steps = 0;
            foreach (var cmd in lines[0].Split(','))
            {
                var dir = cmd[0];
                var dist = int.Parse(cmd.Substring(1));
                var dif = new V();
                switch (dir)
                {
                    case 'U':
                        dif = new V(0, -1);
                        break;
                    case 'D':
                        dif = new V(0, 1);
                        break;
                    case 'L':
                        dif = new V(-1, 0);
                        break;
                    case 'R':
                        dif = new V(1, 0);
                        break;
                }

                for (int i = 0; i < dist; i++)
                {
                    cur += dif;
                    used.TryAdd(cur, ++steps);
                }
            }

            cur = new V();
            steps = 0;
            foreach (var cmd in lines[1].Split(','))
            {
                var dir = cmd[0];
                var dist = int.Parse(cmd.Substring(1));
                var dif = new V();
                switch (dir)
                {
                    case 'U':
                        dif = new V(0, -1);
                        break;
                    case 'D':
                        dif = new V(0, 1);
                        break;
                    case 'L':
                        dif = new V(-1, 0);
                        break;
                    case 'R':
                        dif = new V(1, 0);
                        break;
                }

                for (int i = 0; i < dist; i++)
                {
                    cur += dif;
                    ++steps;
                    if (used.TryGetValue(cur, out var other))
                    {
                        if (steps + other < min)
                        {
                            min = steps + other;
                        }
                    }
                }
            }


            Console.Out.WriteLine(min);
        }

        static void Main2(string[] args)
        {
            //var line = File.ReadAllText("/Users/spaceorc/Downloads/input.txt");
            var line = "1,1,1,4,99,5,6,0,99";

            var original = line.Split(',').Select(long.Parse).ToArray();

            var expected = 1;
            //var expected = 19690720;

            for (int n = 0; n <= 99; n++)
            for (int v = 0; v <= 99; v++)
            {
                var program = original.ToArray();

                program[1] = n;
                program[2] = v;

                var computer = new Computer("prog", program);
                if (!computer.TryRun().Result)
                    continue;

                var output = computer.ProgramAt(0);
                if (output == expected)
                {
                    Console.Out.WriteLine(n * 100 + v);
                    return;
                }
            }
        }

        static void Main1(string[] args)
        {
            var lines = File.ReadAllLines("/Users/spaceorc/Downloads/input.txt");
            //var lines = new[] { "100756" };
            var total = 0L;
            foreach (var mass in lines.Select(long.Parse))
            {
                var fuel = mass;
                while (fuel > 0)
                {
                    fuel = fuel / 3 - 2;
                    if (fuel > 0)
                        total += fuel;
                }
            }

            Console.Out.WriteLine(total);
        }
    }
}