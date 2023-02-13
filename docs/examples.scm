(set! fib (lambda (n) (if (< n 2) n (+ (fib (- n 1)) (fib (- n 2))))))
(set! map (lambda (f xs) (if (pair? xs) (cons (f (car xs)) (map f (cdr xs))) xs)))
(set! filter (lambda (f xs) (if (pair? xs) (if (f (car xs)) (cons (car xs) (filter f (cdr xs))) (filter f (cdr xs))) xs)))
(set! fizz? (lambda (x) (eq? 0 (mod x 3))))
(set! buzz? (lambda (x) (eq? 0 (mod x 5))))
(set! fizzbuzz (lambda (x) (if (fizz? x) (if (buzz? x) "FizzBuzz" "Fizz") (if (buzz? x) "Buzz" x))))

(map fizzbuzz (range 1 20))