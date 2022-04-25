# MukaVM

Compiler backend in C#

This is a hobby project that aims to compile a simple intermediate language (IR) into x86-64 assembly.

## Design

Currently it implements a limited number of instructions. Construction of control flow graph and conversion to static single assignment form (SSA) are the main strategies for performing code optimization passes.

https://en.wikipedia.org/wiki/Control-flow_graph

https://en.wikipedia.org/wiki/Static_single_assignment_form

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

```
FUNCTION f {
    x = 1
    again
    x = x + 1
    IF x > 5: end1
    IF x > 10: end2
    JMP again
    end1
    x = 10 + x
    end2
    y = x + 5
    RET
}
```

### Generated CFG in SSA form:

```
FUNCTION f {
    BB1 {
        v1 = 1
    }
    BB2 {
        v2 = PHI(v1, v3)
        v3 = v2 + 1
        IF v3 > 5: BB5
    }
    BB3 {
        IF v3 > 10: BB6
    }
    BB4 {
        JMP BB2
    }
    BB5 {
        v4 = 10 + v3
    }
    BB6 {
        v5 = PHI(v3, v4)
        v6 = v5 + 5
        RET
    }
}
```
