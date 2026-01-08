using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelEditorController : MonoBehaviour
{
    public LevelObjectRegistry registry;
    public Transform levelRoot;
    public Camera sceneCamera;
    public GameObject borderSpritePrefab;
    public Transform borderSpriteObjBin;

    public GameObject defaultPreviewObject;

    public KeyCode continuousPlacementKey = KeyCode.LeftShift;
    public CurrentEditorMode currentEditorMode = CurrentEditorMode.Select;

    public event Action OnLevelChanged;
    public event Action<ILevelEditable> OnEditableChanged;

    public bool IsDirty { get; private set; }

    [Header("Transform Gizmo")]
    public LevelTransformGizmo transformGizmo;

    private string currentObjectTypeId;

    private string currentLevelName;
    private List<PlacedLevelObject> placedObjects = new List<PlacedLevelObject>();
    
    private PlacedLevelObject selectedObject;
    public PlacedLevelObject SelectedObject => selectedObject;

    private GameObject previewObject;

    [SerializeField] private LevelObjectPropertiesWindow propertiesWindow;

    public string CurrentLevelName
    {
        get => currentLevelName;
        set => currentLevelName = value;
    }

    public void MarkEditableChanged(ILevelEditable editable)
    {
        if (editable == null) return;
        OnEditableChanged?.Invoke(editable);
    }

    public void MarkLevelChanged()
    {
        IsDirty = true;
        OnLevelChanged?.Invoke();
    }

    public void ClearDirtyFlag()
    {
        IsDirty = false;
    }

    public void SetLevelState(LevelData data)
    {
        LoadFromLevelData(data);
        currentLevelName = data?.levelName ?? "Untitled";
        ClearDirtyFlag();
    }

    public void SetCurrentObjectType(string objectTypeId)
    {
        currentObjectTypeId = objectTypeId;
    }

    public void StartPlacement()
    {
        if (string.IsNullOrEmpty(currentObjectTypeId)) return;
        currentEditorMode = CurrentEditorMode.Create;

        if(previewObject != null)
            Destroy(previewObject);

        var prefab = registry.GetAllPrefabs(currentObjectTypeId).previewPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("No prefab registered for id: " + currentObjectTypeId);
            previewObject = Instantiate(defaultPreviewObject);
            return;
        }

        previewObject = Instantiate(prefab);
    }

    public void CancelPlacement()
    {
        if (previewObject != null)
            Destroy(previewObject);

        currentEditorMode = CurrentEditorMode.Select;
    }

    void Update()
    {
        if (currentEditorMode == CurrentEditorMode.Create)
        {
            HandlePlacementInput();

            Vector3 mouseWorld = sceneCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            previewObject.transform.position = mouseWorld;
        }
        else if (currentEditorMode == CurrentEditorMode.Select)
            HandleSelectionInput();
    }


    /*bool IsWithinPlayArea(Vector3 worldPos)
    {
        if (playAreaCollider == null) return true;
        return playAreaCollider.bounds.Contains(worldPos);
    }*/

    void HandlePlacementInput()
    {
        if (IsPointerOverUI()) return;

        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = sceneCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            //if (!IsWithinPlayArea(mouseWorld))
            //    return;

            PlaceObjectAt(mouseWorld);
        }
    }

    void HandleSelectionInput()
    {
        if (IsPointerOverUI()) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = sceneCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            Vector2 pos2D = mouseWorld;

            var hit = Physics2D.Raycast(pos2D, Vector2.zero, 0f);
            if (hit.collider != null)
            {
                var placed = hit.collider.GetComponentInParent<PlacedLevelObject>();
                if (placed != null)
                {
                    SelectObject(placed);
                    return;
                }
            }

            SelectObject(null);
        }
    }

    void SelectObject(PlacedLevelObject obj)
    {
        if (selectedObject == obj)
        {
            if (selectedObject != null && propertiesWindow != null)
                propertiesWindow.ShowFor(selectedObject);

            if (transformGizmo != null)
                transformGizmo.Attach(selectedObject);

            return;
        }

        if (selectedObject != null)
            selectedObject.SetSelected(false);

        selectedObject = obj;

        if (selectedObject != null)
        {
            selectedObject.SetSelected(true);
            if (propertiesWindow != null)
                propertiesWindow.ShowFor(selectedObject);

            if (transformGizmo != null)
                transformGizmo.Attach(selectedObject);
        }
        else
        {
            if (propertiesWindow != null)
                propertiesWindow.Hide();

            if (transformGizmo != null)
                transformGizmo.Attach(null);
        }
    }

    void PlaceObjectAt(Vector3 position)
    {
        var prefab = registry.GetAllPrefabs(currentObjectTypeId).editorPrefab;
        if (prefab == null)
        {
            Debug.LogWarning("No prefab registered for id: " + currentObjectTypeId);
            return;
        }

        var obj = Instantiate(prefab, position, Quaternion.identity, levelRoot);
        
        if(!obj.TryGetComponent(out PlacedLevelObject placed))
            placed = obj.AddComponent<PlacedLevelObject>();
        
        placed.objectTypeId = currentObjectTypeId;

        placedObjects.Add(placed);

        SelectObject(placed);

        MarkLevelChanged();

        if (!Input.GetKey(continuousPlacementKey))
        {
            if (previewObject != null)
                Destroy(previewObject);

            CancelPlacement();
        }
    }

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        if (transformGizmo != null)
            return transformGizmo.IsPointerOverGizmo(Input.mousePosition);
        
        if (Input.mousePresent)
            return EventSystem.current.IsPointerOverGameObject();

        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);


        return false;
    }

    public LevelData BuildLevelData()
    {
        var levelData = new LevelData { levelName = currentLevelName };
        foreach (var placed in placedObjects)
        {
            if (placed == null) continue;
            levelData.objects.Add(placed.ToInstanceData(registry));
        }

        return levelData;
    }

    public void LoadFromLevelData(LevelData data)
    {
        foreach (var placed in placedObjects)
            if (placed != null)
                Destroy(placed.gameObject);
        placedObjects.Clear();

        if (data == null) return;

        foreach (var o in data.objects)
        {
            var prefab = registry.GetAllPrefabs(o.objectTypeId).editorPrefab;
            if (prefab == null) continue;

            var obj = Instantiate(prefab, levelRoot);

            if (!obj.TryGetComponent(out PlacedLevelObject placed))
                placed = obj.AddComponent<PlacedLevelObject>();

            placed.objectTypeId = o.objectTypeId;
            placed.ApplyFromInstanceData(o, registry);

            placedObjects.Add(placed);
        }
    }

    public enum CurrentEditorMode
    {
        Select,
        Create
    }
}