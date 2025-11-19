using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // When mouse enters button
    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetTrigger("Hover");
    }

    // When mouse exits button
    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetTrigger("Normal");
    }
}
