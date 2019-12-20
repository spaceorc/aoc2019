using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aoc
{
    public class Computer
    {
        private static readonly int[] divs = {100, 1000, 10000};
        private readonly string id;
        private readonly Dictionary<long, long> program;
        private long ip;
        private long relativeBase;

        public Computer(string id, long[] program)
        {
            this.id = id;
            this.program = program.Select((x, i) => new {x, i}).ToDictionary(x => (long) x.i, x => x.x);
        }

        public bool Terminated { get; private set; }
        public List<long> Outputs { get; } = new List<long>();
        public Action<long> Output { get; set; } = Console.WriteLine;
        public Input Input { get; } = new Input();

        public long ProgramAt(long position)
        {
            program.TryGetValue(position, out var result);
            return result;
        }

        public void RedirectOutput(Input input)
        {
            Output = v => input.Send(v);
        }

        public async Task Run()
        {
            if (!await TryRun())
                throw new Exception($"{id}: FAILURE!");
        }

        public async Task<bool> TryRun()
        {
            while (true)
                switch (await Eval())
                {
                    case EvalResult.Success:
                        break;
                    case EvalResult.Halt:
                        Terminated = true;
                        return true;
                    case EvalResult.Failure:
                        Terminated = true;
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }

        private async Task<EvalResult> Eval()
        {
            program.TryGetValue(ip, out var opCode);
            var op = opCode % 100;
            if (op == 99)
                return EvalResult.Halt;

            switch (op)
            {
                case 1:
                {
                    if (!ReadArg(0, out var arg0)
                        || !ReadArg(1, out var arg1)
                        || !ReadAddress(2, out var res))
                        return EvalResult.Failure;

                    program[res] = arg0 + arg1;
                    ip += 4;
                    return EvalResult.Success;
                }

                case 2:
                {
                    if (!ReadArg(0, out var arg0)
                        || !ReadArg(1, out var arg1)
                        || !ReadAddress(2, out var res))
                        return EvalResult.Failure;

                    program[res] = arg0 * arg1;
                    ip += 4;
                    return EvalResult.Success;
                }

                case 3:
                {
                    if (!ReadAddress(0, out var res))
                        return EvalResult.Failure;

                    program[res] = await Input.Wait();
                    ip += 2;
                    return EvalResult.Success;
                }

                case 4:
                {
                    if (!ReadArg(0, out var arg0))
                        return EvalResult.Failure;

                    Write(arg0);
                    ip += 2;
                    return EvalResult.Success;
                }

                case 5:
                {
                    if (!ReadArg(0, out var arg0)
                        || !ReadArg(1, out var arg1))
                        return EvalResult.Failure;

                    if (arg0 != 0)
                    {
                        if (arg1 < 0)
                            return EvalResult.Failure;
                        ip = (int) arg1;
                    }
                    else
                    {
                        ip += 3;
                    }

                    return EvalResult.Success;
                }

                case 6:
                {
                    if (!ReadArg(0, out var arg0)
                        || !ReadArg(1, out var arg1))
                        return EvalResult.Failure;

                    if (arg0 == 0)
                    {
                        if (arg1 < 0)
                            return EvalResult.Failure;
                        ip = (int) arg1;
                    }
                    else
                    {
                        ip += 3;
                    }

                    return EvalResult.Success;
                }

                case 7:
                {
                    if (!ReadArg(0, out var arg0)
                        || !ReadArg(1, out var arg1)
                        || !ReadAddress(2, out var res))
                        return EvalResult.Failure;

                    program[res] = arg0 < arg1 ? 1 : 0;
                    ip += 4;
                    return EvalResult.Success;
                }

                case 8:
                {
                    if (!ReadArg(0, out var arg0)
                        || !ReadArg(1, out var arg1)
                        || !ReadAddress(2, out var res))
                        return EvalResult.Failure;

                    program[res] = arg0 == arg1 ? 1 : 0;
                    ip += 4;
                    return EvalResult.Success;
                }

                case 9:
                {
                    if (!ReadArg(0, out var arg0))
                        return EvalResult.Failure;

                    if (relativeBase + arg0 < 0)
                        return EvalResult.Failure;

                    relativeBase += arg0;
                    ip += 2;
                    return EvalResult.Success;
                }

                default:
                    return EvalResult.Failure;
            }

            bool ReadArg(int arg, out long value)
            {
                value = default;
                var div = divs[arg];
                var mode = opCode / div % 10;

                program.TryGetValue(ip + arg + 1, out value);
                if (mode == 1)
                    return true;

                if (mode == 2)
                    value = relativeBase + value;

                if (value < 0)
                    return false;

                program.TryGetValue(value, out value);
                return true;
            }

            bool ReadAddress(int arg, out long address)
            {
                address = default;

                var div = divs[arg];
                var mode = opCode / div % 10;
                if (mode == 1)
                    return false;

                program.TryGetValue(ip + arg + 1, out address);
                if (mode == 2)
                    address = relativeBase + address;

                if (address < 0)
                    return false;

                return true;
            }
        }

        private void Write(long value)
        {
            Outputs.Add(value);
            Output(value);
        }

        private enum EvalResult
        {
            Success,
            Halt,
            Failure
        }
    }
}