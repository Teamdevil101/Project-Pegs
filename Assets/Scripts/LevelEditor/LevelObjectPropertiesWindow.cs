using System.Collections.Generic;
using UnityEngine;

public class LevelObjectPropertiesWindow : MonoBehaviour
{
    public GameObject rootPanel;
    public Transform contentRoot;

    public GameObject sectionGroupPrefab;
    public GameObject rowPrefab;

    [SerializeField]
    private LevelEditorController editorController;
    private PlacedLevelObject currentTarget;

    private class BoundProperty
    {
        public ILevelEditable owner;
        public LevelPropertyDescriptor descriptor;
        public LevelPropertyRow row;
    }

    private List<BoundProperty> boundProperties = new List<BoundProperty>();

    void OnEnable()
    {
        if (editorController != null)
            editorController.OnEditableChanged += HandleEditableChanged;
    }

    void OnDisable()
    {
        if (editorController != null)
            editorController.OnEditableChanged -= HandleEditableChanged;
    }

    private void HandleEditableChanged(ILevelEditable editable)
    {
        if (currentTarget == null || editable == null) return;

        if (editable is Component c && c.gameObject == currentTarget.gameObject)
            RebuildUI();
    }

    public void ShowFor(PlacedLevelObject target)
    {
        currentTarget = target;
        RebuildUI();
        rootPanel.SetActive(true);
    }

    public void Hide()
    {
        rootPanel.SetActive(false);
        currentTarget = null;
        ClearUI();
    }

    void ClearUI()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);
        boundProperties.Clear();
    }

    void RebuildUI()
    {
        ClearUI();
        if (currentTarget == null) return;

        var editables = currentTarget.GetComponents<ILevelEditable>();
        var sectionGroupRoot = Instantiate(sectionGroupPrefab, contentRoot);

        foreach (var ed in editables)
        {
            var props = new List<LevelPropertyDescriptor>();
            ed.GetProperties(props);
            if (props.Count == 0) continue;

            var header = sectionGroupRoot.GetComponentInChildren<SectionHeaderRow>();
            header.Setup(ed.GetSectionTitle());

            foreach (var p in props)
            {
                var rowObj = Instantiate(rowPrefab, header.sectionContentRoot);
                var row = rowObj.GetComponent<LevelPropertyRow>();

                var bound = new BoundProperty { owner = ed, descriptor = p, row = row };
                boundProperties.Add(bound);

                var localBound = bound;
                row.Setup(p, desc =>
                {
                    for (int i = 0; i < boundProperties.Count; i++)
                    {
                        if (ReferenceEquals(boundProperties[i].descriptor, desc) ||
                            (boundProperties[i].descriptor.ownerId == desc.ownerId &&
                             boundProperties[i].descriptor.id == desc.id))
                        {
                            boundProperties[i].descriptor = desc;
                            boundProperties[i].owner.ApplyProperty(desc);

                            if (editorController != null)
                            {
                                editorController.MarkLevelChanged();
                                editorController.MarkEditableChanged(boundProperties[i].owner);
                            }
                            break;
                        }
                    }
                });
            }
        }
    }
}