using System.Text;

namespace Sha1AlgorithmProject.Math
{
    public class Sha1
    {
        private readonly int BlockSizeBytes = 64;
        private readonly int MessageLengthSizeBytes = 8;
        private readonly int WordsCount = 80;

        /// <summary>Константы алгоритма</summary>
        private readonly uint[] Salt =
        {
            0x5A827999,
            0x6ED9EBA1,
            0x8F1BBCDC,
            0xCA62C1D6
        };

        /// <summary>Начальные значения регистра</summary>
        private Registers Buffer = new()
        {
            A = 0x67452301,
            B = 0xEFCDAB89,
            C = 0x98BADCFE,
            D = 0x10325476,
            E = 0xC3D2E1F0,
        };

        /// <summary>
        /// Логическая функция над словами B,C и D, вычисляемая в зависимости от итерации t
        /// </summary>
        /// <param name="t">Номер итерации</param>
        /// <param name="B">32-битное слово B</param>
        /// <param name="C">32-битное слово C</param>
        /// <param name="D">32-битное слово D</param>
        /// <returns>Возвращает 32-битный результат побитовых операций</returns>
        /// <exception cref="InvalidOperationException"></exception>
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

        /// <summary>
        /// Функция дополнения блока до длины, кратной размеру блока сообщения
        /// </summary>
        /// <param name="bytes">Ссылка на список байтов, которые необходимо дополнить</param>
        private void CompleteBlock(ref List<byte> bytes)
        {
            // Вычисляем длину исходного сообщения
            int length = bytes.Count;
            // Находим длину последнего блока
            int mod = length % BlockSizeBytes;
            // Дописываем 1 и 7 нулей : 10000000 (0х80)
            bytes.Add(0x80);
            // Вычисляем количество байт без учета байт для записи длины исходного сообщения
            int messageBlockSizeBytes = BlockSizeBytes - MessageLengthSizeBytes;
            // Производим вычисление необходимого кол-ва байтов с 0. Инициализируем -1, так как уже добавили блок 0х80.
            int countZeroBlocks = -1;
            if (mod < messageBlockSizeBytes)
                // Если длина последнего блока меньше 448 бит, то необходимо дополнить блок до 448 бит нулями,
                // и записать длину исходного сообщения в последних 64 битах.
                countZeroBlocks += (messageBlockSizeBytes - mod);
            else
            {
                // Если длина последнего блока равна 448бит, то необходимо дописать текущий блок (64 бита) нулями,
                // создать новый, заполнить его до 448 бит и записать длину исходного сообщения в последних 64 битах.
                // Кол-во нулей равно размеру блока
                if (mod == messageBlockSizeBytes)
                    countZeroBlocks += BlockSizeBytes;
                // длина последнего блока больше 448 бит, то необходимо дополнить блок до 512 бит нулями, 
                // создать новый, заполнить его до 448 бит и записать длину исходного сообщения в последних 64 битах.
                // Кол-во нулей равно разнице длины всего блока и длины последнего блока + 448 бит
                else
                    countZeroBlocks += (BlockSizeBytes - mod + messageBlockSizeBytes);
            }
            // Заполняем нулями
            for (int i = 0; i < countZeroBlocks; ++i)
                bytes.Add(0);
            // Представляем длину исходного сообщения в байтах
            long lengthInBits = length * 8;
            byte[] length2Bytes = BitConverter.GetBytes(lengthInBits);
            // Добавляем длину исходного сообщения в байтах, записанную в прямом порядке
            bytes.AddRange(length2Bytes.Reverse());
        }

        /// <summary>
        /// Метод получения хеш-значения по байтам
        /// </summary>
        /// <param name="bytes">Сообщение в байтах</param>
        /// <returns>Хеш-значение в hex-формате</returns>
        public string GetHash(byte[] bytes)
        {
            var listBytes = bytes.ToList();
            CompleteBlock(ref listBytes);
            var registers = Algorithm(listBytes);
            return registers.ToString();
        }

        /// <summary>
        /// Метод получения хеш-значения по строке
        /// </summary>
        /// <param name="bytes">Сообщение</param>
        /// <returns>Хеш-значение в hex-формате</returns>
        public string GetHash(string message)
        {
            return GetHash(Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// Функция прохождения алгоритма по всем блокам сообщения
        /// </summary>
        /// <param name="bytes">Списк байтов сообщения</param>
        /// <returns>Хеш-значение</returns>
        private Registers Algorithm(List<byte> bytes)
        {
            // Рассчитываем количество блоков сообщения
            long countBLocks = bytes.Count / BlockSizeBytes;
            // Инициализируем регистр начальным значением
            var registers = Buffer;
            // Выполняем для каждого блока сообщения обработку, состоящую из 80 итераций
            for (int i = 0; i < countBLocks; ++i)
            {
                // Перезаписываем регистр результатом обработки текущего регистра
                registers = BlockPrepare(block: bytes.GetRange(index: (int)(i * BlockSizeBytes),
                count: (int)BlockSizeBytes), buffer: registers);
            }
            return registers;
        }

        /// <summary>
        /// Функция основной обработки блока сообщения
        /// </summary>
        /// <param name="block">Список байтов блока сообщения</param>
        /// <param name="buffer">Значения регистра</param>
        /// <returns></returns>
        private Registers BlockPrepare(List<byte> block, Registers buffer)
        {
            // Создаем копию регистра
            var prepareBuffer = new Registers(buffer);
            // Создаем массив слов
            uint[] words = new uint[WordsCount];
            // Первые 16 слов инициализируем из блока сообщения.
            for (int i = 0; i < 16; ++i)
            {
                var wordListBytes = block.GetRange(index: i * 4, count: 4);
                wordListBytes.Reverse();
                words[i] = BitConverter.ToUInt32(wordListBytes.ToArray());
            }
            // Последующие слова высчитываем на основе предыдущих с помощью циклического сдвига
            for (int t = 16; t < WordsCount; ++t)
                words[t] = CyclicShift32(value: words[t - 3] ^ words[t - 8] ^ words[t - 14] ^ words[t - 16], count: 1);

            // Главный цикл обработки блока.
            for (int t = 0; t < WordsCount; ++t)
            {
                // Высчитываем значение для нового А
                uint temp = CyclicShift32(value: prepareBuffer.A, count: 5) + LogicalFunction(t, prepareBuffer.B, prepareBuffer.C, prepareBuffer.D)
                 + prepareBuffer.E + words[t] + Salt[t / 20];
                // Перезаписываем слова регистра новыми значениями
                prepareBuffer.E = prepareBuffer.D;
                prepareBuffer.D = prepareBuffer.C;
                prepareBuffer.C = CyclicShift32(value: prepareBuffer.B, count: 30);
                prepareBuffer.B = prepareBuffer.A;
                prepareBuffer.A = temp;
            }
            // Возвращаем сумму регистра, который пришел в начале обработки, и регистра, полученного в результате обработки
            return buffer + prepareBuffer;
        }

        /// <summary>
        /// Функция циклического сдвига влево
        /// </summary>
        /// <param name="value">32-битное значение</param>
        /// <param name="count">Кол-во сдвигаемых бит</param>
        /// <returns>Результат циклического сдвига</returns>
        private static uint CyclicShift32(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }
    }
}