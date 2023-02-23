﻿using System.Diagnostics;

namespace AoC18
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int day = 2;
            int part = 2;
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
    }
}