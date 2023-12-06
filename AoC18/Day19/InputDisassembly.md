# Input disassembly

`I am copying this approach with pride from mMosiur - one of the best explanations out there on how to manage part 2 - I've added some additional info like the contents of the registers and the resulting instruction pointer hoping it can help someone else :)`

[Original file in mMosiur Github](https://github.com/mMosiur/AdventOfCode2018/blob/main/Day19%20-%20Go%20With%20The%20Flow/InputDisassembly.md)

## Declarations

`#ip 5` - Bind the instruction pointer to `r[5]`.

Later on `r[5]` will be annotated with `r<5>` instead to signalize that it is bound to the instruction pointer.

## Instructions

We can divide the instructions into 4 blocks:

### Jump to setup block

|  # |  Instruction  |  Pseudocode                   | Registers - Part 2     | ipAfter |
|---:|:--------------|:------------------------------|:-----------------------|:--------|
|  0 | `addi 5 16 5` | `r<5> = r<5> + 16`            | `[ 1, 0, 0, 0, 0, 16]` | `17`    |

Translation of the block above:

- ( go to #17 ), so directly start a [setup block](#base-setup-block). Explanation is that register 5 holds our instruction pointer (_ip_), so after the execution of the above, the _ip_ will increase one more and his value will be effectively **17**

### Main loop block

|  # |  Instruction  |  Pseudocode                   | Registers - Part 2                   | ipAfter | Pseudo                |
|---:|:--------------|:------------------------------|:-------------------------------------|:--------|-----------------------|
|  1 | `seti 1  0 4` | `r[4] = 1`                    | `[ 1, 0, 10551377, 10550400, 1, 1]`  | `2`     | For r[4]              |
|  2 | `seti 1  8 1` | `r[1] = 1`                    | `[ 1, 1, 10551377, 10550400, 1, 2]`  | `3`     | For r[1]              |
|  3 | `mulr 4  1 3` | `r[3] = r[4] * r[1]`          | `[ 1, 1, 10551377, 1, 1, 3]`         | `4`     | Calc prod             |
|  4 | `eqrr 3  2 3` | `r[3] = r[3] == r[2] ? 1 : 0` | `[ 1, 1, 10551377, 1/0, 1, 4]`       | `5`     | If_1 divisors         |
|  5 | `addr 3  5 5` | `r<5> += r[3]`                | `[ 1, 1, 10551377, 0, 1, 5 or 6]`    | `6-7`   | Skip next (if1-yes)   |
|  6 | `addi 5  1 5` | `r<5> += 1`                   | `[ 1, 1, 10551377, 0, 1, 7]`         | `8`     | Skip sum (if1-no)     |
|  7 | `addr 4  0 0` | `r[0] += r[4]`                | `R0 accums divisors`                 | `8`     | Add divisor (if1-yes) |
|  8 | `addi 1  1 1` | `r[1] += 1`                   | `[ 1, 2, 10551377, 0, 1, 8]`         | `9`     | Inc r[1] - for        |
|  9 | `gtrr 1  2 3` | `r[3] = r[1] > r[2] ? 1 : 0`  | `[ 1, 2, 10551377, 1/0, 1, 9]`       | `10`    | If_2 r[1] - for       |
| 10 | `addr 5  3 5` | `r<5> += r[3]`                | `[ 1, 2, 10551377, 0, 1, 10]`        | `11-12` | Skip next (if2 - yes) |
| 11 | `seti 2  5 6` | `r<5> = 2`                    | `[ 1, 2, 10551377, 0, 1, 2]`         | `3`     | Jmp to 3 (if2 - no)   |
| 12 | `addi 4  1 4` | `r[4] += 1`                   | `[ 1, 2, 10551377, 0, 2, 12]`        | `13`    | Inc r[4] - for        |
| 13 | `gtrr 4  2 3` | `r[3] = r[4] > r[2] ? 1 : 0`  | `[ 1, 2, 10551377, 1/0, 2, 13]`      | `14`    | If_3 r[4] - for       |
| 14 | `addr 3  5 5` | `r<5> += r[3]`                | `[ 1, 2, 10551377, 1/0, 2, 14/15]`   | `15-16` | Skip next (if3 - yes) |
| 15 | `seti 1  7 5` | `r<5> = 1`                    | `[ 1, 2, 10551377, 1/0, 2, 15]`      | `2`     | Jmp to 2 - for        |
| 16 | `mulr 5  5 5` | `r<5> *= r<5>`                | `[ 1, 2, 10551377, 1/0, 2, (16*16)]`  | `257`   | Finish Execution      |

Translation of the block above:

```
for r4 = 1..r5
    for r1 = 1..r5
      if r4 * r1 == r5:
        r0 += r4
```

At the end of the loop:

- `r[0]` = sum of all divisors of `r[3]`
- `r<5>` = (16*16) = 256, so the program halts

### Base setup block

|  # |  Instruction  |  Pseudocode                   | Registers - Part 2       | ipAfter |
|---:|:--------------|:------------------------------|:-------------------------|:--------|
| 17 | `addi 2  2 2` | `r[2] = r[2] + 2`             | `[ 1, 0, 2, 0, 0, 17]`   | `18`    |
| 18 | `mulr 2  2 2` | `r[2] = r[2] * r[2]`          | `[ 1, 0, 4, 0, 0, 18]`   | `19`    |
| 19 | `mulr 5  2 2` | `r[2] = r[2] * r<5>`          | `[ 1, 0, 76, 0, 0, 19]`  | `20`    |
| 20 | `muli 2 11 2` | `r[2] = r[2] * 11`            | `[ 1, 0, 836, 0, 0, 20]` | `21`    |
| 21 | `addi 3  6 3` | `r[3] = r[3] + 6`             | `[ 1, 0, 836, 6, 0, 21]` | `22`    |
| 22 | `mulr 3  5 3` | `r[3] = r[3] * r<5>`          | `[ 1, 0, 836, 132, 0, 22]`| `23`    |
| 23 | `addi 3  9 3` | `r[3] = r[3] + 9`             | `[ 1, 0, 836, 141, 0, 23]`| `24`    |
| 24 | `addr 2  3 2` | `r[2] = r[2] + r[3]`          | `[ 1, 0, 977, 141, 0, 24]`| `25`    |
| 25 | `addr 5  0 5` | `r<5> = r<5> + r[0]`          | `[ 1, 0, 977, 141, 0, 26]`| `27`    |
| 26 | `seti 0  5 5` | `r<5> = 0`                    | **Skipped in part 2**     | --   |

Translation of the block above:

- `r[3]` = 141 ( based on assumption that `r[1]` starts with 0 )
- `r[2]` = 977 ( based on assumption that `r[2]` and `r[3]` start with 0 )
- ( go to `r[0]`+1 ), effectively:
  - if `r[0]` == 0: ( go to #1 ) by stepping into line 26 and jump to [main loop block](#main-loop-block).
  - else if `r[0]` == 1: ( go to #27 ) by stepping over line 26 and jump to [extended setup block](#extended-setup-block)
  - else: undefined behavior and not supported.

Effectively: if we start with `r[0]` = 0 we jump into [the main loop](#main-loop-block) with `r[5]` = 981. If we start with `r[0]` = 1 we continue into the [extended setup](#extended-setup-block).

This is **where part 2 acts** - starting  with r[0] = 1 kicks an additional value to the instuction pointer at line 25, thus leading the excution to line 27.

### Extended setup block

|  # |  Instruction  |  Pseudocode                   | Registers - Part 2           | ipAfter |
|---:|:--------------|:------------------------------|:-----------------------------|:--------|
| 27 | `setr 5  9 3` | `r[3] = r<5>`                 | `[ 1, 0, 977, 27, 0, 27]`   | `28`    |
| 28 | `mulr 3  5 3` | `r[3] *= r<5>`                | `[ 1, 0, 977, 756, 0, 28]`   | `29`    |
| 29 | `addr 5  3 3` | `r[3] += r<5>`                | `[ 1, 0, 977, 785, 0, 29]`   | `30`    |
| 30 | `mulr 5  3 3` | `r[3] *= r<5>`                | `[ 1, 0, 977, 23550, 0, 30]`  | `31`    |
| 31 | `muli 3 14 3` | `r[3] *= 14`                  | `[ 1, 0, 977, 329700, 0, 31]`  | `32`    |
| 32 | `mulr 3  5 3` | `r[3] *= r<5>`                | `[ 1, 0, 977, 10550400, 0, 32]`  | `33`    |
| 33 | `addr 2  3 2` | `r[2] += r[3]`                | `[ 1, 0, 10551377, 10550400, 0, 33]`  | `34` |
| 34 | `seti 0  1 0` | `r[0] = 0`                    | `[ 1, 0, 10551377, 10550400, 0, 34]`  | `35` |
| 35 | `seti 0  0 5` | `r<5> = 0`                    | `[ 1, 0, 10551377, 10550400, 0, 0]`   | `1` |

Translation of the block above:

- `r[3]` = 10550400
- `r[2]` += `r[3]` (10551337)
- `r[0]` = 0
- `r<5>` = 0 ( go to #1 ), jump into [main loop block](#main-loop-block)

Effectively: We add `10550400` into `r[5]` and jump to [main loop block](main-loop-block).

## Conclusion

Considering all blocks and flow between them there are two supported flows that correspond with our part one and part two data:

If we start with `r[0]` = 0:

- We start with a jump into [base setup block](#base-setup-block) that leaves us with `r[3]` = 977.
- We jump straight into [main loop block](#main-loop-block) and calculate the sum of the divisors of `r[3]` = 977.
- When we exit the `r[0]` contains that sum the divisors of 977.

If we start with `r[0]` = 1:

- We start with a jump into [base setup block](#base-setup-block) that leaves us with `r[3]` = 977.
- We step into [extended setup block](#extended-setup-block) that leaves us with `r[3]` = 977 + 10550400 = 10551377.
- We jump into [main loop block](#main-loop-block) and calculate the sum of the divisors of `r[3]` = 10551377.
- When we exit the `r[0]` contains that sum the divisors of 10551377.