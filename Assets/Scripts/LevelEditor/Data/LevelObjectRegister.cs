using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelEditor/LevelObjectRegistry")]
public class LevelObjectRegistry : ScriptableObject
{
    [Serializable]
    public class ObjectEntry
    {
        public string id;
        public LevelObjects levelObjects;
    }

    [Serializable]
    public class LevelObjects
    {
        public GameObject previewPrefab;
        public GameObject inGamePrefab;
        public GameObject editorPrefab;
    }

    [Serializable]
    public class PhysicsMaterialEntry
    {
        public string id;
        public PhysicsMaterial2D material;
    }

    public List<ObjectEntry> objects = new List<ObjectEntry>();
    public List<PhysicsMaterialEntry> physicsMaterials = new List<PhysicsMaterialEntry>();

    private Dictionary<string, LevelObjects> objectLookup;
    private Dictionary<string, PhysicsMaterial2D> materialLookup;

    void OnEnable()
    {
        objectLookup = new Dictionary<string, LevelObjects>();
        foreach (var e in objects)
            objectLookup[e.id] = e.levelObjects;

        materialLookup = new Dictionary<string, PhysicsMaterial2D>();
        foreach (var e in physicsMaterials)
            materialLookup[e.id] = e.material;
    }

    public LevelObjects GetAllPrefabs(string id)
    {
        objectLookup.TryGetValue(id, out var prefab);
        return prefab;
    }

    public PhysicsMaterial2D GetMaterial(string id)
    {
        materialLookup.TryGetValue(id, out var mat);
        return mat;
    }

    public string GetMaterialId(PhysicsMaterial2D mat)
    {
        foreach (var e in physicsMaterials)
            if (e.material == mat)
                return e.id;
        return null;
    }
}
