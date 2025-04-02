using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectableImage : MonoBehaviour
{
    private Vector3 originalScale;
    private bool isSelected = false;
    private Image image;

    void Start()
    {
        originalScale = transform.localScale;
        image = GetComponent<Image>();
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;
        transform.localScale = isSelected ? originalScale * 0.9f : originalScale;
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void ResetSelection()
    {
        isSelected = false;
        transform.localScale = originalScale;
    }
}
