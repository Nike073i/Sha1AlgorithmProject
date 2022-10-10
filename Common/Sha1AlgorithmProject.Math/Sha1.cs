using Sha1AlgorithmProject.Math.Interfaces;

namespace Sha1AlgorithmProject.Math
{
    // internal class Registers
    // {
    //     public uint A { get; set; }
    //     public uint B { get; set; }
    //     public uint C { get; set; }
    //     public uint D { get; set; }
    //     public uint E { get; set; }
    // }
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

        private static readonly uint[] InitialRegisters =
        {
            0x67452301,
            0xEFCDAB89,
            0x98BADCFE,
            0x10325476,
            0xC3D2E1F0,
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
            throw new NotImplementedException();
        }

        private void Algorithm(List<byte> bytes)
        {
        }

        public static void BlockPrepare(List<byte> block/*, uint A, uint B, uint C, uint D, uint E*/)
        {
            uint[] words = new uint[WordsCount];
            for (int i = 0; i < 16; ++i)
            {
                var wordListBytes = block.GetRange(i * 4, 4);
                wordListBytes.Reverse();
                words[i] = BitConverter.ToUInt32(wordListBytes.ToArray());
            }
        }
    }
}