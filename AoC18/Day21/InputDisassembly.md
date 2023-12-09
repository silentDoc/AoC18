# Input disassembly
The same approach as day 19 part 2 is followed

## Declarations

`#ip 1` - Bind the instruction pointer to `r[1]`.

Later on `r[1]` will be annotated with `r<1>` instead to signalize that it is bound to the instruction pointer.

## Program

|  # |  Instruction        |  Pseudocode                   | Registers                       | ipAfter | Comments                  |
|---:|:--------------------|:------------------------------|:--------------------------------|:--------|---------------------------|
| 0  | `seti 123 0 3`      | r[3] = 123                    | [x, 0, 0, 123, 0, 0]            | 1       |                           |
| 1  | `bani 3 456 3`      | r[3] = r[3] & 456             | [x, 1, 0, 72 , 0, 0]            | 2       |                           |
| 2  | `eqri 3 72 3`       | r[3] = (r[3] == 72) ? 1 : 0   | [x, 2, 0,  1 , 0, 0]            | 3       | `Check r[3] == 72`        |
| 3  | `addr 3 1 1`        | r<1> = r<1> + r[3]            | [x, 3/4, 0,  1 , 0, 0]          | 4/5     | `No  - Jmp 4, Yes - Jmp 5`|
| 4  | `seti 0 0 1`        | r<1> = 0  ; jmp 1             | [x, 0, 0,  1 , 0, 0]            | 1       | `Jump to 1`               |
| 5  | `seti 0 1 3`        | r[3] = 0                      | [x, 5, 0,  0 , 0, 0]            | 6       |                           |
| 6  | `bori 3 65536 2`    | r[2] = r[3] | 65536           | [x, 6, 65536, 0 , 0, 0]         | 8       | `r[2] = 65536`            |
| 7  | `seti 1505483 6 3`  | r[3] = 1505483                | [x, 7, 65536, 1505483 , 0, 0]   | 9       | `r[3] = 1505483`          |
| 8  | `bani 2 255 4`      | r[4] = r[2] & 255             | [x, 8, 65536, 1505483 , 8, 0]   | 10      | `r[4] = 8`                |
| 9  | `addr 3 4 3`        | r[3] = r[3] + r[4]            | [x, 9 , 65536, 1505491 , 8, 0]  | 11      | `r[3] = 1505491`          |
| 10 | `bani 3 16777215 3` | r[3] = r[3] & 16777215        | [x, 10, 65536, 1505491 , 8, 0]  | 12      |                           |
| 11 | `muli 3 65899 3`    | r[3] = r[3] * 65899           | [x, 11, 65536, 426103601 , 8, 0]| 13      | `r[3] = 426103601`        |
| 12 | `bani 3 16777215 3` | r[3] = r[3] & 16777215        | [x, 12, 65536, 6673201, 8, 0]   | 14      | `r[3] = 6673201`          |
| 13 | `gtir 256 2 4`      | r[4] = (256 > r[2]) ? 1 : 0   | [x, 13, 65536, 6673201, 0, 0]   | 15      | `Check 256 > r[2]`        |
| 14 | `addr 4 1 1`        | r<1> = r<1> + r[4]            | [x, 14/15, 65536, 6673201, 0, 0]| 16/17   | `Yes - Jmp to 16`         |
| 15 | `addi 1 1 1`        | r<1> = r<1> + 1               | [x, 16, 65536, 6673201, 0, 0]   | 17      | `No - Jmp to 17`          |
| 16 | `seti 27 6 1`       | r<1> = 27                     | [x, 27, 65536, 6673201, 0, 0]   | 28      | `Yes - Jmp to 28 (2 jmps)`|
| 17 | `seti 0 3 4`        | r[4]  = 0                     | [x, 17, 65536, 6673201, 0, 0]   | 18      |                           |
| 18 | `addi 4 1 5`        | r[5]  = r[5] + 4              | [x, 18, 65536, 6673201, 0, 4]   | 19      |                           |
| 19 | `muli 5 256 5`      | r[5]  = r[5] * 256            | [x, 19, 65536, 6673201, 0, 1024]| 20      |                           |
| 20 | `gtrr 5 2 5`        | r[5] = (r[5] > r[2]) ? 1 : 0  | [x, 20, 65536, 6673201, 0, 1/0] | 21      | `Check r[5] > r[2]`       |
| 21 | `addr 5 1 1`        | r<1> = r[5] + r<1>            | [x, 21/22, 65536, 6673201, 0, 0]| 22/23   | `Yes - Jmp to 23`         |
| 22 | `addi 1 1 1`        | r<1> = r<1> + 1               | [x, 24, 65536, 6673201, 0, 0]   | 25      | `No - Jmp to 24`          |
| 23 | `seti 25 4 1`       | r<1> = 25                     | [x, 25, 65536, 6673201, 0, 1]   | 26      | `Yes - Jmp to 26`         |
| 24 | `addi 4 1 4`        | r[4] = r[4] + 4               | [x, 24, 65536, 6673201, 4, 0]   | 25      | `r[4] = 4`                |
| 25 | `seti 17 3 1`       | r<1> = 17                     | [x, 17, 65536, 6673201, 4, 0]   | 18      | `Jmp to 18`               |
| 26 | `setr 4 1 2`        | r[2] = r[4]                   | [x, 26, 0, 6673201, 4, 0]       | 27      |                           |
| 27 | `seti 7 4 1`        | r<1> = 7                      | [x, 7, 0, 6673201, 4, 0]        | 8       | `Jmp to 8`                |
| 28 | `eqrr 3 0 4`        | r[4] = (r[3] == r[0]) ? 1 : 0 | [x, 28, 65536, 6673201, 0/1, 0] | 29      | `Check r[3] = r[0] Only place!`  |
| 20 | `addr 4 1 1`        | r<1> = r[4] + r<1>            | [x, 29/30, 65536, 6673201, 0, 0]| 30/31   | `Halts if equal`          |
| 30 | `seti 5 9 1`        | r<1> = 5                      | [x, 5, 65536, 6673201, 0, 0]    | 6       | `Jmp to 6 if not equal`   |


