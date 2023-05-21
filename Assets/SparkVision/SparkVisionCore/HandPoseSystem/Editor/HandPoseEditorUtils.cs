using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;

namespace SparkVision.HandPoseSystem.EditorUtils
{
    public static class IOUtils
    {
        public static bool SaveScriptableObject<T>(string relativeSaveDirectory, string fileName, T objectInstance)
            where T : ScriptableObject
        {
            bool isValidFileName = fileName.IndexOfAny(Path.GetInvalidFileNameChars()) <= 0;
            
            if(!isValidFileName)
            {
                Debug.LogError($"{fileName} is not a valid file name.");
                return false;
            }


            if(!CreateDataFolder(relativeSaveDirectory))
            {
                Debug.LogError($"{relativeSaveDirectory} is not a valid folder path.");
                return false;
            }

            string assetDirectory = $@"Assets/{relativeSaveDirectory}/{fileName}.asset";
            AssetDatabase.CreateAsset(objectInstance, assetDirectory);
            return true;
        }

        public static bool CreateDataFolder(string relativeFolderDirectory)
        {
            string absolutePath = $@"{Application.dataPath}/{relativeFolderDirectory}";
            bool isValidFolderDirectory = absolutePath.IndexOfAny(Path.GetInvalidPathChars()) <= 0;
            if (!isValidFolderDirectory) return false;
            if (!Directory.Exists(absolutePath))
            {
                Directory.CreateDirectory(absolutePath);
            }
            return true;
        }
    }
}
