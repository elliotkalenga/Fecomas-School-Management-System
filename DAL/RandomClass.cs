using System;

public class RandomStringGenerator
{
    private static readonly Random _random = new Random();
    private static readonly string _characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static string GenerateRandomString(int length = 4)
    {
        if (length <= 0) throw new ArgumentException("Length must be greater than 0", nameof(length));

        char[] result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = _characters[_random.Next(_characters.Length)];
        }

        return new string(result);
    }
}
