using System;
using System.Runtime.Intrinsics.Arm;

public class MD5Hash
{
    private uint[] T; // Массив для хранения констант T
    private byte[] buffer; // Массив для хранения буфера данных
    private uint[] X; // Массив для хранения блока данных X
    private uint A, B, C, D; // Переменные для хранения промежуточных результатов вычислений

    public MD5Hash()
    {
        T = new uint[64]; // Инициализация массива T длиной 64 элемента
        for (int i = 0; i < 64; i++)
        {
            T[i] = (uint)(Math.Pow(2, 32) * Math.Abs(Math.Sin(i + 1))); // Заполнение массива T значениями на основе математической функции
        }

        buffer = new byte[64]; // Инициализация массива буфера длиной 64 байта
        X = new uint[16]; // Инициализация массива X длиной 16 элементов
    }

    public string ComputeMD5Hash(string input)
    {
        A = 0x67452301; // Инициализация начальных значений переменных A, B, C, D
        B = 0xefcdab89;
        C = 0x98badcfe;
        D = 0x10325476;

        byte[] data = System.Text.Encoding.UTF8.GetBytes(input); // Преобразование входной строки в массив байт

        int originalLength = data.Length; // Получение длины исходных данных
        int paddedLength = originalLength + 1 + ((originalLength + 8) % 64); // Вычисление длины дополненных данных

        byte[] paddedData = new byte[paddedLength]; // Создание массива для дополненных данных
        Array.Copy(data, paddedData, originalLength); // Копирование исходных данных в массив дополненных данных
        paddedData[originalLength] = 0x80; // Добавление бита "1" в конец данных

        long bitLength = (long)originalLength * 8; // Вычисление длины данных в битах
        byte[] bitLengthBytes = BitConverter.GetBytes(bitLength); // Преобразование длины в байты
        Array.Copy(bitLengthBytes, 0, paddedData, paddedLength - 8, 8); // Добавление длины данных в конец массива

        for (int i = 0; i < paddedLength; i += 64)
        {
            int bytesToCopy = Math.Min(64, paddedLength - i); // Определение количества байт для копирования
            Array.Copy(paddedData, i, buffer, 0, bytesToCopy); // Копирование блока данных в буфер
            ProcessBlock(); // Обработка блока данных
        }

        return $"{A:X8}{B:X8}{C:X8}{D:X8}"; // Возврат результата в виде строки шестнадцатеричных чисел
    }

    private void ProcessBlock() // Метод для обработки блока данных
    {
        for (int i = 0; i < 16; i++) // Цикл обработки слов блока данных
        {
            X[i] = BitConverter.ToUInt32(buffer, i * 4); // Преобразование четырех байт в слово и сохранение в массив X
        }

        uint AA = A;
        uint BB = B;
        uint CC = C;
        uint DD = D;

        FF(ref A, B, C, D, X[0], 7, T[0]); // Вызов функции FF для первого раунда
        FF(ref D, A, B, C, X[1], 12, T[1]); // Вызов функции FF для второго раунда
        FF(ref C, D, A, B, X[2], 17, T[2]); // Вызов функции FF для третьего раунда
        FF(ref B, C, D, A, X[3], 22, T[3]); // Вызов функции FF для четвертого раунда

        // Продолжение с остальными раундами...

        A += AA;
        B += BB;
        C += CC;
        D += DD;
    }

    private void FF(ref uint a, uint b, uint c, uint d, uint x, int s, uint t) // Функция FF для раундов MD5
    {
        a += F(b, c, d) + x + t; // Вычисление нового значения переменной a
        a = RotateLeft(a, s); // Циклический сдвиг влево на s бит
        a += b; // Добавление значения переменной b
    }

    private uint F(uint x, uint y, uint z) // Функция F для раундов MD5
    {
        return (x & y) | (~x & z); // Логическая операция по формуле MD5
    }

    private uint RotateLeft(uint x, int n) // Функция циклического сдвига влево
    {
        return (x << n) | (x >> (32 - n)); // Реализация циклического сдвига
    }
}

class Program
{
    static void Main()
{
    MD5Hash md5 = new MD5Hash();

    string input = "Hello, World!";
    string hashResult = md5.ComputeMD5Hash(input);

    Console.WriteLine($"MD5 Hash of '{input}': {hashResult}");
}
}