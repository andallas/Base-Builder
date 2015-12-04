using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour
{
    public GameObject circleCursor;

    private Vector3 _lastMousePosition;


    void Start()
    {
        _lastMousePosition = GetCurrentMousePosition();
    }

    void Update()
    {
        Vector3 currentMousePosition = GetCurrentMousePosition();

        // Update the circleCursor position
        Vector3 circleCursorPos = currentMousePosition;
        circleCursorPos.x = Mathf.RoundToInt(circleCursorPos.x);
        circleCursorPos.y = Mathf.RoundToInt(circleCursorPos.y);

        circleCursor.transform.position = circleCursorPos;

        // Handle screen dragging
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Vector3 difference = _lastMousePosition - currentMousePosition;
            Camera.main.transform.Translate(difference);
        }

        _lastMousePosition = GetCurrentMousePosition();
    }

    private Vector3 GetCurrentMousePosition()
    {
        Vector3 curMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        curMousePos.z = 0;
        return curMousePos;
    }
}
