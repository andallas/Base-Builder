using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AutomaticVerticalSize : MonoBehaviour
{
    public float childHeight = 35f;
    public float padding = 2f;
	
	void Start()
    {
        AdjustSize();
	}

    void AdjustSize()
    {
        float childCount = this.transform.childCount;
        float spacing = (childCount - 1) * padding;

        Vector2 size = this.GetComponent<RectTransform>().sizeDelta;
        size.y = (childCount * childHeight) + spacing;
        this.GetComponent<RectTransform>().sizeDelta = size;
    }
}
