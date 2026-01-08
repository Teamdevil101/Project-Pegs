using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelNameDialog : MonoBehaviour
{
    public TMP_InputField levelNameField;
    public TMP_InputField fileNameField;
    public Toggle sameAsLevelNameToggle;

    private Action<string, string> onConfirm;
    private Action onCancel;

    private void Start()
    {
        Hide();
    }

    public void Show(string defaultLevelName, string defaultFileName, Action<string, string> onConfirm, Action onCancel = null)
    {
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;

        gameObject.SetActive(true);
        
        fileNameField.interactable = !sameAsLevelNameToggle.isOn;

        levelNameField.text = defaultLevelName;
        fileNameField.text = defaultFileName;

        levelNameField.onValueChanged.RemoveListener(OnLevelNameChanged);
        levelNameField.onValueChanged.AddListener(OnLevelNameChanged);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        onConfirm = null;
        onCancel = null;
    }

    void OnLevelNameChanged(string newText)
    {
        if (sameAsLevelNameToggle.isOn)
        {
            fileNameField.text = MakeSafeFileName(newText.Truncate(60));
        }
    }

    public void OnSameNameSwitchToggled(bool toggle)
    {
        fileNameField.interactable = !toggle;
    }    

    public void OnClickOk()
    {
        var levelName = string.IsNullOrWhiteSpace(levelNameField.text)
            ? "Untitled"
            : levelNameField.text.Trim();

        var fileName = string.IsNullOrWhiteSpace(fileNameField.text)
            ? MakeSafeFileName(levelName.Truncate(60))
            : MakeSafeFileName(fileNameField.text);
        
        if(sameAsLevelNameToggle.isOn)
            fileName = MakeSafeFileName(levelName.Truncate(60));

        onConfirm?.Invoke(levelName, fileName);
        Hide();
    }

    public void OnClickCancel()
    {
        onCancel?.Invoke();
        Hide();
    }

    static string MakeSafeFileName(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "Untitled";

        raw = raw.Trim();
        foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            raw = raw.Replace(c.ToString(), "");

        raw = raw.Replace(' ', '_');
        raw = Regex.Replace(raw, @"\p{Cs}", ""); // Remove most unicode characters from filename

        return raw.Length == 0 ? "Untitled" : raw;
    }
}