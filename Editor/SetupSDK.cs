using UnityEngine;
using UnityEditor;
using System.IO;
#if UNITY_EDITOR
public class SetupSDK : MonoBehaviour
{
    private static string destinationFolder = "Assets/Plugins/Android";
    private static string packageCacheFolder = "Library/PackageCache";
    private static string packageName = "com.callerid.calleridsdk";
    private static string packageSourceFolder = "Runtime/Android";


    [MenuItem("CallerID/Setup SDK")]
    public static void CopyFiles()
    {
        Copy();
        AssetDatabase.Refresh();
    }

    private static string GetSourceFolderFullPath()
    {
        //string packagesPath = Application.dataPath.Replace("Assets", packageCacheFolder);
        string packagesPath = packageCacheFolder;
        if (Directory.Exists(packagesPath))
        {
            string[] packageFolders = Directory.GetDirectories(packagesPath);
            
            //Debug.Log("Available package folders:");

            foreach (string folder in packageFolders)
            {
                //Debug.Log(folder);
                if (folder.Contains(packageName))
                {
                    return folder.Replace("\\", "/") + "/" + packageSourceFolder;
                }
            }
        }
        else
        {
            Debug.LogError("Packages folder not found.");
        }
        return string.Empty;
    }

    private static string GetDestinationFolderFullPath()
    {
        return destinationFolder;
    }

    private static void Copy()
    {
        var sourcePath = GetSourceFolderFullPath();
        var destDir = GetDestinationFolderFullPath();
        // Перевіряємо, чи існує папка призначення
        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }
        if (Directory.Exists(sourcePath))
        {
            // Отримуємо список файлів у вихідній папці
            string[] files = Directory.GetFiles(sourcePath);
            // Копіюємо кожен файл
            foreach (string file in files)
            {
                Debug.Log(file);
                // Отримуємо ім'я файлу
                string fileName = Path.GetFileName(file);

                // Копіюємо файл в папку призначення
                string destFilePath = Path.Combine(destDir, fileName);
                FileUtil.CopyFileOrDirectory(file, destFilePath);
            }
        }
    }
}
#endif