## Conclusion

*Part 1* 
There is only one check done with R[0] (ins 28) - I will run the program until we land on instruction 28 and print the value of r[3] in that given moment

*Part 2* 
We need to reverse engineer the input, what is happening is something like this :


``` csharp
ISet<ulong> seenTargetRegisterValues
ulong targetRegister = 0;                                         // Target register is r[3], the one checked against r[0]
while (true)
{
  ulong tempRegister = targetRegister | 65536;                    // Line 6 - result in r[2]
  targetRegister = 1505483;                                       // Target register reset value, line 7
  while (true)
  {
    tempRegister = (tempRegister & 255);                          // Line 8 - result in r[4]
    targetRegister = targetRegister + tempRegister;               // Line 9
    targetRegister = targetRegister & 16777215;                   // Line 10
    targetRegister = targetRegister * 65899;                      // Line 11
    targetRegister = targetRegister & 16777215;                   // Line 12
    
    if (256 > tempRegister)                                       // Lines 13, 14, 15, 16 implement this if. True -> line 28, false 24
    {
      
      bool duplicate = !seenTargetRegisterValues.Add(targetRegister);
      if (duplicate)
      {
        yield break;
      }
      yield return targetRegister;
      break;
    }
    else
    {
        r4 = 0;

        r5 +=4;                                                  // Line 18 - This translates to a while - whlile (r5<=r2)
        r5*=256;
        if(r5>r2)                                               // Lines 20,21,22,23 implement this if. True --> line 26, False --> line 
        {
            r4+=4
            Jump to Line 18                                      // End of while loop
        }

        tempRegister = r4;                                       // 26 We jump back to beginning (tempRegister is r2)
        jmp to line 8                                            // We go back to the beginning of While (true)
    }
  }
}
```

This translates to what we are looking for in Part 2 - in C#. The key optimization that saves a lot of time is to see that the loo from lines 18 to 25 can be replaced by `r4 / 256` . Both the brute force (around 450 secs) implementation of Part 2 and the reverse engineered version (300 millis) are implemented in the solution. 

``` csharp
public int CalcPart2()
{
    int B, E;
    int prev = -1;
    var seen = new HashSet<int>();

    B = 65536;
    E = 1505483;

    while (true)
    {
        E += B & 255;
        E &= 16777215;
        E *= 65899;
        E &= 16777215;

        if (B < 256)
        {
            // Find the last new value of E during instruction 28 before it cycles
            // That will be the last hit, so setting r[0] to its value results
            // in the most instructions among all possible terminating values.
            if (!seen.Add(E))
            {
                return prev;
            }
            else
            {
                prev = E;
            }
            B = E | 65536;
            E = 1505483;
        }
        else
        {
            B = (int) Math.Floor((decimal) (B / 256));
        }
    }
}
``` 

