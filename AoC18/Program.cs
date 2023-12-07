using System.Diagnostics;

namespace AoC18
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int day = 20;
            int part = 2;
            bool test = false;

            string input = "./Input/day" + day.ToString("00");
            input += (test) ? "_test.txt" : ".txt";

            Console.WriteLine("AoC 2018 - Day {0} , Part {1} - Test Data {2}", day, part, test);
            Stopwatch st = new();
            st.Start();
            string result = day switch
            {
                1 => day1(input, part),
                2 => day2(input, part),
                3 => day3(input, part),
                4 => day4(input, part),
                5 => day5(input, part),
                6 => day6(input, part),
                7 => day7(input, part),
                8 => day8(input, part),
                9 => day9(input, part),
                10 => day10(input, part),
                11 => day11(input, part),
                12 => day12(input, part),
                13 => day13(input, part),
                14 => day14(input, part),
                15 => day15(input, part),
                16 => day16(input, part),
                17 => day17(input, part),
                18 => day18(input, part),
                19 => day19(input, part),
                20 => day20(input, part),

                _ => throw new ArgumentException("Wrong day number - unimplemented")
            };
            st.Stop();
            Console.WriteLine("Result : {0}", result);
            Console.WriteLine("Ellapsed : {0}", st.Elapsed.TotalSeconds);
        } 

        static string day1(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day01.DeviceCalibrator dc = new();
            dc.ParseInput(lines);

            return dc.Solve(part).ToString();
        }

        static string day2(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day02.ChecksumCalculator cs = new();
            return cs.Solve(lines, part);
        }

        static string day3(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day03.PatchFabric fabric = new();
            fabric.ParseInput(lines);
            return fabric.Solve(part).ToString();
        }

        static string day4(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day04.GuardWatcher watcher = new();
            watcher.ParseInput(lines);
          
            return watcher.Solve(part).ToString();
        }

        static string day5(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day05.PolymerReductor polRed = new();
            polRed.ParseInput(lines);
            return polRed.Solve(part).ToString();
        }

        static string day6(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day06.LocationSpace space = new();
            space.ParseInput(lines);
                      
            return space.Solve(part).ToString();
        }

        static string day7(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day07.PrecedenceSolver solver = new();
            solver.ParseInput(lines);

            return solver.Solve(part);
        }

        static string day8(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day08.TreeParser treeParser = new();
            treeParser.ParseInput(lines);
            return treeParser.Solve(part).ToString();
        }

        static string day9(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day09.MarbleGame marbleGame = new();
            marbleGame.ParseInput(lines);

            return marbleGame.Solve(part).ToString();
        }

        static string day10(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day10.MessageBuilder msgBuilder = new();
            msgBuilder.ParseInput(lines);

            return msgBuilder.Solve(part);
        }

        static string day11(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day11.CellInspector inspector = new();
            inspector.ParseInput(lines);
            return inspector.Solve(part);
        }

        static string day12(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day12.FlowerPot pots = new();
            pots.ParseInput(lines);
            return pots.Solve(part).ToString();
        }

        static string day13(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day13.MineCartRunner runner = new();
            runner.ParseInput(lines);

            return runner.Solve(part);
        }

        static string day14(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day14.RecipeFinder finder = new();
            finder.ParseInput(lines);

            return finder.Solve(part);
        }

        static string day15(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            

            return "";
        }

        static string day16(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day16.OpcodeCracker cracker = new();
            cracker.ParseInput(lines);

            return cracker.Solve(part).ToString();
        }

        static string day17(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day17.WaterSpring waterVeins = new();
            waterVeins.ParseInput(lines);

            return waterVeins.Solve(part).ToString();
        }

        static string day18(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day18.LumberjackElves settlement = new();
            settlement.ParseInput(lines);
            return settlement.Solve(part).ToString();
        }

        static string day19(string input, int part) 
        {
            var lines = File.ReadAllLines(input).ToList();
            Day19.ElvenDevice device = new();
            device.ParseInput(lines);

            return device.Solve(part).ToString();
        }

        static string day20(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day20.MazeBuilder builder = new();
            builder.ParseInput(lines);
            
            return builder.Solve(part).ToString();
        }

    }
}