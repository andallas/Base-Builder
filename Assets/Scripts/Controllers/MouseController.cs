using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour
{
    public GameObject circleCursor;

    private float fastSpeedMultiplier = 2;
    private float keyScrollSpeed = 2;

    private float zoomSpeed = 1f;
    private float zoomMax = 5f;
    private float zoomMin = 15f;
    float targetZoom = 5f;
    float zoomDuration = 1.0f;
    float zoomElapsed = 0.0f;
    bool zoomTransition = false;

    private Vector3 _lastMousePosition;


    void Start()
    {
        _lastMousePosition = GetCurrentMousePosition();
    }

    void Update()
    {
        // Update the circleCursor position
        circleCursor.transform.position = NormalizeToTileCoords(GetCurrentMousePosition());

        // Handle camera movement
        HandleKeyboardScroll();
        HandleZoom();
        HandleScreenDrag();

        _lastMousePosition = GetCurrentMousePosition();
    }

    // TODO: Let's LERP this
    private void HandleKeyboardScroll()
    {
        float translationX = Input.GetAxis("Horizontal");
        float translationY = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Camera.main.transform.Translate(translationX * fastSpeedMultiplier * keyScrollSpeed, translationY * fastSpeedMultiplier * keyScrollSpeed, 0);
        }
        else
        {
            Camera.main.transform.Translate(translationX * keyScrollSpeed, translationY * keyScrollSpeed, 0);
        }
    }

    private void HandleZoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.orthographicSize > zoomMax) // Zoom in
        {
            targetZoom -= zoomSpeed;
            zoomTransition = true;
            zoomElapsed = 0.0f;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.orthographicSize < zoomMin) // Zoom out
        {
            targetZoom += zoomSpeed;
            zoomTransition = true;
            zoomElapsed = 0.0f;
        }

        if (zoomTransition)
        {
            zoomElapsed += Time.deltaTime / zoomDuration;

            float currentOrtho = Camera.main.orthographicSize;
            currentOrtho = Mathf.Lerp(currentOrtho, targetZoom, zoomElapsed);
            currentOrtho = (currentOrtho > zoomMin) ? zoomMin : (currentOrtho < zoomMax) ? zoomMax : currentOrtho;
            Camera.main.orthographicSize = currentOrtho;
            
            if (zoomElapsed > 1.0f)
            {
                targetZoom = Camera.main.orthographicSize;
                zoomTransition = false;
                zoomElapsed = 0.0f;
            }
        }
    }

    // TODO: Maybe we should LERP this too?
    private void HandleScreenDrag()
    {
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Vector3 difference = _lastMousePosition - GetCurrentMousePosition();
            Camera.main.transform.Translate(difference);
        }
    }

    private Vector3 NormalizeToTileCoords(Vector3 vec)
    {
        vec.x = Mathf.RoundToInt(vec.x);
        vec.y = Mathf.RoundToInt(vec.y);
        return vec;
    }

    private Vector3 GetCurrentMousePosition()
    {
        Vector3 curMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        curMousePos.z = 0;
        return curMousePos;
    }
}
