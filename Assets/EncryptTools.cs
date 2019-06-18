using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class EncryptTools
{
    /// <summary>
    /// 复制一份加密的出来
    /// </summary>
    /// <param name="cmd"></param>
    [MenuItem("Tools/EncryptVideo")]
    static void EncryptVideo(MenuCommand cmd)
    {
        //获取选择文件夹的路径
        string[] str = Selection.assetGUIDs;
        string path = AssetDatabase.GUIDToAssetPath(str[0]);
        string newPath = Application.dataPath + "/StreamingAssets/videoNew";
        FileTools.CreateDirectory(newPath);

        //获取指定路径下的指定类型资源
        DirectoryInfo root = new DirectoryInfo(path);
        FileInfo[] files = root.GetFiles("*.mp4");
        Rijndael rij = new Rijndael();
        RijndaelKey rijKey = rij.CreateKeyAndIV(newPath);

        for (int i = 0; i < files.Length; i++)
        {
            byte[] enBytes = rij.Encrypt(FileTools.ReadFile(files[i].FullName), rijKey.key, rijKey.IV);

            string strWriteFile = newPath + "/" + files[i].Name;
            FileTools.CreateFile(strWriteFile, enBytes);
         
            EditorUtility.DisplayProgressBar("进度", i + "/" + files.Length + "完成修改值", (float)i / files.Length);
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// C#获取指定目录下多种指定类型文件
    /// </summary>
    /// <param name="filePath">路径</param>
    /// <returns></returns>
    static List<string> GetVideoFiles(string filePath)
    {
        DirectoryInfo di = new DirectoryInfo(filePath);
        FileInfo[] files = di.GetFiles();
        string fileName;
        List<string> list = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            fileName = files[i].Name.ToLower();
            if (fileName.EndsWith(".mp4") || fileName.EndsWith(".avi"))
            {
                list.Add(fileName);
            }
        }
        return list;
    }
}
