using System.Text;
using Sha1AlgorithmProject.Math.Interfaces;

namespace Sha1AlgorithmProject.Math
{
    public class Registers
    {
        public uint A { get; set; }
        public uint B { get; set; }
        public uint C { get; set; }
        public uint D { get; set; }
        public uint E { get; set; }
    }
    public class Sha1 : ISha1
    {
        private static readonly uint BlockSizeBytes = 64;
        private static readonly uint MessageLengthSizeBytes = 8;
        private static readonly uint WordsCount = 80;
        private static readonly uint[] Salt =
        {
            0x5A827999,
            0x6ED9EBA1,
            0x8F1BBCDC,
            0xCA62C1D6
        };

        private static readonly Registers InitialRegisters = new()
        {
            A = 0x67452301,
            B = 0xEFCDAB89,
            C = 0x98BADCFE,
            D = 0x10325476,
            E = 0xC3D2E1F0,
        };

        private static uint LogicalFunction(int t, uint B, uint C, uint D)
        {
            if (t >= 0 && t < 20)
                return (B & C) | ((~B) & D);
            else if (t >= 20 && t < 40)
                return (B ^ C ^ D);
            else if (t >= 40 && t < 60)
                return (B & C) | (B & D) | (C & D);
            else if (t >= 60 && t < 80)
                return (B ^ C ^ D);
            throw new InvalidOperationException("Получено некорректное значение итерации - " + t);
        }

        public static void CompleteBlock(ref List<byte> bytes)
        {
            ulong length = (ulong)bytes.Count;
            uint mod = (uint)length % BlockSizeBytes;
            bytes.Add(0x80);
            uint messageBlockSizeBytes = BlockSizeBytes - MessageLengthSizeBytes;
            uint countZeroBlocks = (mod == 0 || mod < messageBlockSizeBytes) ? messageBlockSizeBytes : BlockSizeBytes + messageBlockSizeBytes;
            countZeroBlocks -= (mod + 1);
            for (int i = 0; i < countZeroBlocks; ++i)
                bytes.Add(0);
            ulong lengthInBits = length * 8;
            byte[] length2Bytes = BitConverter.GetBytes(lengthInBits);
            bytes.AddRange(length2Bytes.Reverse());
        }

        public string GetHash(string message)
        {
            var listBytes = Encoding.UTF8.GetBytes(message).ToList();
            CompleteBlock(ref listBytes);
            var registers = Algorithm(listBytes);

            var register2Bytes = new List<List<byte>>
            {
                BitConverter.GetBytes(registers.A).ToList(),
                BitConverter.GetBytes(registers.B).ToList(),
                BitConverter.GetBytes(registers.C).ToList(),
                BitConverter.GetBytes(registers.D).ToList(),
                BitConverter.GetBytes(registers.E).ToList()
            };

            foreach (var bytes in register2Bytes)
            {
                bytes.Reverse();
            }

            var union = new List<byte>();
            union.AddRange(register2Bytes[0]);
            union.AddRange(register2Bytes[1]);
            union.AddRange(register2Bytes[2]);
            union.AddRange(register2Bytes[3]);
            union.AddRange(register2Bytes[4]);

            return BitConverter.ToString(union.ToArray());
        }

        private Registers Algorithm(List<byte> bytes)
        {
            long countBLocks = bytes.Count / BlockSizeBytes;
            var registers = InitialRegisters;
            for (int i = 0; i < countBLocks; ++i)
            {
                registers = BlockPrepare(block: bytes.GetRange(index: (int)(i * BlockSizeBytes),
                count: (int)BlockSizeBytes), registers: registers);
            }
            return registers;
        }

        public static Registers BlockPrepare(List<byte> block, Registers registers)
        {
            uint[] words = new uint[WordsCount];
            for (int i = 0; i < 16; ++i)
            {
                var wordListBytes = block.GetRange(index: i * 4, count: 4);
                wordListBytes.Reverse();
                words[i] = BitConverter.ToUInt32(wordListBytes.ToArray());
            }

            for (int t = 0; t < 80; ++t)
            {
                uint temp = CyclicShift(value: registers.A, count: 5) + LogicalFunction(t, registers.B, registers.C, registers.D)
                 + registers.E + words[t] + Salt[t / 20];
                if (t > 15)
                    words[t] = CyclicShift(value: words[t - 3] ^ words[t - 8] ^ words[t - 14] ^ words[t - 16], count: 1);
                registers.E = registers.D;
                registers.D = registers.C;
                registers.C = CyclicShift(registers.B, 30);
                registers.B = registers.A;
                registers.A = temp;
            }
            return registers;
        }

        private static uint CyclicShift(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }
    }
}