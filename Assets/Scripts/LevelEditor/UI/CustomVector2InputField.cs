using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CustomVector2InputField : MonoBehaviour
{
    public TMP_InputField x_inputField;
    public TMP_InputField y_inputField;

    public void SetValue(Vector2 v2)
    {
        x_inputField.text = v2.x.ToString("0.###");
        y_inputField.text = v2.y.ToString("0.###");
    }

    public void AddListener(UnityAction<string> onValueChanged, UnityAction<string> onEndEdit = null, UnityAction<string> onSelect = null)
    {
        x_inputField.onValueChanged.AddListener(onValueChanged);
        y_inputField.onValueChanged.AddListener(onValueChanged);

        if (onEndEdit != null)
        {
            x_inputField.onEndEdit.AddListener(onEndEdit);
            y_inputField.onEndEdit.AddListener(onEndEdit);
        }

        if (onSelect != null)
        {
            x_inputField.onSelect.AddListener(onSelect);
            y_inputField.onSelect.AddListener(onSelect);
        }
    }
}