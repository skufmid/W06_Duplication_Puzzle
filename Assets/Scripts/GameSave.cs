using UnityEngine;
using System;
using System.Text;

public static class GameSave
{
    private static readonly string encryptionKey = "BWYr5GvGB2Gx"; // 원하는 키로 바꾸세요

    public static void SetEncryptedString(string key, string value)
    {
        string encrypted = Encrypt(value);
        PlayerPrefs.SetString(key, encrypted);
    }

    public static string GetEncryptedString(string key)
    {
        if (!PlayerPrefs.HasKey(key)) return null;

        string encrypted = PlayerPrefs.GetString(key);
        return Decrypt(encrypted);
    }

    private static string Encrypt(string plainText)
    {
        byte[] data = Encoding.UTF8.GetBytes(plainText);
        byte[] key = Encoding.UTF8.GetBytes(encryptionKey);
        for (int i = 0; i < data.Length; i++)
            data[i] ^= key[i % key.Length]; // XOR

        return Convert.ToBase64String(data);
    }

    private static string Decrypt(string encryptedText)
    {
        byte[] data = Convert.FromBase64String(encryptedText);
        byte[] key = Encoding.UTF8.GetBytes(encryptionKey);
        for (int i = 0; i < data.Length; i++)
            data[i] ^= key[i % key.Length]; // XOR

        return Encoding.UTF8.GetString(data);
    }
}
