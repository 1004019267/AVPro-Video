using System.Collections;
using System.IO;

/// <summary>
/// IO读写类
/// </summary>
public class FileTools
{
    /// <summary>
    /// 创建目录文件夹 有就不创建
    /// </summary>
    public static void CreateDirectory(string filePath)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
        }
    }
    /// <summary>
    /// 创建文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="bytes"></param>
    public static void CreateFile(string filePath, byte[] bytes)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            fs.Write(bytes, 0, bytes.Length);
        }
    }
    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static byte[] ReadFile(string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            byte[] infbytes = new byte[fs.Length];
            fs.Read(infbytes, 0, infbytes.Length);
            return infbytes;
        }
    }

}
