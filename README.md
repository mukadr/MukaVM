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

## Example

### Pseudo code
```
FUNCTION sample {
    x = 10
    if x > 5 {
        y = x + 3
    } else {
        y = x + 1
    }
    if y > 10 {
        z = y + 1
    } else {
        z = y + 5
    }
    t = z + y
}
```

### Intermediate representation (IR)

Available instructions: addition, jump, labels, return

```
FUNCTION sample {
    x = 0 + 10
    IF x > 5: gt5
    y = x + 1
    JMP end1
    gt5
    y = x + 3
    end1
    IF y > 10: gt10
    z = y + 5
    JMP end2
    gt10
    z = y + 1
    end2
    t = z + y
    RET
}
```

### Generated CFG in SSA form:

```
FUNCTION sample {
    BB1 {
        v1 = 0 + 10
        IF v1 > 5: BB3
        <BB2, BB3>
    }
    BB2 {
        <BB1>
        v2 = v1 + 1
        JMP BB4
        <BB4>
    }
    BB3 {
        <BB1>
        v3 = v1 + 3
        <BB4>
    }
    BB4 {
        <BB2, BB3>
        v4 = PHI(v2, v3)
        IF v4 > 10: BB6
        <BB5, BB6>
    }
    BB5 {
        <BB4>
        v5 = v4 + 5
        JMP BB7
        <BB7>
    }
    BB6 {
        <BB4>
        v6 = v4 + 1
        <BB7>
    }
    BB7 {
        <BB5, BB6>
        v7 = PHI(v5, v6)
        v8 = v7 + v4
        RET
    }
}
```