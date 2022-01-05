using MukaVM.IR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MukaVM.Gen.Amd64
{
    public class Register
    {
        public static List<Register> TestRegisters = new()
        {
            new("RAX"),
            new("RBX")
        };

        public string Name { get; }

        private Register(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }

    public class Reg : Var
    {
        public SSAVar SSAVar { get; }

        public Register Register { get; }

        public Reg(SSAVar ssaVar, Register r)
            : base(r.Name)
        {
            SSAVar = ssaVar;
            Register = r;
        }
    }

    public class RegAlloc
    {
        private readonly List<Register> _availableRegisters;

        private readonly HashSet<Reg> _busyRegs = new();
        private readonly HashSet<(SSAVar, int)> _savedVars = new();

        private RegAlloc(List<Register> availableRegisters)
        {
            _availableRegisters = availableRegisters;
        }

        public static void Transform(CfgFunction function)
        {
            new RegAlloc(Register.TestRegisters).TransformFunction(function);
        }

        private Register? GetAvailableRegister()
        {
            foreach (var register in _availableRegisters)
            {
                if (!_busyRegs.Any(r => r.Register == register))
                {
                    return register;
                }
            }
            return null;
        }

        private Reg? GetCurrentReg(SSAVar ssaVar)
        {
            return _busyRegs.SingleOrDefault(r => r.SSAVar == ssaVar);
        }

        private void TransformFunction(CfgFunction function)
        {
            function.BasicBlocks.ForEach(AllocateRegisters);
        }

        private void AllocateRegisters(BasicBlock bb)
        {
            for (var index = 0; index < bb.Instructions.Count; index++)
            {
                UseRegisterForInstruction(bb, bb.Instructions[index], index);
            }
        }

        private void UseRegisterForInstruction(BasicBlock bb, Instruction instruction, int index)
        {
            if (instruction is InstructionWithOperands io)
            {
                UpdateRegisterForInstructionOperands(bb, io, index);

                if (io is InstructionWithTarget it)
                {
                    AllocateRegisterForInstructionTarget(bb, it, index);
                }
            }
        }

        private void UpdateRegisterForInstructionOperands(BasicBlock bb, InstructionWithOperands io, int index)
        {
            for (var i = 0; i < io.Operands.Length; i++)
            {
                if (io.Operands[i] is SSAVar ssaVar)
                {
                    var reg = GetCurrentReg(ssaVar);
                    if (reg is null)
                    {
                        var saved = _savedVars.Single(sv => sv.Item1 == ssaVar);
                        reg = Spill(bb, ssaVar, index);
                        bb.Instructions.Insert(index + 1, new Restore(saved.Item2, reg));
                    }
                    io.Operands[i] = reg;
                }
            }
        }

        private void AllocateRegisterForInstructionTarget(BasicBlock bb, InstructionWithTarget it, int index)
        {
            if (it.Target is SSAVar ssaVar)
            {
                var reg = GetCurrentReg(ssaVar);
                if (reg is null)
                {
                    var register = GetAvailableRegister();
                    if (register is null)
                    {
                        reg = Spill(bb, ssaVar, index);
                    }
                    else
                    {
                        reg = new Reg(ssaVar, register);
                    }
                    Allocate(reg);
                }
                it.Target = reg;
            }
        }

        private Reg Spill(BasicBlock bb, SSAVar ssaVar, int index)
        {
            var freeReg = SaveSSAVar(bb, ssaVar, index);
            return new Reg(ssaVar, freeReg.Register);
        }

        private Reg SaveSSAVar(BasicBlock bb, SSAVar ssaVar, int index)
        {
            var freeReg = _busyRegs.First();
            var memPos = _savedVars.Count;
            bb.Instructions.Insert(index, new Save(memPos, freeReg));
            Deallocate(freeReg);
            _savedVars.Add((freeReg.SSAVar, memPos));
            return freeReg;
        }

        private void Allocate(Reg reg)
        {
            if (_busyRegs.Any(r => r.Register == reg.Register))
            {
                throw new Exception($"Register {reg} is in use.");
            }

            _busyRegs.Add(reg);
        }

        private void Deallocate(Reg reg)
        {
            if (_busyRegs.RemoveWhere(r => r.Register == reg.Register) == 0)
            {
                throw new Exception($"Register {reg} is not in use.");
            }
        }
    }
}