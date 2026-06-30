using System;
using System.Text;

namespace AesCwcNet.AotTest;

internal class Program
{
    static void Main(string[] args)
    {
        string test = "Hello, world!";

        byte[] key = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];
        using var cwc = new Cwc(key);

        byte[] iv = new byte[11];
        byte[] header = new byte[4];
        byte[] message = Encoding.ASCII.GetBytes(test);
        byte[] tag = new byte[16];
        cwc.Encrypt(iv, header, message, tag);

        Console.WriteLine($"Cipher Text (Hex): {Convert.ToHexString(message)}");
        Console.WriteLine($"   Auth Tag (Hex): {Convert.ToHexString(tag)}");
        cwc.Decrypt(iv, header, message, tag);

        test = Encoding.ASCII.GetString(message);
        Console.WriteLine(test);
    }
}
