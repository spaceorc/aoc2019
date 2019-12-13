using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace aoc
{
    class Program
    {
        static void Main(string[] args)
        {
            var line = File.ReadAllText("/Users/spaceorc/Downloads/input.txt");

            //var line = "104,1125899906842624,99";

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
            var r = 1L;
            for (int i = 0; i < values.Length; i++)
            {
                r = Lcm(r, values[i]);
            }

            return r;
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
                    a = a % b;
                else
                    b = b % a;
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