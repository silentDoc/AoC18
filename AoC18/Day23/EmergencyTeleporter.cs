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

#pragma warning disable CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL
#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo

        // Helpers to get absolute value. Z3 does not expose the method directly, so we implement Abs(x) = x<0 ? -x : x
        ArithExpr z3Abs(ArithExpr x, Context ctx)
            => ctx.MkITE(ctx.MkLt(x, ctx.MkInt(0)),
                         ctx.MkSub(ctx.MkInt(0), x),
                         x) as ArithExpr;

        ArithExpr z3AbsDif(ArithExpr x, int nx, Context ctx)
        {
            ArithExpr expr_x       = ctx.MkInt(nx);
            ArithExpr substraction = ctx.MkSub(expr_x, x);


            ArithExpr result       = ctx.MkITE(ctx.MkLt(substraction, ctx.MkInt(0)),
                                          ctx.MkSub(ctx.MkInt(0), substraction),
                                          substraction) as ArithExpr;
            
            if (result == null)
                throw new Exception("Shite");
            return result;
        }
#pragma warning restore CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo

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
                Tuple<int, int, int> nPos = new(pos.x, pos.y, pos.z);
                int range = Bots[i].Range;

                inRanges[i] = z3Ctx.MkIntConst("in_range_" + i);
                ArithExpr difX = z3AbsDif(x, nPos.Item1, z3Ctx);
                ArithExpr difY = z3AbsDif(y, nPos.Item2, z3Ctx);
                ArithExpr difZ = z3AbsDif(z, nPos.Item3, z3Ctx);

                ArithExpr dist = z3Ctx.MkAdd(difX, difY, difZ);

                // o.add(in_ranges[i] == dist <= nrng, 1, 0))
                optimizer.Add(z3Ctx.MkEq(inRanges[i],
                                         z3Ctx.MkITE(z3Ctx.MkLe(dist, z3Ctx.MkInt(range)),
                                                     z3Ctx.MkInt(1),
                                                     z3Ctx.MkInt(0)  
                                                     ) ) );
            }

            optimizer.Add(z3Ctx.MkEq(rangeCount, z3Ctx.MkAdd(inRanges)));
            ArithExpr distFromZero = z3Ctx.MkIntConst("dist");

            optimizer.Add(z3Ctx.MkEq(distFromZero, z3Ctx.MkAdd(z3Ctx.MkAdd(z3Abs(x, z3Ctx), z3Abs(y, z3Ctx)), z3Abs(z, z3Ctx))));
            optimizer.MkMaximize(rangeCount);
            var resDis = optimizer.MkMinimize(distFromZero);
            optimizer.Check();

            // Maximize the number of nanobots in range
            Console.WriteLine(resDis.Lower.ToString());

            return 0;
        }

        public int Solve(int part)
            => part == 1 ? Bots.First(x => x.Range == Bots.Max(b => b.Range)).HowManyInRange(Bots) 
            : SolvePart2();
    }
}
