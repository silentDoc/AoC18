# Day 23 - Part 2 - Explanation
Since this is my first time using the Z3 Solver/Optimizer library, I want to debrief a little bit what has been done and what does each part do. So here it goes:

## The Problem

We are basically given a set of Nanobots, each one with a _position_ `<X,Y,Z>` and a _range_ `r`.

Our goal is to find the position in the space `<x,y,z>` that lies in the range of the most nanobots possible and is closer to origin ( `<0,0,0>` ). We have to **return the distance from such Nanobot to the origin** as the answer to part 2.

The problem is that we have 1000 Nanobots, and the order of magnitude of the coords and ranges is about 10^8, for instance:
`pos=<56223373,19185523,54669966>, r=97281076`

So any basic iterative approach is completely out of the question, as it will last forever. 

## The Solution (and some monologue and old man babbling)

The solution proposed is not the unique solution to the problem. At the time of solving this many different approaches are present in the internet, from quadratic search, to clique based approaches, to smart linear methods that are surprisingly efficient -- you can check the solutions [here](https://www.reddit.com/r/adventofcode/comments/a8s17l/2018_day_23_solutions/). 

In this very thread, the first solution caught my attention, a piece of Python code that was this simple:

``` python
from z3 import *

def zabs(x):
  return If(x >= 0,x,-x)

(x, y, z) = (Int('x'), Int('y'), Int('z'))

in_ranges = [
  Int('in_range_' + str(i)) for i in lenr(nanobots)
]
range_count = Int('sum')
o = Optimize()

for i in lenr(nanobots):
  (nx, ny, nz), nrng = nanobots[i]
  o.add(in_ranges[i] == If(zabs(x - nx) + zabs(y - ny) + zabs(z - nz) <= nrng, 1, 0))

o.add(range_count == sum(in_ranges))
dist_from_zero = Int('dist')
o.add(dist_from_zero == zabs(x) + zabs(y) + zabs(z))
h1 = o.maximize(range_count)
h2 = o.minimize(dist_from_zero)
print o.check()
#print o.lower(h2)
```

The solution to the second part of the puzzle basically consisted on **setting up the constraints and making the library work**. I had read about constraint based programming before and knew about some libraries for SMT - SAT Solving ([_Google OR-Tools_](https://developers.google.com/optimization/introduction/dotnet?hl=es-419) being probably the most popular these days) so I decided to give it a shot. And after checking out the Z3 library and acknowledge that is one of the most popular as well (if not the most) I went for it. 

### The Z3 Library

The Z3 Solver is a project from Microsoft research and some of the basic resources are listed below:

- [Project Page](https://www.microsoft.com/en-us/research/project/z3-3/)
- [Github](https://github.com/Z3Prover/z3)
- [DotNet examples](https://github.com/Z3Prover/z3/tree/master/examples/dotnet)
- [Nuget Package Page](https://www.nuget.org/packages/Microsoft.Z3/)
- [API Reference](https://z3prover.github.io/api/html/namespace_microsoft_1_1_z3.html)
- [Wiki](https://github.com/Z3Prover/z3/wiki#background)

I have not had luck finding a **C#** tutorial for doing simple stuff with Z3 and DotNet - it's not that I have insisted that much, but the basic search results are not leading me to any trustable source. If I have better luck I will update this document :)

### Differences with Python

Despite being a library from Microsoft, my experience is that using Z3Solver with .NET is far more tedious and less natural than it is with Python. 

Just compare the python version of this line (to add an expression to optimize to the optimizer):

``` python
o.add(dist_from_zero == zabs(x) + zabs(y) + zabs(z))
```

With the c# version:

``` csharp
optimizer.Add(z3Ctx.MkEq(distFromZero, z3Ctx.MkAdd(z3Ctx.MkAdd(z3Abs(x, z3Ctx), z3Abs(y, z3Ctx)), z3Abs(z, z3Ctx))));
```

The _implicit type casts_ are also a pain. In python, the expressions can be added on a more seamless way than they can be in DotNet (or maybe it's just me that do not know how to do it better). 

Anyhow, trying new approaches is always worth the effort, so let's jump into the code. 

## The Code

The code I provided is a direct port from the Python snippet I found. I will try to explain bit by bit what I am doing at each step. 

### Block 1 : Declarations and initialization

``` csharp
using Microsoft.Z3;

Context z3Ctx = new Context();
ArithExpr x = z3Ctx.MkIntConst("x");
ArithExpr y = z3Ctx.MkIntConst("y");
ArithExpr z = z3Ctx.MkIntConst("z");
```

The main interaction with Z3 happens via the `Context`. The context object will allow us to create expressions, variables and constraints - and of course, the optimizer. In LP problems using Z3, a context is the first object to create. 

Then, the first variables to be found are created. Remember the goal of the problem `Our goal is to find the position in the space <x,y,z> ...` - so we will need to have x,y and z !

`x,y,z` are `ArithExpr` that is _arithmetic expressions_ of type _int constant_. A constant will be a variable to find for us, in this case of case int. More info on MkConst can be found [here](https://z3prover.github.io/api/html/class_microsoft_1_1_z3_1_1_context.html#a8ca84ff84e3488b10c965b5eaca146cc)

We continue and we find a new block of declarations

``` csharp
ArithExpr[] inRanges = new ArithExpr[Bots.Count];
ArithExpr rangeCount = z3Ctx.MkIntConst("sum");
Optimize optimizer = z3Ctx.MkOptimize();
```

`inRanges` is an array of `ArithExpr` that is prepared to hold as many expressions as bots we have. 

`rangeCount` is another constant to be found (back to our problem statement: `... that lies in the range of the most nanobots possible`. So we will have to find the _most rangeCount_. 

Finally, the `Optimize` object will be used to effectively run the optimization of the constants given the constraints we will add in the nect block. 

### Block 2 : Adding the "constraints" to the Optimizer

Until now we have just declared and initialized some objects and constant, the next step is to **add the constraints into our optimizer**. The constraints are the set of expressions that our solution has to take into account. If we have a **SOLVER** all the constraints we add into the solver must be satisfied in order for a valid solution to be yield. 

In our case, we do not have a **solver** but an **OPTIMIZER** , which means that we will add the constraints and the optimization engine will not look for the satisfaction of all the expressions, but for a **maximum** or **minimum** value of a given expression to optimize.

``` csharp
for (int i = 0; i < Bots.Count; i++)
{
    Coord3D pos = Bots[i].Position;
    int range   = Bots[i].Range;

    inRanges[i] = z3Ctx.MkIntConst("in_range_" + i);
    ArithExpr difX = z3AbsDif(x, pos.x, z3Ctx);
    ArithExpr difY = z3AbsDif(y, pos.y, z3Ctx);
    ArithExpr difZ = z3AbsDif(z, pos.z, z3Ctx);

    ArithExpr dist = z3Ctx.MkAdd(difX, difY, difZ);

    // o.add(in_ranges[i] == dist <= nrng, 1, 0))
    optimizer.Add(z3Ctx.MkEq(inRanges[i],
                                z3Ctx.MkITE(z3Ctx.MkLe(dist, z3Ctx.MkInt(range)),
                                            z3Ctx.MkInt(1),
                                            z3Ctx.MkInt(0)  
                                            ) ) );
}
```

What we do here is the following pseudocode

```
foreach Bot_i in Nanobots
    - Create a new constant named in_range_i and add it to the array inRanges
    - build the expression dist to calculate the Manhattan distance betwen Bot_i and our x,y,z constants
    - Add a new expression to the optimizer:
    - in_ranges[i] = BooleanExpression(IF dist <= nrng THEN 1 ELSE 0)
```

In here we start suffering the difference of syntax integration between Python and C#. There is one important rule to bear in mind: **Z3 does not work with variables or values directly, or any C# operator or statement**. Whatever we want to add to the optimizer, it has to be **a z3 constant** or an **arithmetic expression**. On top of that, Z3 does not support all the operations from the beginning. 

#### Step 1a - Distance between bot pos and x,y,z cts (Substraction)

Our starting point is that we want to build an expression to compute a Manhattan distance between the previously defined constants `x,y,z` and the position of the Nanobot stored in `Bots[i].Position` (that happens to be an object with fields named `x,y,z` as well, we will treat them as  `bot.x,bot.y,bot.z`.

The Manhattan distance is defined as `|x-bot.x| + |y-bot.y| + |z-bot.z|` - but remember, up to this point `bot.x` is a plain C# integer, while `x` is the Z3 constant we declared before. 

In order to first compute any substraction, we will:
1. Create an expression to transform `bot.x` into a Z3 ArithExpression
2. Create an expression to substract `x minus bot.x`
3. Once we have this substraction, we can calculate the absolute value

This can be seen in these 2 lines of code:

``` csharp
ArithExpr expr_x       = ctx.MkInt(bot.x);
ArithExpr substraction = ctx.MkSub(expr_x, x);
```

The first one creates a Z3 expression `expr_x` from `int bot.x` and the second implements a substraction (`MkSub`) between `expr_x` and `x`

Now it's only a matter of calculating the absolute value. Piece of cake, right? Wrong.

#### Step 1b - Absolute value of the substraction

Z3 has some methods to implement many expressions (Additions, substractions, comparisons, etc ...) but it **has not a direct method to implement an absolute value expression**. So we have to build it ourselves.

We can interpret the absolute value operation as a comparison: 
```
Abs(x) = if(x <0) 
         then 0-x 
         else x
```

And this is precisely what has been done, but since we will use it many times, a metod `z3Abs` has been created:

``` csharp
 ArithExpr z3Abs(ArithExpr a, Context ctx)
            => (ArithExpr) ctx.MkITE(ctx.MkLt(a, ctx.MkInt(0)),
                                     ctx.MkSub(ctx.MkInt(0), a),
                                     a);
```

What the method above is doing is the following:

1. Create an **IF THEN ELSE** expression `MkITE` that has a boolean expression as a first parameter (if), an expression as the second (then), and an expression as the third (else)
2. First param : Create a **LESS THAN** expression `MkLt` that compares expression `a` with an integer constant expression 0 `MkInt(0)`
3. Second param : Create a **SUBSTRACTION** expression `MkSub` that substracts expression `a` from an integer constant expression 0 `MkInt(0)`
4. Third param : Expression a

We have a cast to `ArithExpr` - this is done because `MkITE` return an object of type `Expr` and not `ArithExpr` (in Z3 we can use ITE to return many things, from numbers to strings) - but in this case we're certain that the object to be returned will be `ArithExpr` because we're either serving an `ArithExpr` from `a` or from the substraction itself. 

In the code the `z3Abs` can be found, as well as `z3AbsDif`which implements the substraction and absolute value in the same method:

``` csharp
 ArithExpr z3AbsDif(ArithExpr x, int nx, Context ctx)
{
    ArithExpr expr_x       = ctx.MkInt(nx);
    ArithExpr substraction = ctx.MkSub(expr_x, x);

    return (ArithExpr) ctx.MkITE(ctx.MkLt(substraction, ctx.MkInt(0)),
                                          ctx.MkSub(ctx.MkInt(0), substraction),
                                          substraction);
}
```

In the code of Part 2 , the calculation of the absolute value of the substraction is obtained in these lines, calling the method just mentioned:

``` csharp
ArithExpr difX = z3AbsDif(x, pos.x, z3Ctx);
ArithExpr difY = z3AbsDif(y, pos.y, z3Ctx);
ArithExpr difZ = z3AbsDif(z, pos.z, z3Ctx);
```

#### Step 1c - Addition of the factors to get the distance

We finally reach this line:

``` csharp
ArithExpr dist = z3Ctx.MkAdd(difX, difY, difZ);
```

We are creating an expression that holds the **Manhattan distance** calculated from the absolute value of the substractions we just calculated. 

#### Step 2 - Adding the expression to the optimizer

Once we have the distance, we have to add the expression for that **given bot `bot_i` distance** to the optimizer. This allows us to be able to determine whether a position in space is in range of that given bot or not.

``` csharp
optimizer.Add(z3Ctx.MkEq(inRanges[i],
                        z3Ctx.MkITE(z3Ctx.MkLe(dist, z3Ctx.MkInt(range)),
                                    z3Ctx.MkInt(1),
                                    z3Ctx.MkInt(0)  
                                    ) ) );
```

Ok, let's start from right to left, let's call `IsPositionInRangeOrNot`to :

``` csharp
z3Ctx.MkITE(z3Ctx.MkLe(dist, z3Ctx.MkInt(range)),
            z3Ctx.MkInt(1),
            z3Ctx.MkInt(0)  
            ) 

```

In here we find an **IF THEN ELSE** Expression we've seen before. We're basically building an expression that checks if the **calculated distance is less or equal to the bot range** `z3Ctx.MkLe(dist, z3Ctx.MkInt(range)`.

If the distance is in Range, we obtain a 1 `z3Ctx.MkInt(1)`. If not, a 0 `z3Ctx.MkInt(0)`.

So the first snippet is something like this:

``` csharp
optimizer.Add(z3Ctx.MkEq(inRanges[i], IsPositionInRangeOrNot) );
```

Which translates to: `in_ranges[i] == dist <= nrng, 1, 0)`

We are adding an expression to the optimizer that later will be able to be checked, and we are storing it in our **Array of expressions `inRanges`**

### Block 3 : The optimization

Once we have looped through all the bots and built the expressions that allow z3 if a position is in range of them, we proceed to the last block of the program, the optimization:

```csharp
optimizer.Add(z3Ctx.MkEq(rangeCount, z3Ctx.MkAdd(inRanges)));
ArithExpr distFromZero = z3Ctx.MkIntConst("dist");
optimizer.Add( z3Ctx.MkEq(distFromZero, z3Ctx.MkAdd(z3Abs(x, z3Ctx), z3Abs(y, z3Ctx), z3Abs(z, z3Ctx))  ) );

optimizer.MkMaximize(rangeCount);
var resDis = optimizer.MkMinimize(distFromZero);
optimizer.Check();

// Maximize the number of nanobots in range
Console.WriteLine(resDis.Lower.ToString());

```

What we do here, in order is the following:

1- Create a new expression for the optimizer that tells him that `rangeCount` equals to the sum of all the `inRanges`elements. 

`optimizer.Add(z3Ctx.MkEq(rangeCount, z3Ctx.MkAdd(inRanges)));`

We just assigned each of these element list to one Range check of a nanobot, returning 1 if the position is in range or 0 if not. `rangeCount` will be holding **how many nanobots have a position in range** which is effectively what we want to maximize. 

2- Create another constante named `dist` and keep it on `ArithExpr distFromZero`. Then build this expression, distFromZero summatory of the absolute values of each coord (which is the module, but we do not need to substract anything because Origin = 0,0,0)

``` csharp
ArithExpr distFromZero = z3Ctx.MkIntConst("dist");
optimizer.Add( z3Ctx.MkEq(distFromZero, z3Ctx.MkAdd(z3Abs(x, z3Ctx), z3Abs(y, z3Ctx), z3Abs(z, z3Ctx))  ) );
```

3 - Finally, we optimize what we want to. **ORDER IS IMPORTANT** - as we can see:

``` csharp
optimizer.MkMaximize(rangeCount);
var resDis = optimizer.MkMinimize(distFromZero);
optimizer.Check();

// Maximize the number of nanobots in range
Console.WriteLine(resDis.Lower.ToString());
```

We first *Maximize* the `rangeCount` expression, because we want to find the positions **where the most bots are in range**.

`optimizer.MkMaximize(rangeCount);` 

We then *Minimize* the `distFromZero`  expression. Z3 Optimizers act in sequence, we can have several expressions to optimize and the sequece of optimizations filter the results. In our case, the first optimization (maximize) has yielded the set of positions where the most nanobots are in range, and the second optimization (minimize) has taken only into account the positions that satisfied the first optimization. 

``` csharp
var resDis = optimizer.MkMinimize(distFromZero);
optimizer.Check();
```

It is important that in order to access the **results of an optimization** a `Handle`object must be retrieved from the optimization call. In our case, we are keeping it in `resDis`.

The call to `Check` makes the optimizer check the satisfiability of asserted constraints. Produces a model that (when the objectives are bounded and don't use strict inequalities) meets the objectives.

And **finally we print the result of the process**

` Console.WriteLine(resDis.Lower.ToString());`

Notice how we access `resDis` , that holds the `Handle` to the result of the **distance to origin optimization (Minimize)** and we access the `Lower` property to show it on screen. 

Day 23 - Part 2 Solved :)

## Conclusion

First of all, Z3 rocks big time. Being able to solve a problem like this has been great IMO.

Second, if I had to use Z3 on a daily basis, I would use it on Python and not C#. I still have to try a the LINQ Wrapper that are around ( this [one](https://github.com/endjin/Z3.Linq)) and dive a little deeper into the matter, but it seems to me that Z3 is most enjoyed on languages where the verbosity needed is not this much. 

