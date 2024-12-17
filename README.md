# IronCaml
IronCaml is an open-source implementation of the OCaml programming language written for .NET.

IronCaml is written on top of the Dynamic Language Runtime

Here is an example how to call OCaml code from a C# program.

leap.ml
```
let divisible_by_four y = y mod 4 = 0

let divisible_by_hundred y = y mod 100 = 0

let divisible_four_hundred y = y mod 400 = 0

let leap_year y = divisible_by_four y && (not (divisible_by_hundred y) || divisible_four_hundred y)
```
Program.cs
```cs
var code = File.ReadAllText("leap.ml");
var isLeapYear = IronCaml.Execute<Func<long, bool>>(code);
// false
isLeapYear(1800);
// true
isLeapYear(1996);
```
