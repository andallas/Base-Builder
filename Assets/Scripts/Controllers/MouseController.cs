using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseController : MonoBehaviour
{
    public GameObject circleCursorPrefab;

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
    private Vector3 _dragStartPosition;
    private List<GameObject> _dragPreviewGameObjects;


    void Start()
    {
        _lastMousePosition = GetCurrentMousePosition();
        _dragPreviewGameObjects = new List<GameObject>();
    }

    void Update()
    {
        //UpdateCursor();
        HandleLeftClick();
        HandleKeyboardScroll();
        HandleZoom();
        HandleScreenDrag();

        _lastMousePosition = GetCurrentMousePosition();
    }

    private void HandleLeftClick()
    {
        // Start Drag
        if (Input.GetMouseButtonDown(0))
        {
            _dragStartPosition = GetNormalizedMousePosition();
        }


        while (_dragPreviewGameObjects.Count > 0)
        {
            GameObject go = _dragPreviewGameObjects[0];
            _dragPreviewGameObjects.RemoveAt(0);
            SimplePool.Despawn(go);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = GetNormalizedMousePosition();

            int start_x = (int)_dragStartPosition.x;
            int end_x = (int)currentMousePosition.x;
            int start_y = (int)_dragStartPosition.y;
            int end_y = (int)currentMousePosition.y;

            if (end_x < start_x) { int tempX = end_x; end_x = start_x; start_x = tempX; }
            if (end_y < start_y) { int tempY = end_y; end_y = start_y; start_y = tempY; }

            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile tile = WorldController.Instance.WorldData.GetTileAt(x, y);
                    if (tile != null)
                    {
                        GameObject go = SimplePool.Spawn(circleCursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        go.transform.SetParent(this.transform);
                        _dragPreviewGameObjects.Add(go);
                    }
                }
            }
        }

        // End drag
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 currentMousePosition = GetNormalizedMousePosition();

            int start_x = (int)_dragStartPosition.x;
            int end_x = (int)currentMousePosition.x;
            int start_y = (int)_dragStartPosition.y;
            int end_y = (int)currentMousePosition.y;

            if (end_x < start_x) { int tempX = end_x;  end_x = start_x;  start_x = tempX; }
            if (end_y < start_y) { int tempY = end_y;  end_y = start_y;  start_y = tempY; }

            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile tile = WorldController.Instance.WorldData.GetTileAt(x, y);
                    if (tile != null)
                    {
                        tile.Type = Tile.TileType.Floor;
                    }
                }
            }
        }
    }

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

    private void HandleScreenDrag()
    {
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Camera.main.transform.Translate(_lastMousePosition - GetCurrentMousePosition());
        }
    }

    private Vector3 GetCurrentMousePosition()
    {
        Vector3 curMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        curMousePos.z = 0;
        return curMousePos;
    }

    private Vector3 GetNormalizedMousePosition()
    {
        Vector3 curMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        curMousePos.x = Mathf.RoundToInt(curMousePos.x);
        curMousePos.y = Mathf.RoundToInt(curMousePos.y);
        curMousePos.z = 0;
        return curMousePos;
    }

    //private void UpdateCursor()
    //{
    //    Vector3 normalizedMousePosition = GetNormalizedMousePosition();
    //    Tile currentTile = WorldController.Instance.GetTileAtWorldCoord(normalizedMousePosition);
    //    if (currentTile != null)
    //    {
    //        circleCursor.SetActive(true);
    //        circleCursor.transform.position = normalizedMousePosition;
    //    }
    //    else
    //    {
    //        circleCursor.SetActive(false);
    //    }
    //}
}
