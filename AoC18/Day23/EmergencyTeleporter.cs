using AoC18.Common;
using System.Text.RegularExpressions;
using Microsoft.Z3;

namespace AoC18.Day23
{
    public class Nanobot
    {
        public Coord3D Position;
        public int Range;

        public Nanobot(string inputLine)
        { 
            Regex regex = new Regex(@"pos=<(-?\d+),(-?\d+),(-?\d+)>, r=(\d+)");
            var groups = regex.Match(inputLine).Groups;
            Position = (int.Parse(groups[1].Value), int.Parse(groups[2].Value), int.Parse(groups[3].Value));
            Range = int.Parse(groups[4].Value);
        }

        public int HowManyInRange(List<Nanobot> list)
            => list.Count(n => Position.Manhattan(n.Position) <= Range);
    }

    internal class EmergencyTeleporter
    {
        List<Nanobot> Bots = new();
        public void ParseInput(List<string> lines)
            => lines.ForEach(x => Bots.Add(new Nanobot(x)));

        // Helpers to get absolute value. Z3 does not expose the method directly, so we implement Abs(x) = x<0 ? -x : x
        ArithExpr z3Abs(ArithExpr x, Context ctx)
            => (ArithExpr) ctx.MkITE(ctx.MkLt(x, ctx.MkInt(0)),
                                     ctx.MkSub(ctx.MkInt(0), x),
                                     x);

        ArithExpr z3AbsDif(ArithExpr x, int nx, Context ctx)
        {
            ArithExpr expr_x       = ctx.MkInt(nx);
            ArithExpr substraction = ctx.MkSub(expr_x, x);

            return (ArithExpr) ctx.MkITE(ctx.MkLt(substraction, ctx.MkInt(0)),
                                                  ctx.MkSub(ctx.MkInt(0), substraction),
                                                  substraction);
        }

        private int SolvePart2()
        {
            Context z3Ctx = new Context();
            ArithExpr x = z3Ctx.MkIntConst("x");
            ArithExpr y = z3Ctx.MkIntConst("y");
            ArithExpr z = z3Ctx.MkIntConst("z");

            ArithExpr[] inRanges = new ArithExpr[Bots.Count];
            ArithExpr rangeCount = z3Ctx.MkIntConst("sum");
            Optimize optimizer = z3Ctx.MkOptimize();

            for (int i = 0; i < Bots.Count; i++)
            {
                Coord3D pos = Bots[i].Position;
                int range   = Bots[i].Range;

                inRanges[i] = z3Ctx.MkIntConst("in_range_" + i);
                ArithExpr difX = z3AbsDif(x, pos.x, z3Ctx);
                ArithExpr difY = z3AbsDif(y, pos.y, z3Ctx);
                ArithExpr difZ = z3AbsDif(z, pos.z, z3Ctx);

                ArithExpr dist = z3Ctx.MkAdd(difX, difY, difZ);

                optimizer.Add(z3Ctx.MkEq(inRanges[i],
                                         z3Ctx.MkITE(z3Ctx.MkLe(dist, z3Ctx.MkInt(range)),
                                                     z3Ctx.MkInt(1),
                                                     z3Ctx.MkInt(0)  
                                                     ) ) );
            }

            optimizer.Add(z3Ctx.MkEq(rangeCount, z3Ctx.MkAdd(inRanges)));
            ArithExpr distFromZero = z3Ctx.MkIntConst("dist");
            
            optimizer.Add( z3Ctx.MkEq(distFromZero, z3Ctx.MkAdd(z3Abs(x, z3Ctx), z3Abs(y, z3Ctx), z3Abs(z, z3Ctx))  ) );
            optimizer.MkMaximize(rangeCount);
            var resDis = optimizer.MkMinimize(distFromZero);
            optimizer.Check();

            Console.WriteLine("Part 2 Result" + resDis.Lower.ToString());
            return 0;
        }

        public int Solve(int part)
            => part == 1 ? Bots.First(x => x.Range == Bots.Max(b => b.Range)).HowManyInRange(Bots) 
            : SolvePart2();
    }
}
