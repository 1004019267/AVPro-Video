using UnityEngine;
using System.Collections;
using System;
using System.Security.Cryptography;
using System.Text;
public struct RijndaelKey
{
    public byte[] key;
    public byte[] IV;

    public RijndaelKey(byte[] key, byte[] iV)
    {
        this.key = key;
        IV = iV;
    }
}

public class Rijndael : MonoBehaviour
{
    /// <summary>
    /// Rijndael加密
    /// </summary>
    /// <param name="plainData">密文</param>
    /// <param name="key">密钥</param>
    /// <param name="IV">向量</param>
    /// <returns></returns>
    public byte[] Encrypt(byte[] plainData, byte[] key, byte[] IV)
    {
        if (plainData == null || plainData.Length <= 0)
            throw new ArgumentNullException("video");
        if (key == null || key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        byte[] encrypted;
        //创建RijndaelManaged对象
        //使用指定的键和IV。
        //用完就会被释放
        using (RijndaelManaged rijAlg = new RijndaelManaged())
        {
            rijAlg.Key = key;
            rijAlg.IV = IV;
            //设置对称算法的运算模式
            rijAlg.Mode = CipherMode.ECB;
            //设置填充模式
            rijAlg.Padding = PaddingMode.ISO10126;

            //创建加密程序以执行流转换。
            ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
            encrypted = encryptor.TransformFinalBlock(plainData, 0, plainData.Length);
        }
        //从内存流返回加密的字节。
        return encrypted;
    }
    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="cipherData"></param>
    /// <param name="key"></param>
    /// <param name="IV"></param>
    /// <returns></returns>
    public byte[] Decrypt(byte[] cipherData, byte[] key, byte[] IV)
    {
        if (cipherData == null || cipherData.Length <= 0)
            throw new ArgumentNullException("video");
        if (key == null || key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        byte[] decrypted;
        using (RijndaelManaged rijAlg = new RijndaelManaged())
        {
            rijAlg.Key = key;
            rijAlg.IV = IV;
            rijAlg.Mode = CipherMode.ECB;
            rijAlg.Padding = PaddingMode.ISO10126;
            //创建解密程序以执行流转换。
            ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
            decrypted = decryptor.TransformFinalBlock(cipherData, 0, cipherData.Length);
        }
        return decrypted;
    }

    /// <summary>
    /// 创建Key 和IV在某个目录下
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public RijndaelKey CreateKeyAndIV(string path)
    {
        RijndaelManaged rijma = new RijndaelManaged();
        rijma.GenerateKey();
        rijma.GenerateIV();
        FileTools.CreateFile(path + "/keyTxt.txt", rijma.Key);
        FileTools.CreateFile(path + "/IVTxt.txt", rijma.IV);

        return new RijndaelKey(rijma.Key, rijma.IV);
    }

    /// <summary>
    /// 获取Key 和IV
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public RijndaelKey GetKeyAndIV(string path)
    {
        byte[] key = FileTools.ReadFile(path + "/keyTxt.txt");
        byte[] IV = FileTools.ReadFile(path + "/IVTxt.txt");
        return new RijndaelKey(key, IV);
    }
}

