# Parser grammar

```
<program> ::= <function>

<function> ::= "int" <identifier> "(" "void" ")" "{" <statement> "}"

<statement> ::= "return" <exp> ";"

<exp> ::= <factor> | <exp> <binop> <exp>

<factor> ::= <int> | <unop> <factor> | "(" <exp> ")"

<unop> ::= "-" | "~"

// See BinOp.cs for precedence
<binop> ::= "-" | "+" | "*" | "/" | "%" | "<<" | ">>" | "&" | "|"

<identifier> ::= ? An identifier token ?

<int> ::= ? A constant token ?
```
