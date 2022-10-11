namespace Sha1AlgorithmProject.Math
{
    public class Registers
    {
        public uint A { get; set; }
        public uint B { get; set; }
        public uint C { get; set; }
        public uint D { get; set; }
        public uint E { get; set; }

        public Registers() { }

        public Registers(in Registers other)
        {
            this.A = other.A;
            this.B = other.B;
            this.C = other.C;
            this.D = other.D;
            this.E = other.E;
        }

        public static Registers operator +(Registers a, Registers b)
        {
            return new Registers
            {
                A = a.A + b.A,
                B = a.B + b.B,
                C = a.C + b.C,
                D = a.D + b.D,
                E = a.E + b.E,
            };
        }

        public override string ToString()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(A).Reverse());
            bytes.AddRange(BitConverter.GetBytes(B).Reverse());
            bytes.AddRange(BitConverter.GetBytes(C).Reverse());
            bytes.AddRange(BitConverter.GetBytes(D).Reverse());
            bytes.AddRange(BitConverter.GetBytes(E).Reverse());
            return BitConverter.ToString(bytes.ToArray());
        }
    }
}