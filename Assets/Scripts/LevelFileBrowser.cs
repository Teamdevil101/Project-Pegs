using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelFileBrowser : MonoBehaviour
{
    public string levelsFolderName = "Levels";
    public GameObject loadFileButtonPrefab;
    public Transform detectedLevelsContainer;
    public GameObject levelFileBrowserObject;

    public void Start()
    {
        if(levelFileBrowserObject.activeSelf)
            levelFileBrowserObject.SetActive(false);
    }

    public void LoadLevelFiles()
    {
        levelFileBrowserObject.SetActive(true);

        var listOfDetectedLevels = GetAllCustomLevelFiles();

        foreach (var level in listOfDetectedLevels)
        {
            GameObject _go = Instantiate(loadFileButtonPrefab, detectedLevelsContainer);
            LoadLevelButton llb = _go.GetComponent<LoadLevelButton>();

            FileInfo ff = new FileInfo(level);
            var json = File.ReadAllText(level); // Not the best but it works.
            LevelData ld = JsonUtility.FromJson<LevelData>(json);
            string time = ff.LastWriteTime.ToShortDateString() + " " + ff.LastWriteTime.ToShortTimeString();

            llb.SetElements(ld.levelName, level, time);
            llb.button.onClick.AddListener(() => { LevelEditorUI.Instance.OnLoadLevelButton(level); UnloadLevelFiles(); });
        }
    }

    public void UnloadLevelFiles()
    {
        foreach (Transform obj in detectedLevelsContainer)
        {
            if(obj.TryGetComponent(out LoadLevelButton llb))
                llb.button.onClick.RemoveAllListeners();

            Destroy(obj.gameObject);
        }

        levelFileBrowserObject.SetActive(false);
    }

    public string DefaultLevelsFolderPath
    {
        get
        {
            string path = Path.Combine(Application.dataPath, levelsFolderName);

            /*try
            {*/
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
/*            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException || ex is DirectoryNotFoundException)
                {
                    path = Path.Combine(Application.persistentDataPath, levelsFolderName);

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
                else
                    Debug.LogError(ex);
            }*/

            return path;
        }
    }

    public string CustomLevelsFolderPath
    {
        get
        {
            string path = Path.Combine(Application.dataPath, levelsFolderName, "Custom");

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException || ex is DirectoryNotFoundException)
                {
                    path = Path.Combine(Application.persistentDataPath, levelsFolderName);

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
                else
                    Debug.LogError(ex);
            }

            Debug.Log($"My current custom path is {path}");
            return path;
        }
    }

    public List<string> GetAllCustomLevelFiles()
    {
        List<string> allFiles = new List<string>();

        var path1 = Path.Combine(Application.persistentDataPath, levelsFolderName);
        var path2 = Path.Combine(Application.dataPath, levelsFolderName, "Custom");

        try {
            allFiles.AddRange(Directory.GetFiles(path1, "*.json"));
        } 
        catch (DirectoryNotFoundException ex) {
            Debug.Log($"No directory found in persistentDataPath...");
        }

        try
        {
            allFiles.AddRange(Directory.GetFiles(path2, "*.json"));
        }
        catch (DirectoryNotFoundException ex)
        {
            Debug.LogWarning($"No directory found in base game files!");
        }

        return allFiles;
        }

    public List<string> GetAllDefaultLevelFiles()
    {
        var path = DefaultLevelsFolderPath;
        var files = Directory.GetFiles(path, "*.json");
        return new List<string>(files);
    }

    public string GetFullPath(string fileNameWithoutExtension)
    {
        return GetFullPath(CustomLevelsFolderPath, fileNameWithoutExtension);
    }

    public string GetFullPath(string finalLevelPath, string fileNameWithoutExtension)
    {
        return Path.Combine(finalLevelPath, fileNameWithoutExtension + ".json");
    }
}