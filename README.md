# MukaVM

Compiler backend in C#

This is a hobby project that aims to compile a simple intermediate language (IR) into x86-64 assembly.

## Design

Currently it implements a limited number of instructions.

Construction of control flow graph and conversion to static single assignment form (SSA) are the main strategies for performing code optimization passes.

https://en.wikipedia.org/wiki/Control-flow_graph

https://en.wikipedia.org/wiki/Static_single_assignment_form

https://c9x.me/compile/bib/braun13cc.pdf

## Roadmap

- [x] Parse IR
- [x] Build Control Flow Graph
- [x] Convert to SSA
- [ ] Perform simple optimizations
- [ ] Register allocation
- [ ] Code generation

## What it looks like

### Intermediate representation (IR)

Available instructions: assignment, addition, jump, conditional jump, label, return

Fibonacci sequence example:

```
FUNCTION f {
    n1 = 0
    n2 = 1
    count = 10
    i = 2
    loop
    IF i = count: exit
    n3 = n1 + n2
    n1 = n2
    n2 = n3
    JMP loop
    exit
    RET
}
```

### Generated CFG in SSA form:

```
FUNCTION f {
    BB1 {
        v1 = 0
        v2 = 1
        v3 = 10
        v4 = 2
    }
    BB2 {
        v5 = PHI(v4, v12)
        v7 = PHI(v1, v10)
        v8 = PHI(v2, v11)
        IF v5 = v3: BB4
    }
    BB3 {
        v9 = v7 + v8
        v10 = v8
        v11 = v9
        v12 = v5 + 1
        JMP BB2
    }
    BB4 {
        RET
    }
}
```
