using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Revolutions.LocalizationGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var randomIds = new List<string>();
            for (var i = 0; i < 100; i++)
            {
                randomIds.Add(GetRandomString());
            }

            var filePath = Path.Combine(Environment.CurrentDirectory, "RandomIds.txt");
            File.WriteAllLines(filePath, randomIds);
            Process.Start(filePath);
        }

        private static string GetRandomString(int length = 8, string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            var outOfRange = byte.MaxValue + 1 - (byte.MaxValue + 1) % alphabet.Length;

            return string.Concat
            (
                Enumerable
                    .Repeat(0, int.MaxValue)
                    .Select(repeat => GetRandomByte())
                    .Where(randomByte => randomByte < outOfRange)
                    .Take(length)
                    .Select(randomByte => alphabet[randomByte % alphabet.Length]
            )
            );
        }

        private static byte GetRandomByte()
        {
            using (var randomizationProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[1];
                randomizationProvider.GetBytes(randomBytes);
                return randomBytes.Single();
            }
        }
    }
}