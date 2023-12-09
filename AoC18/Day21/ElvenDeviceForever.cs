using System.Collections.Generic;

namespace AoC18.Day21
{
    public record DeviceInstruction
    {
        public string ins;
        public int[] ops = { 0, 0, 0 };

        public override string ToString()
            => ins + " " + ops[0].ToString() + " " + ops[1].ToString() + " " + ops[2].ToString();
    }

    public  class ElvenDeviceForever
    {
        int ParsedIntPointer = -1;
        Dictionary<int, DeviceInstruction> Program = new();
        int[] registers = new int[6];

        DeviceInstruction ParseInstruction(string line)
        {
            var instruction = new DeviceInstruction();

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            instruction.ins = parts[0];
            instruction.ops[0] = int.Parse(parts[1]);
            instruction.ops[1] = int.Parse(parts[2]);
            instruction.ops[2] = int.Parse(parts[3]);

            return instruction;
        }

        public void ParseInput(List<string> input)
        {
            ParsedIntPointer = int.Parse(input[0].Replace("#ip ", ""));
            foreach (var c in Enumerable.Range(1, input.Count - 1))
                Program[c - 1] = ParseInstruction(input[c]);
        }

        int[] RunOpCode(DeviceInstruction instruction, int[] regs)
        {
            int[] resultRegs = (int[])regs.Clone();
            var instructionName = instruction.ins;

            var opA = instruction.ops[0];   // Just to reduce verbosity on the switch belo
            var opB = instruction.ops[1];
            var opC = instruction.ops[2];


            resultRegs[opC] = instructionName switch
            {
                "addr" => regs[opA] + regs[opB],
                "addi" => regs[opA] + opB,
                "mulr" => regs[opA] * regs[opB],
                "muli" => regs[opA] * opB,
                "banr" => regs[opA] & regs[opB],
                "bani" => regs[opA] & opB,
                "borr" => regs[opA] | regs[opB],
                "bori" => regs[opA] | opB,
                "setr" => regs[opA],
                "seti" => opA,
                "gtir" => opA > regs[opB] ? 1 : 0,
                "gtri" => regs[opA] > opB ? 1 : 0,
                "gtrr" => regs[opA] > regs[opB] ? 1 : 0,
                "eqir" => opA == regs[opB] ? 1 : 0,
                "eqri" => regs[opA] == opB ? 1 : 0,
                "eqrr" => regs[opA] == regs[opB] ? 1 : 0,
                _ => throw new System.Exception("Invalid operation")
            };
            return resultRegs;
        }

        int RunProgram(int part =1)
        {
            HashSet<int> states = new();
            int last = 0;

            registers = new int[] { 0, 0, 0, 0, 0, 0 };
            int ip = 0;
            long iterations = 0;

            while (ip < Program.Count && ip >= 0)
            {
                iterations++;
                if (ip == 28 && part == 1)
                    return registers[3];
                else if (ip == 28)
                {
                    if (!states.Add(registers[3]))
                        return last;
                    else
                        last = registers[3];
                }

                registers[ParsedIntPointer] = ip;
                registers = RunOpCode(Program[ip], registers);
                ip = registers[ParsedIntPointer];
                ip++;
            }
            return registers[0];
        }

        public int ReverseEngineeredPart2()
        {
            int tempValue, targetValue;
            int prev = -1;
            var seen = new HashSet<int>();

            tempValue = 65536;
            targetValue = 1505483;

            while (true)
            {
                targetValue += tempValue & 255;
                targetValue &= 16777215;
                targetValue *= 65899;
                targetValue &= 16777215;

                if (tempValue < 256)
                {
                    if (!seen.Add(targetValue))
                        return prev;
                    else
                        prev = targetValue;

                    tempValue = targetValue | 65536;
                    targetValue = 1505483;
                }
                else
                    tempValue = (int) Math.Floor((decimal) (tempValue / 256));
            }
        }

        public int Solve(int part)
            => part == 1 ? RunProgram(part) : ReverseEngineeredPart2();
    }
}