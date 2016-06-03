using UnityEngine;


public class AutomaticVerticalSize : MonoBehaviour
{
	public float childHeight = 35f;
	
	void Start()
	{
		AdjustSize();
	}
	
	public void AdjustSize()
	{
		Vector2 size = this.GetComponent<RectTransform>().sizeDelta;
		size.y = (this.transform.childCount * childHeight);
		this.GetComponent<RectTransform>().sizeDelta = size;
	}
}
