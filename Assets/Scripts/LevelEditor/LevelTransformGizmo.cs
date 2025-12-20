using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelTransformGizmo : MonoBehaviour
{
    public enum HandleMode
    {
        None,
        MoveX,
        MoveY,
        MoveXY,
        ScaleX,
        ScaleY,
        Rotate
    }

    [Header("References")]
    public LevelEditorController editor;
    public Camera sceneCamera;

    [Header("Handles")]
    public Collider2D handleXCollider;
    public Collider2D handleYCollider;
    public Collider2D handleCenterCollider;
    public Collider2D handleRotateCollider;

    public static bool positionSnapEnabled = true;
    public static float positionSnapStep = 0.25f;

    public static bool scaleSnapEnabled = true;
    public static float scaleSnapStep = 0.25f;

    public static bool rotationSnapEnabled = true;
    public static float rotationSnapStep = 15f;

    public LayerMask gizmoLayer;

    private PlacedLevelObject target;
    private HandleMode currentMode = HandleMode.None;

    private Vector3 dragStartMouseWorld;
    private Vector3 dragStartPos;
    private Vector3 dragStartScale;
    private float dragStartRotZ;

    private bool isDragging;

    void Awake()
    {
        if (sceneCamera == null && editor != null)
            sceneCamera = editor.sceneCamera;
    }

    void Update()
    {
        if (editor == null || sceneCamera == null)
            return;

        if (target != null)
        {
            SetGizmoActive(true);
            transform.position = target.transform.position;
        }
        else
        {
            SetGizmoActive(false);
            return;
        }

        HandleInput();
    }

    public void Attach(PlacedLevelObject newTarget)
    {
        target = newTarget;
        isDragging = false;
        currentMode = HandleMode.None;

        if (target != null)
        {
            transform.position = target.transform.position;
            SetGizmoActive(true);
        }
        else
        {
            SetGizmoActive(false);
        }
    }

    private void HandleInput()
    {
        if (IsPointerOverUI())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            BeginDrag();
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            ContinueDrag();
        }

        if (isDragging && Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    private void BeginDrag()
    {
        if (target == null)
            return;

        Vector3 mouseWorld = GetMouseWorld();
        Vector2 mouse2D = mouseWorld;

        Collider2D hit = Physics2D.OverlapPoint(mouse2D, gizmoLayer);
        if (hit == null)
            return;

        if (hit == handleXCollider)
            currentMode = HandleMode.MoveX;
        else if (hit == handleYCollider)
            currentMode = HandleMode.MoveY;
        else if (hit == handleCenterCollider)
            currentMode = HandleMode.MoveXY;
        else if (hit == handleRotateCollider)
            currentMode = HandleMode.Rotate;
        else
            currentMode = HandleMode.None;

        if (currentMode == HandleMode.None)
            return;

        isDragging = true;
        dragStartMouseWorld = mouseWorld;
        dragStartPos = target.transform.position;
        dragStartRotZ = target.transform.eulerAngles.z;
        dragStartScale = target.MyMainObject.transform.localScale;
    }

    private void ContinueDrag()
    {
        if (target == null || currentMode == HandleMode.None)
            return;

        Vector3 mouseWorld = GetMouseWorld();
        Vector3 delta = mouseWorld - dragStartMouseWorld;

        var t = target.transform;

        if (currentMode == HandleMode.MoveX ||
            currentMode == HandleMode.MoveY ||
            currentMode == HandleMode.MoveXY)
        {
            Vector3 newPos = dragStartPos;

            if (currentMode == HandleMode.MoveX || currentMode == HandleMode.MoveXY)
                newPos.x += delta.x;

            if (currentMode == HandleMode.MoveY || currentMode == HandleMode.MoveXY)
                newPos.y += delta.y;

            if (positionSnapEnabled)
                newPos = ApplyPositionSnap(newPos);

            t.position = newPos;
        }
        else if (currentMode == HandleMode.Rotate)
        {
            Vector3 center = dragStartPos;
            float startAngle = Mathf.Atan2(dragStartMouseWorld.y - center.y, dragStartMouseWorld.x - center.x) * Mathf.Rad2Deg;
            float currentAngle = Mathf.Atan2(mouseWorld.y - center.y, mouseWorld.x - center.x) * Mathf.Rad2Deg;

            float deltaAngle = Mathf.DeltaAngle(startAngle, currentAngle);
            float newRot = dragStartRotZ + deltaAngle;

            if (rotationSnapEnabled && rotationSnapStep > 0.0001f)
            {
                newRot = Mathf.Round(newRot / rotationSnapStep) * rotationSnapStep;
            }

            t.rotation = Quaternion.Euler(0f, 0f, newRot);
        }

        if (target != null)
            target.NotifyTransformEdited();
    }

    private void EndDrag()
    {
        isDragging = false;
        currentMode = HandleMode.None;

        var posChanged = target.transform.position != dragStartPos;
        var rotChanged = Mathf.Abs(Mathf.DeltaAngle(target.transform.eulerAngles.z, dragStartRotZ)) > 0.001f;
        var scaleChanged = target.MyMainObject.transform.localScale != dragStartScale;

        if (posChanged || rotChanged || scaleChanged)
        {
            editor.MarkLevelChanged();
            editor.MarkEditableChanged(target);
        }
    }

    private Vector3 ApplyPositionSnap(Vector3 worldPos)
    {
        Vector3 snapped = worldPos;

        if (positionSnapStep > 0.0001f)
        {
            snapped.x = Mathf.Round(worldPos.x / positionSnapStep) * positionSnapStep;
            snapped.y = Mathf.Round(worldPos.y / positionSnapStep) * positionSnapStep;
        }

        return snapped;
    }

    private Vector3 GetMouseWorld()
    {
        Vector3 mouse = Input.mousePosition;
        var cam = sceneCamera != null ? sceneCamera : Camera.main;
        Vector3 world = cam.ScreenToWorldPoint(mouse);
        world.z = 0f;
        return world;
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        if (Input.mousePresent)
            return EventSystem.current.IsPointerOverGameObject();

        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        return false;
    }

    public void SetGizmoActive(bool active)
    {
        handleXCollider.gameObject.SetActive(active);
        handleYCollider.gameObject.SetActive(active);
        handleCenterCollider.gameObject.SetActive(active);
        handleRotateCollider.gameObject.SetActive(active);
    }

    public bool IsPointerOverGizmo(Vector2 screenPos)
    {
        if (IsPointerOverUI())
            return true;

        if (sceneCamera == null && editor != null)
            sceneCamera = editor.sceneCamera;

        var cam = sceneCamera != null ? sceneCamera : Camera.main;
        Vector3 w = cam.ScreenToWorldPoint(screenPos);
        w.z = 0f;

        var hit = Physics2D.OverlapPoint((Vector2)w, gizmoLayer);
        if (hit == null) return false;

        return hit == handleXCollider ||
               hit == handleYCollider ||
               hit == handleCenterCollider ||
               hit == handleRotateCollider;
    }
}
