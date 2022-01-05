using MukaVM.IR;

namespace MukaVM.Gen.Amd64
{
    public class Save : Instruction
    {
        public int Index { get; set; }
        public Reg Reg { get; set; }

        public Save(int index, Reg reg)
        {
            Index = index;
            Reg = reg;
        }

        public override string ToString() => "MEM[" + Index + "] = " + Reg.Register.Name;
    }

    public class Restore : Instruction
    {
        public int Index { get; set; }
        public Reg Reg { get; set; }

        public Restore(int index, Reg reg)
        {
            Index = index;
            Reg = reg;
        }

        public override string ToString() => Reg.Register.Name + " = MEM[" + Index + "]";
    }
}
