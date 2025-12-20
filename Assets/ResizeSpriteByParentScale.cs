using UnityEngine;

public class ResizeSpriteByParentScale : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 currentParentSize;

    public void Update()
    {
        if(currentParentSize != transform.parent.localScale)
        {
            currentParentSize = transform.localScale;
            spriteRenderer.size = currentParentSize;
        }
    }
}
