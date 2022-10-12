using System.Text;

namespace Sha1AlgorithmProject.Math
{
    public class Sha1
    {
        private readonly int BlockSizeBytes = 64;
        private readonly int MessageLengthSizeBytes = 8;
        private readonly int WordsCount = 80;
        private readonly uint[] Salt =
        {
            0x5A827999,
            0x6ED9EBA1,
            0x8F1BBCDC,
            0xCA62C1D6
        };

        private Registers Buffer = new()
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

        private void CompleteBlock(ref List<byte> bytes)
        {
            int length = bytes.Count;
            int mod = length % BlockSizeBytes;
            bytes.Add(0x80);
            int messageBlockSizeBytes = BlockSizeBytes - MessageLengthSizeBytes;
            int countZeroBlocks = -1;
            if (mod < messageBlockSizeBytes)
                countZeroBlocks += (messageBlockSizeBytes - mod);
            else
            {
                if (mod == messageBlockSizeBytes)
                    countZeroBlocks += BlockSizeBytes;
                else
                    countZeroBlocks += (BlockSizeBytes - mod + messageBlockSizeBytes);
            }
            for (int i = 0; i < countZeroBlocks; ++i)
                bytes.Add(0);
            long lengthInBits = length * 8;
            byte[] length2Bytes = BitConverter.GetBytes(lengthInBits);
            bytes.AddRange(length2Bytes.Reverse());
        }

        public string GetHash(byte[] bytes)
        {
            var listBytes = bytes.ToList();
            CompleteBlock(ref listBytes);
            var registers = Algorithm(listBytes);
            return registers.ToString();
        }

        public string GetHash(string message)
        {
            return GetHash(Encoding.UTF8.GetBytes(message));
        }

        private Registers Algorithm(List<byte> bytes)
        {
            long countBLocks = bytes.Count / BlockSizeBytes;
            var registers = Buffer;
            for (int i = 0; i < countBLocks; ++i)
            {
                registers = BlockPrepare(block: bytes.GetRange(index: (int)(i * BlockSizeBytes),
                count: (int)BlockSizeBytes), buffer: registers);
            }
            return registers;
        }

        private Registers BlockPrepare(List<byte> block, Registers buffer)
        {
            var prepareBuffer = new Registers(buffer);
            uint[] words = new uint[WordsCount];
            for (int i = 0; i < 16; ++i)
            {
                var wordListBytes = block.GetRange(index: i * 4, count: 4);
                wordListBytes.Reverse();
                words[i] = BitConverter.ToUInt32(wordListBytes.ToArray());
            }

            for (int t = 16; t < WordsCount; ++t)
                words[t] = CyclicShift32(value: words[t - 3] ^ words[t - 8] ^ words[t - 14] ^ words[t - 16], count: 1);

            for (int t = 0; t < WordsCount; ++t)
            {
                uint temp = CyclicShift32(value: prepareBuffer.A, count: 5) + LogicalFunction(t, prepareBuffer.B, prepareBuffer.C, prepareBuffer.D)
                 + prepareBuffer.E + words[t] + Salt[t / 20];
                prepareBuffer.E = prepareBuffer.D;
                prepareBuffer.D = prepareBuffer.C;
                prepareBuffer.C = CyclicShift32(value: prepareBuffer.B, count: 30);
                prepareBuffer.B = prepareBuffer.A;
                prepareBuffer.A = temp;
            }
            return buffer + prepareBuffer;
        }

        private static uint CyclicShift32(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }
    }
}