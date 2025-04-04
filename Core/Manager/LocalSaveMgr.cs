using UnityEngine;
using System;
using System.Text;

public static class LocalSaveMgr
{
    private static string Encrypt(string plainText)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(bytes);
    }

    private static string Decrypt(string encodedText)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(encodedText);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            Debug.LogWarning("복호화 실패. 기본값을 반환합니다.");
            return "";
        }
    }

    public static void SaveInt(string key, int value)
    {
        SaveString(key, value.ToString());
    }

    public static int LoadInt(string key, int defaultValue = 0)
    {
        string decrypted = LoadString(key);
        if (int.TryParse(decrypted, out int result))
            return result;

        return defaultValue;
    }

    public static void SaveFloat(string key, float value)
    {
        SaveString(key, value.ToString());
    }

    public static float LoadFloat(string key, float defaultValue = 0f)
    {
        string decrypted = LoadString(key);
        if (float.TryParse(decrypted, out float result))
            return result;

        return defaultValue;
    }

    public static void SaveString(string key, string value)
    {
        string encrypted = Encrypt(value);
        PlayerPrefs.SetString(key, encrypted);
        PlayerPrefs.Save();
    }

    public static string LoadString(string key, string defaultValue = "")
    {
        if (!PlayerPrefs.HasKey(key)) return defaultValue;

        string encrypted = PlayerPrefs.GetString(key);
        string decrypted = Decrypt(encrypted);
        return string.IsNullOrEmpty(decrypted) ? defaultValue : decrypted;
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public static void ClearAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
