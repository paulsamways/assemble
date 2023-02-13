# Assemble - a Scheme R⁷RS implementation in C#

This project aims to implement a R⁷RS conformant Scheme interpreter in C# for the purposes of self-education.

## Getting Started

1. **Prerequisites**: You will need a working .NET 7.0+ development environment.
2. Run the read-eval-print-loop project, `cd src/Scheme.Repl && dotnet run`.

## Examples

### Fibonacci

```scheme
(set! fib 
    (lambda (n) 
        (if (< n 2) 
            n 
            (+ (fib (- n 1)) (fib (- n 2)))
        )
    )
)
```

### List Map

```scheme
(set! map 
    (lambda (f xs)
        (if (pair? xs) 
            (cons (f (car xs)) (map f (cdr xs))) 
            xs
        )
    )
)
```

### List Filter

```scheme
(set! filter
    (lambda (f xs)
        (if (pair? xs)
            (if (f (car xs))
                (cons (car xs) (filter f (cdr xs)))
                (filter f (cdr xs))
            )
            xs
        )
    )
)
```

### FizzBuzz

```scheme
(set! fizz? (lambda (x) (eq? 0 (mod x 3))))
(set! buzz? (lambda (x) (eq? 0 (mod x 5))))

(set! fizzbuzz 
    (lambda (x) 
        (if (fizz? x) 
            (if (buzz? x) "FizzBuzz" "Fizz")
            (if (buzz? x) "Buzz" x)
        )
    )
)

(map fizzbuzz (range 1 20))
```

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