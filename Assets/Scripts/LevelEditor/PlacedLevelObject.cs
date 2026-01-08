using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacedLevelObject : MonoBehaviour, ILevelEditable
{
    public string objectTypeId;
    
    [SerializeField]
    private GameObject myBorderSpriteObj;
    [SerializeField]
    private GameObject myMainObject;
    public GameObject MyMainObject => myMainObject;
    
    private SpriteRenderer borderSpriteRenderer;

    void Awake()
    {
        if (myBorderSpriteObj != null)
        {
            borderSpriteRenderer = myBorderSpriteObj.GetComponent<SpriteRenderer>();
        
            borderSpriteRenderer.drawMode = SpriteDrawMode.Tiled;
            borderSpriteRenderer.tileMode = SpriteTileMode.Continuous;
            myBorderSpriteObj.SetActive(false);
        }

        UpdateBorderSelectionBox();
    }

    public void SetSelected(bool selected) => myBorderSpriteObj.SetActive(selected);

    public void NotifyTransformEdited() => UpdateBorderSelectionBox();
    

    public BaseObjectInstanceData ToInstanceData(LevelObjectRegistry registry)
    {
        var t = transform;
        var data = new BaseObjectInstanceData
        {
            objectTypeId = objectTypeId,
            position = t.position,
            rotationZ = t.eulerAngles.z,
            scale = t.localScale
        };

        var editables = GetComponents<ILevelEditable>();
        foreach (var ed in editables)
            ed.WriteToLevelData(data);

        return data;
    }

    public void ApplyFromInstanceData(BaseObjectInstanceData data, LevelObjectRegistry registry)
    {
        transform.position = data.position;
        transform.rotation = Quaternion.Euler(0, 0, data.rotationZ);
        myMainObject.transform.localScale = data.scale;

        UpdateBorderSelectionBox();

        var editables = GetComponents<ILevelEditable>();
        foreach (var ed in editables)
            ed.ReadFromLevelData(data);
    }

    private void UpdateBorderSelectionBox()
    {
        if(!myMainObject.TryGetComponent(out SpriteRenderer renderer))
            renderer = myMainObject.GetComponentInChildren<SpriteRenderer>();

        TryGetComponent(out Collider2D collider);

        if (myBorderSpriteObj != null)
        {
            myBorderSpriteObj.transform.position = transform.position;
            borderSpriteRenderer.size = renderer ? renderer.bounds.size : (collider ? collider.bounds.size : new Vector2(1, 1));
        }
    }

    public string GetSectionTitle() => "Transform";

    public void GetProperties(List<LevelPropertyDescriptor> output)
    {
        var t = myMainObject.transform;

        output.Add(new LevelPropertyDescriptor
        {
            ownerId = "Transform",
            id = "pos",
            label = "Position",
            type = LevelPropertyType.Vector2,
            vector2Value = transform.position
        });

        output.Add(new LevelPropertyDescriptor
        {
            ownerId = "Transform",
            id = "rotZ",
            label = "Rotation Z",
            type = LevelPropertyType.Float,
            floatValue = transform.eulerAngles.z
        });

        output.Add(new LevelPropertyDescriptor
        {
            ownerId = "Transform",
            id = "scale",
            label = "Scale",
            type = LevelPropertyType.Vector2,
            vector2Value = t.localScale
        });
    }

    public void ApplyProperty(LevelPropertyDescriptor property)
    {
        var t = myMainObject.transform;

        switch (property.id)
        {
            case "pos":
                transform.position = property.vector2Value;
                break;
            case "rotZ":
                transform.rotation = Quaternion.Euler(0f, 0f, property.floatValue);
                break;
            case "scale":
                t.localScale = (Vector3)property.vector2Value + Vector3.forward;
                break;
        }

        UpdateBorderSelectionBox();
    }

    public void WriteToLevelData(BaseObjectInstanceData data)
    {
        data.position = transform.position;
        data.rotationZ = transform.eulerAngles.z;
        data.scale = myMainObject.transform.localScale;
    }

    public void ReadFromLevelData(BaseObjectInstanceData data)
    {
        transform.position = data.position;
        transform.rotation = Quaternion.Euler(0, 0, data.rotationZ);
        myMainObject.transform.localScale = data.scale;

        UpdateBorderSelectionBox();
    }
}