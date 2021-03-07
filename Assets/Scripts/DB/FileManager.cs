using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class FileManager/*: MonoBehaviour*/
{
    private static string target;
    static string Target
    {
        get
        {
            if (string.IsNullOrEmpty(target))
            {
                target = Application.persistentDataPath + "/";
            }
            return target;
        }
    }
    /*void Start(){
Debug.Log (Application.persistentDataPath + "/participantsData/");
}*/
    public static string LoadFile(string fileName)
    {
        string targetFile = Target + fileName;
        FileStream fileStream;
        if (File.Exists(targetFile)) fileStream = File.OpenRead(targetFile);
        else return "";
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string fileContent = (string)binaryFormatter.Deserialize(fileStream);
        fileStream.Dispose();
        fileStream.Close();
        return fileContent;
    }
    public static void SaveFile(string fileName, string fileContent)
    {
        string targetFile = Target + fileName;
        FileStream fileStream;
        if (File.Exists(targetFile)) fileStream = File.OpenWrite(targetFile);
        else fileStream = File.Create(targetFile);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, fileContent);
        fileStream.Dispose();
        fileStream.Close();
    }
}