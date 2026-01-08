using UnityEngine;
public enum LevelPropertyType
{
    Float,
    Int,
    Bool,
    String,
    Vector2,
    Vector3,
    Enum
}

[System.Serializable]
public class LevelPropertyDescriptor
{
    public string ownerId;
    public string id;
    public string label;

    public LevelPropertyType type;

    public float floatValue;
    public int intValue;
    public bool boolValue;
    public string stringValue;
    public Vector2 vector2Value;
    public Vector3 vector3Value;

    public string[] enumOptions;
    public int enumIndex;
}