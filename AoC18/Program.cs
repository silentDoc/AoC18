using System.Diagnostics;

namespace AoC18
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int day = 10;
            int part = 1;
            bool test = false;

            string input = "./Input/day" + day.ToString("00");
            input += (test) ? "_test.txt" : ".txt";

            Console.WriteLine("AoC 2018 - Day {0} , Part {1} - Test Data {2}", day, part, test);
            Stopwatch st = new();
            st.Start();
            string result = day switch
            {
                1 => day1(input, part).ToString(),
                2 => day2(input, part).ToString(),
                3 => day3(input, part).ToString(),
                4 => day4(input, part).ToString(),
                5 => day5(input, part).ToString(),
                6 => day6(input, part).ToString(),
                7 => day7(input, part).ToString(),
                8 => day8(input, part).ToString(),
                9 => day9(input, part).ToString(),
                10 => day10(input, part).ToString(),
                11 => day11(input, part).ToString(),
                _ => throw new ArgumentException("Wrong day number - unimplemented")
            };
            st.Stop();
            Console.WriteLine("Result : {0}", result);
            Console.WriteLine("Ellapsed : {0}", st.Elapsed.TotalSeconds);
        } 

        static int day1(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day01.DeviceCalibrator dc = new();
            dc.ParseInput(lines);

            return dc.Solve(part);
        }

        static string day2(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day02.ChecksumCalculator cs = new();
            return cs.Solve(lines, part);
        }

        static int day3(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day03.PatchFabric fabric = new();
            fabric.ParseInput(lines);
            return fabric.Solve(part);
        }

        static int day4(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day04.GuardWatcher watcher = new();
            watcher.ParseInput(lines);
          
            return watcher.Solve(part);
        }

        static int day5(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day05.PolymerReductor polRed = new();
            polRed.ParseInput(lines);
            return polRed.Solve(part);
        }

        static int day6(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day06.LocationSpace space = new();
            space.ParseInput(lines);
                      
            return space.Solve(part);
        }

        static string day7(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day07.PrecedenceSolver solver = new();
            solver.ParseInput(lines);

            return solver.Solve(part);
        }

        static int day8(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day08.TreeParser treeParser = new();
            treeParser.ParseInput(lines);
            return treeParser.Solve(part);
        }

        static long day9(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day09.MarbleGame marbleGame = new();
            marbleGame.ParseInput(lines);

            return marbleGame.Solve(part);
        }

        static string day10(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            Day10.MessageBuilder msgBuilder = new();
            msgBuilder.ParseInput(lines);

            return msgBuilder.Solve(part);
        }

        static int day11(string input, int part)
        {
            var lines = File.ReadAllLines(input).ToList();
            return 0;
        }
    }
}