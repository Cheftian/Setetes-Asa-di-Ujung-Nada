using UnityEngine;
using UnityEngine.EventSystems;

public class SfxTrigger : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Nama SFX di AudioManager")]
    [SerializeField] private string hoverSfxName = "Button-hover"; 
    [SerializeField] private string clickSfxName = "Button-click";

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(hoverSfxName))
        {
            AudioManager.Instance.PlaySFX(hoverSfxName);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(clickSfxName))
        {
            AudioManager.Instance.PlaySFX(clickSfxName);
        }
    }
}