using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelEditorUI : MonoBehaviour
{
    public static LevelEditorUI Instance { get; set; }

    public LevelEditorController editor;
    public LevelFileBrowser fileBrowser;
    public LevelNameDialog levelNameDialog;

    public Button saveButton;
    public Button saveAsButton;

    public TMP_InputField levelNameField;

    public Toggle snapToggle;
    public TMP_InputField snapPosField;
    public TMP_InputField snapRotField;

    private string currentFilePath;
    private bool isLoading;

    void Awake()
    {
        if(Instance)
            Destroy(gameObject);
        else
            Instance = this;

        
        saveButton.interactable = false;
        
        if (editor != null)
            editor.OnLevelChanged += HandleLevelChanged;
    }

    void OnDestroy()
    {
        if (editor != null)
            editor.OnLevelChanged -= HandleLevelChanged;
    }

    void HandleLevelChanged() => saveButton.interactable = true;

    void UpdateLevelLabel()
    {
        if (levelNameField != null)
        {
            string name = string.IsNullOrEmpty(editor.CurrentLevelName)
                ? "Untitled"
                : editor.CurrentLevelName;
            levelNameField.text = name;
        }
    }

    public void OnLevelTitleChanged(string levelName)
    {
        if(isLoading) return;

        editor.CurrentLevelName = levelName;
        saveButton.interactable = true;
    }

    public void OnNewLevelButton()
    {
        levelNameDialog.Show(
            defaultLevelName: "NewLevel",
            defaultFileName: "NewLevel",
            onConfirm: (levelName, fileName) =>
            {
                var data = new LevelData
                {
                    levelName = levelName
                };

                editor.SetLevelState(data);
                currentFilePath = fileBrowser.GetFullPath(fileName);
                saveButton.interactable = false;

                UpdateLevelLabel();
            },
            onCancel: null
        );
    }

    public void OnSaveButton()
    {
        // If we have no file path yet, treat Save as Save As
        if (string.IsNullOrEmpty(currentFilePath))
        {
            OnSaveAsButton();
            return;
        }

        SaveToCurrentPath();
    }

    public void OnSaveAsButton()
    {
        // Pre-fill with current level name and file name if any
        string currentName = string.IsNullOrEmpty(editor.CurrentLevelName)
            ? "Untitled"
            : editor.CurrentLevelName;

        string fileNameNoExt = "Untitled";
        if (!string.IsNullOrEmpty(currentFilePath))
        {
            fileNameNoExt = System.IO.Path.GetFileNameWithoutExtension(currentFilePath);
        }
        else
        {
            fileNameNoExt = currentName;
        }

        levelNameDialog.Show(
            defaultLevelName: currentName,
            defaultFileName: fileNameNoExt,
            onConfirm: (levelName, fileName) =>
            {
                editor.CurrentLevelName = levelName;
                currentFilePath = fileBrowser.GetFullPath(fileName);

                SaveToCurrentPath();
                UpdateLevelLabel();
            },
            onCancel: null
        );
    }

    public void OnLoadLevelButton(string fullPath)
    {
        isLoading = true;

        var data = LevelSerializer.LoadLevel(fullPath);
        if (data == null)
        {
            Debug.LogWarning($"Failed to load level from {fullPath}");
            return;
        }

        editor.SetLevelState(data);
        currentFilePath = fullPath;
        saveButton.interactable = false;

        UpdateLevelLabel();
        isLoading = false;
    }

    public void OnPosSnapSubmit(string result)
    {
        if (float.TryParse(result, out float amount))
        {
            LevelTransformGizmo.positionSnapStep = amount;
            LevelTransformGizmo.scaleSnapStep = amount;
        }

        snapPosField.text = LevelTransformGizmo.positionSnapStep.ToString();
    }

    public void OnRotSnapSubmit(string result)
    {
        if (float.TryParse(result, out float amount))
        {
            LevelTransformGizmo.rotationSnapStep = amount;
        }

        snapRotField.text = LevelTransformGizmo.rotationSnapStep.ToString();
    }

    public void ToggleSnaps(bool snapToggle)
    {
        LevelTransformGizmo.positionSnapEnabled = snapToggle;
        LevelTransformGizmo.rotationSnapEnabled = snapToggle;
        LevelTransformGizmo.scaleSnapEnabled = snapToggle;
    }

    void SaveToCurrentPath()
    {
        var data = editor.BuildLevelData();
        LevelSerializer.SaveLevel(data, currentFilePath);

        editor.ClearDirtyFlag();
        saveButton.interactable = false;
        Debug.Log("Saved level to: " + currentFilePath);
    }
}