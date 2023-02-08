# Scheme R⁷RS in C#

This project aims to implement a R⁷RS conformant Scheme interpreter in C# for the purposes of self-education.

## Getting Started

1. **Prerequisites**: You will need a working .NET 7.0+ development environment.
2. Run the read-eval-print-loop project, `cd Scheme.Repl && dotnet run`.

## References

[Revised⁷ Report on the Algorithmic Language Scheme](https://standards.scheme.org/official/r7rs.pdf):
The Scheme implemented in this project aspires to be a conformant R⁷RS implementation.

[Three Implementation Models for Scheme](https://www.cs.unm.edu/~williams/cs491/three-imp.pdf):
The compiler is an implementation of the heap-based model described in R. Kent Dybvig's dissertation.

[Crafting Interpreters](https://craftinginterpreters.com/):
An excellent, non-academic introduction on how to implement a programming language.

[Write Yourself a Scheme in 48 Hours](https://en.wikibooks.org/wiki/Write_Yourself_a_Scheme_in_48_Hours):
Fun guide on how to implement Scheme in the Haskell language using a tree-walk interpreter.

## License

Copyright (C) 2023 Paul Samways

This scheme implementation is available under the MIT license.