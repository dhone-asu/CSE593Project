# Computer Algebra System (CAS)

A command-line **computer algebra system written in C#** that parses and evaluates mathematical expressions, performs symbolic algebra and differentiation, and supports numerical calculus and special functions. Developed as a CSE 593 project.

The application includes its own tokenizer, precedence-based expression parser, expression-tree representation, and evaluation and simplification logic. It also supports user-defined variables and functions in an interactive session.

## Features

- **Symbolic algebra:** simplify expressions, combine like terms, and expand polynomial expressions.
- **Symbolic differentiation:** calculate first-order and higher-order derivatives.
- **Numerical calculus:** evaluate definite integrals and derivatives at specified points.
- **Summations and products:** evaluate finite sums and products, including expressions containing symbolic variables.
- **Mathematical functions:** trigonometric, inverse trigonometric, hyperbolic, exponential, logarithmic, factorial, and other functions.
- **Special functions:** evaluate the principal branch of the Lambert W function.
- **User-defined symbols:** assign variables and define functions with parameters.
- **Interactive evaluation:** enter successive expressions and reference the previous result using `ans`.

## Technologies

- **Language:** C#
- **Framework:** .NET Framework 4.7.2
- **Application type:** Console application
- **Development environment:** Visual Studio / MSBuild

The project uses standard .NET Framework libraries and does not declare third-party NuGet package dependencies.

## Getting Started

### Prerequisites

- Windows with **.NET Framework 4.7.2** (or a compatible installed version)
- Visual Studio with .NET Framework desktop development support and the 4.7.2 targeting pack, for building from source

### Build and Run

1. Clone the repository:

   ```bash
   git clone https://github.com/dhone-asu/CSE593Project.git
   cd CSE593Project
   ```

2. Open `CSE593Project.slnx` in a version of Visual Studio that supports `.slnx` solution files. Alternatively, open `CAS/CAS.csproj` directly.
3. Build the `CAS` project and run it from Visual Studio, or launch the generated `CAS.exe` from its build output directory (for example, `CAS/bin/Debug/`).
4. Enter a mathematical expression at the console prompt and press **Enter** to evaluate it.

The console runs continuously; use **Ctrl+C** or close the terminal to exit.

## Usage Examples

Enter one expression per line. The following examples illustrate supported input syntax.

### Basic Expressions

```text
2+3*4
sin(pi/2)
sqrt(2)
fact(5)
```

### Symbolic Algebra

```text
simplify(x^2+x+3*x^2+1)
expand((a+b)^3)
expand((x+1)^6)
```

### Calculus

```text
deriv(x^3+2*x,x)
nderiv(x^5,x,3)
diff(sin(x),x,pi/4)
integ(sqrt(1-x^2),x,-1,1)
```

`deriv` and `nderiv` perform symbolic differentiation; `diff` and `integ` perform numerical calculations.

### Summation, Products, and Special Functions

```text
sum(k^2,k,0,4)
product(x+k,k,0,4)
lambertw(1)
```

### User-Defined Variables and Functions

```text
phi := (sqrt(5)+1)/2
phi^2
add(x,y) := x+y
add(3,4)
ans
```

Variables and functions remain available during the current program session. The built-in `ans` variable contains the most recently evaluated result.

## Expression Syntax

| Operator | Meaning |
| --- | --- |
| `^` | Exponentiation |
| `-` | Unary negation |
| `*`, `/` | Multiplication, division |
| `+`, `-` | Addition, subtraction |
| `=` | Equality |
| `:=` | Assignment |

- Parentheses control grouping; exponentiation is right-associative.
- **Explicit multiplication is required:** use `2*x`, not `2x`.
- Identifiers are case-sensitive and may contain letters, digits, and underscores (starting with a letter).
- Scientific notation is supported, for example `1.3E-10`.
- Trigonometric functions use **radians**.

### Built-In Constants

| Constant | Meaning |
| --- | --- |
| `pi` | π |
| `e` | Euler's number |
| `inf` | Infinity |
| `gamma` | Euler–Mascheroni constant |

Additional functions and detailed syntax rules are documented in the technical guide.

## Project Structure

```text
CSE593Project/
├── CAS/
│   ├── Program.cs                  # Interactive console entry point
│   ├── Tokenizer.cs                # Lexical analysis
│   ├── Parser.cs                   # Expression parsing and precedence
│   ├── ExprNode.cs                 # Expression tree nodes
│   ├── Evaluator.cs                # Expression evaluation and output
│   ├── Simplification.cs           # Algebraic simplification
│   ├── Expansion.cs                # Algebraic expansion
│   ├── Polynomial.cs               # Polynomial representation and operations
│   ├── Calculus.cs                 # Symbolic and numerical calculus
│   ├── ExtendedMath.cs             # Additional mathematical functions
│   ├── Equation.cs                 # Equation-related representation
│   ├── SessionManager.cs           # Session-level symbols and functions
│   ├── SymbolManagement/           # Built-in and user-defined symbols
│   └── CAS.csproj                  # .NET Framework project file
├── CAS Technical Guide.pdf         # Full syntax and function reference
└── CSE593Project.slnx              # Visual Studio solution
```

## Implementation Overview

**Parsing:** `Tokenizer` converts input into tokens. `Parser` uses a precedence-climbing algorithm to construct an expression tree, accounting for operator precedence, parentheses, and associativity.

**Evaluation and simplification:** Expression-tree nodes are processed by the evaluation and simplification components. Polynomial and expansion logic support operations such as collecting like terms and distributing products.

**Calculus:** Symbolic derivatives are computed using the project's polynomial/expression representation. Numerical differentiation uses a finite-difference approximation, while definite integration uses a composite Simpson-style quadrature method.

**Symbol management:** `SessionManager` coordinates built-in constants, native functions, and user-defined variables and functions, allowing expressions to reference previously defined symbols.

## Documentation

See the [CAS Technical Guide](CAS%20Technical%20Guide.pdf) for the full operator grammar, function signatures, examples, and details about supported mathematical operations.

## Notes and Limitations

- This is an educational computer algebra implementation, not a replacement for a full-featured CAS such as Mathematica or SymPy.
- Symbolic transformations are limited to the expression forms supported by the implementation.
- Numerical integration and differentiation are approximations and may be sensitive to rounding error, step size, or the function being evaluated.
- Improper integrals are approximated using finite bounds in the current implementation.
- The program is a console application and does not provide a graphical interface.

## Author

**Dennis Hone** — [GitHub](https://github.com/dhone-asu)
