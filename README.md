# IronCaml
IronCaml is an open-source implementation of the OCaml programming language written for .NET.

IronCaml is written on top of the Dynamic Language Runtime

Here is an example how to call OCaml code from a C# program.

```cs
var code = "let add_two_numbers x y = x + y";
var addTwoNumbers = IronCaml.Execute<Func<long, long, long>> (code);
// result is 20
var result = addTwoNumbers(5, 15);
```
