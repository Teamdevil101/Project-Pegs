using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPropertyRow : MonoBehaviour
{
    public TMP_Text labelText;
    public Transform valueRoot;

    public GameObject inputFieldObject;
    public GameObject vector3InputFieldObject;
    public GameObject vector2InputFieldObject;
    public GameObject numberInputFieldObject;
    public GameObject enumDropdownObject;
    public GameObject toggleObject;

    private LevelPropertyDescriptor descriptor;
    private Action<LevelPropertyDescriptor> onChanged;
    private bool suppressEvents;

    private TMP_InputField currentInput;
    private Toggle currentToggle;
    private TMP_Dropdown currentDropdown;
    private CustomVector2InputField currentVector2;
    private CustomVector3InputField currentVector3;

    private Vector2 previousVector2;
    private Vector3 previousVector3;

    public void Setup(LevelPropertyDescriptor desc, Action<LevelPropertyDescriptor> onChangedCallback)
    {
        descriptor = desc;
        onChanged = onChangedCallback;

        suppressEvents = true;

        ClearValueRoot();

        switch (desc.type)
        {
            case LevelPropertyType.Float:
            case LevelPropertyType.Int:
                SetupNumInputField(desc);
                break;
            case LevelPropertyType.String:
                SetupInputField(desc);
                break;

            case LevelPropertyType.Bool:
                SetupBool(desc);
                break;

            case LevelPropertyType.Vector2:
                SetupVector2(desc);
                break;

            case LevelPropertyType.Vector3:
                //SetupVector3(desc);
                break;

            case LevelPropertyType.Enum:
                SetupEnum(desc);
                break;
            default:
                break;
        }

        suppressEvents = false;


        if (labelText == null)
            labelText = valueRoot.GetComponentInChildren<TMP_Text>();

        labelText.text = desc.label;
    }

    public void Refresh(LevelPropertyDescriptor desc)
    {
        if (desc == null) return;

        // If the property "shape" changed, easiest is to rebuild the row UI.
        // (Type changes are rare, but this keeps things correct.)
        bool mustRebuild =
            descriptor == null ||
            descriptor.type != desc.type;

        // If enums can change options at runtime, rebuild when options list changes.
        if (!mustRebuild && desc.type == LevelPropertyType.Enum)
        {
            // Compare length + contents (cheap enough for small enums)
            if (descriptor.enumOptions == null || desc.enumOptions == null ||
                descriptor.enumOptions.Length != desc.enumOptions.Length)
            {
                mustRebuild = true;
            }
            else
            {
                for (int i = 0; i < desc.enumOptions.Length; i++)
                {
                    if (descriptor.enumOptions[i] != desc.enumOptions[i])
                    {
                        mustRebuild = true;
                        break;
                    }
                }
            }
        }

        // Update our stored descriptor reference first
        descriptor = desc;

        suppressEvents = true;

        if (mustRebuild)
        {
            // Preserve callback, rebuild controls
            ClearValueRoot();

            switch (desc.type)
            {
                case LevelPropertyType.Float:
                case LevelPropertyType.Int:
                    SetupNumInputField(desc);
                    break;

                case LevelPropertyType.String:
                    SetupInputField(desc);
                    break;

                case LevelPropertyType.Bool:
                    SetupBool(desc);
                    break;

                case LevelPropertyType.Vector2:
                    SetupVector2(desc);
                    break;

                case LevelPropertyType.Vector3:
                    // SetupVector3(desc); // currently disabled in your code
                    break;

                case LevelPropertyType.Enum:
                    SetupEnum(desc);
                    break;
            }
        }
        else
        {
            // Same control type already exists - just push new values into it.
            switch (desc.type)
            {
                case LevelPropertyType.Float:
                    if (currentInput != null)
                        currentInput.text = desc.floatValue.ToString("0.###");
                    break;

                case LevelPropertyType.Int:
                    if (currentInput != null)
                        currentInput.text = desc.intValue.ToString();
                    break;

                case LevelPropertyType.String:
                    if (currentInput != null)
                        currentInput.text = desc.stringValue ?? "";
                    break;

                case LevelPropertyType.Bool:
                    if (currentToggle != null)
                        currentToggle.isOn = desc.boolValue;
                    break;

                case LevelPropertyType.Vector2:
                    if (currentVector2 != null)
                    {
                        currentVector2.SetValue(desc.vector2Value);
                        previousVector2 = desc.vector2Value; // keep rollback base sane
                    }
                    break;

                case LevelPropertyType.Vector3:
                    // If you re-enable vector3 later:
                    // if (currentVector3 != null) currentVector3.SetValue(desc.vector3Value);
                    break;

                case LevelPropertyType.Enum:
                    if (currentDropdown != null)
                    {
                        // Options are assumed unchanged here due to mustRebuild check.
                        currentDropdown.value = Mathf.Clamp(desc.enumIndex, 0, Math.Max(0, desc.enumOptions.Length - 1));
                        currentDropdown.RefreshShownValue();
                    }
                    break;
            }
        }

        // Label might change (eg localization or dynamic naming)
        if (labelText == null)
            labelText = valueRoot.GetComponentInChildren<TMP_Text>();
        if (labelText != null)
            labelText.text = desc.label;

        suppressEvents = false;
    }

    void ClearValueRoot()
    {
        foreach (Transform child in valueRoot)
            DestroyImmediate(child.gameObject);

        currentInput = null;
        currentToggle = null;
        currentDropdown = null;
        currentVector2 = null;
        currentVector3 = null;
    }

    void SetupNumInputField(LevelPropertyDescriptor desc)
    {
        var go = Instantiate(numberInputFieldObject, valueRoot);
        currentInput = go.GetComponentInChildren<TMP_InputField>();

        if (currentInput == null)
        {
            Debug.LogError("inputFieldPrefab must have TMP_InputField");
            return;
        }

        switch (desc.type)
        {
            case LevelPropertyType.Float:
                currentInput.contentType = TMP_InputField.ContentType.DecimalNumber;
                currentInput.text = desc.floatValue.ToString("0.###");
                break;

            case LevelPropertyType.Int:
                currentInput.contentType = TMP_InputField.ContentType.IntegerNumber;
                currentInput.text = desc.intValue.ToString();
                break;
        }

        currentInput.onEndEdit.AddListener(OnInputFieldEndEdit);
        currentInput.onEndEdit.AddListener(OnEndEdit);
    }

    void SetupInputField(LevelPropertyDescriptor desc)
    {
        var go = Instantiate(inputFieldObject, valueRoot);
        currentInput = go.GetComponentInChildren<TMP_InputField>();

        if (currentInput == null)
        {
            Debug.LogError("inputFieldPrefab must have TMP_InputField");
            return;
        }

        currentInput.contentType = TMP_InputField.ContentType.Standard;
        currentInput.text = desc.stringValue ?? "";

        currentInput.onEndEdit.AddListener(OnInputFieldEndEdit);
        currentInput.onEndEdit.AddListener(OnEndEdit);
    }

    void SetupBool(LevelPropertyDescriptor desc)
    {
        var go = Instantiate(toggleObject, valueRoot);
        currentToggle = go.GetComponentInChildren<Toggle>();

        if (currentToggle == null)
        {
            Debug.LogError("boolTogglePrefab must have Toggle");
            return;
        }

        currentToggle.isOn = desc.boolValue;
        currentToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void SetupVector2(LevelPropertyDescriptor desc)
    {
        var go = Instantiate(vector2InputFieldObject, valueRoot);
        currentVector2 = go.GetComponentInChildren<CustomVector2InputField>();

        if (currentVector2 == null)
        {
            Debug.LogError("vector2FieldPrefab must have CustomVector2InputField");
            return;
        }

        currentVector2.SetValue(desc.vector2Value);
        
        currentVector2.AddListener(OnVector2Changed, OnEndEdit, OnVector2Selected);
    }

    /*void SetupVector3(LevelPropertyDescriptor desc)
    {
        var go = Instantiate(vector3InputFieldObject, valueRoot);
        currentVector3 = go.GetComponentInChildren<CustomVector3InputField>();

        if (currentVector3 == null)
        {
            Debug.LogError("vector3FieldPrefab must have CustomVector3InputField");
            return;
        }

        currentVector3.SetValue(desc.vector3Value);
        currentVector3.x_inputField.onValueChanged.AddListener(OnVector3Changed);
        currentVector3.y_inputField.onValueChanged.AddListener(OnVector3Changed);
        currentVector3.z_inputField.onValueChanged.AddListener(OnVector3Changed);
    }*/

    void SetupEnum(LevelPropertyDescriptor desc)
    {
        var go = Instantiate(enumDropdownObject, valueRoot);
        currentDropdown = go.GetComponentInChildren<TMP_Dropdown>();

        if (currentDropdown == null)
        {
            Debug.LogError("enumDropdownPrefab must have TMP_Dropdown");
            return;
        }

        currentDropdown.ClearOptions();
        currentDropdown.AddOptions(new System.Collections.Generic.List<string>(desc.enumOptions));
        currentDropdown.value = Mathf.Clamp(desc.enumIndex, 0, Math.Max(0, desc.enumOptions.Length - 1));
        currentDropdown.RefreshShownValue();

        currentDropdown.onValueChanged.AddListener(OnEnumChanged);
    }

    void OnInputFieldEndEdit(string text)
    {
        if (suppressEvents || descriptor == null) return;

        switch (descriptor.type)
        {
            case LevelPropertyType.Float:
                if (float.TryParse(text, out var f))
                    descriptor.floatValue = f;
                break;

            case LevelPropertyType.Int:
                if (int.TryParse(text, out var i))
                    descriptor.intValue = i;
                break;

            case LevelPropertyType.String:
                descriptor.stringValue = text;
                break;
        }

        onChanged?.Invoke(descriptor);
    }

    void OnToggleChanged(bool value)
    {
        if (suppressEvents || descriptor == null) return;
        descriptor.boolValue = value;
        onChanged?.Invoke(descriptor);
    }

    void OnEnumChanged(int index)
    {
        if (suppressEvents || descriptor == null) return;
        descriptor.enumIndex = index;
        onChanged?.Invoke(descriptor);
    }

    void OnVector2Changed(string _)
    {
        if (suppressEvents || descriptor == null) return;

        string vectorX = currentVector2.x_inputField.text;
        string vectorY = currentVector2.y_inputField.text;

        if (string.IsNullOrEmpty(vectorX) || vectorX == "-" ||
            string.IsNullOrEmpty(vectorY) || vectorY == "-" || 
            !ExpressionEvaluator.TryEvaluate(vectorX, out float xVal) ||
            !ExpressionEvaluator.TryEvaluate(vectorY, out float yVal))
        {
            descriptor.vector2Value = previousVector2;
            onChanged?.Invoke(descriptor);
            return;
        }

        descriptor.vector2Value = new Vector2(xVal, yVal);
        onChanged?.Invoke(descriptor);
    }

    void OnVector2Selected(string _)
    {
        previousVector2 = descriptor.vector2Value;
    }

    void OnEndEdit(string _)
    {
        switch (descriptor.type)
        {
            case LevelPropertyType.Float:
                currentInput.text = descriptor.floatValue.ToString("0.###");
                break;

            case LevelPropertyType.Int:
                currentInput.text = descriptor.intValue.ToString();
                break;
            case LevelPropertyType.String:
                currentInput.text = descriptor.stringValue ?? "";
                break;

            case LevelPropertyType.Vector2:
                string vectorX = currentVector2.x_inputField.text;
                string vectorY = currentVector2.y_inputField.text;

                if (string.IsNullOrEmpty(vectorX) || vectorX == "-" ||
                    string.IsNullOrEmpty(vectorY) || vectorY == "-" ||
                    !ExpressionEvaluator.TryEvaluate(vectorX, out float xVal) ||
                    !ExpressionEvaluator.TryEvaluate(vectorY, out float yVal))
                {
                    descriptor.vector2Value = previousVector2;
                    onChanged?.Invoke(descriptor);
                }

                currentVector2.SetValue(descriptor.vector2Value);
                break;

            case LevelPropertyType.Vector3:
                //SetupVector3(desc);
                break;
            default:
                break;
        }
    }

    /*void OnVector3Changed(string _)
    {
        string vectorString = $"{currentVector3.x_inputField.text},{currentVector3.y_inputField.text},{currentVector3.z_inputField.text}";

        if (suppressEvents || descriptor == null) return;

        if (TransformExtentions.TryParseVector3(vectorString, out Vector3 value))
            descriptor.vector3Value = value;
        else
            return;

        onChanged?.Invoke(descriptor);
    }*/
}