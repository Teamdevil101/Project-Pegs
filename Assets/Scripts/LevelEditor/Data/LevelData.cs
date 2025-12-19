using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public string levelName;
    public List<BaseObjectInstanceData> objects = new();
}

[Serializable]
public class BaseObjectInstanceData
{
    public string objectTypeId;
    public Vector2 position;
    public float rotationZ;
    public Vector3 scale = Vector3.one;

    public string physicsMaterialId;
}