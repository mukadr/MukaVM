using System.Collections.Generic;
using System.Linq;

namespace MukaVM.IR
{
    public sealed class Function
    {
        public string Name { get; set; }
        public List<Instruction> Instructions { get; set; }

        public Function(string name, List<Instruction> instructions)
        {
            Name = name;
            Instructions = instructions;
        }

        public override bool Equals(object? obj)
            => obj is Function f &&
               f.Name == Name &&
               f.Instructions.SequenceEqual(Instructions);

        public override int GetHashCode()
            => Name.GetHashCode() ^ Instructions.GetHashCode();
    }
}
