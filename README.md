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

### Pseudo code
```
FUNCTION sample {
    x = 1
    do {
        x = x + 1
    } while (x <= 10)
}
```

### Intermediate representation (IR)

Available instructions: addition, jump, labels, return

```
FUNCTION sample {
    x = 0 + 1
    again
    x = x + 1
    IF x > 10: end
    JMP again
    end
    RET
}
```

### Generated CFG in SSA form:

```
FUNCTION f {
    BB1 {
        v1 = 0 + 1
        <BB2>
    }
    BB2 {
        <BB1, BB3>
        v2 = PHI(v1, v3)
        v3 = v2 + 1
        IF v3 > 10: BB4
        <BB3, BB4>
    }
    BB3 {
        <BB2>
        JMP BB2
        <BB2>
    }
    BB4 {
        <BB2>
        RET
    }
}
```
