﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class MouseController : MonoBehaviour
{
    public GameObject circleCursorPrefab;

    private float _fastSpeedMultiplier = 2;
    private float _keyScrollSpeed = 2;

    private float _zoomSpeed = 1f;
    private float _zoomMax = 5f;
    private float _zoomMin = 15f;

    private float _targetZoom = 5f;
    private float _zoomDuration = 1.0f;
    private float _zoomElapsed = 0.0f;
    private bool _zoomTransition = false;

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
        UpdateDragSelect();

        // Camera movement
        UpdateKeyboardScroll();
        UpdateZoom();
        UpdateScreenDrag();

        _lastMousePosition = GetCurrentMousePosition();
    }


    private void UpdateDragSelect()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Start Drag
        if (Input.GetMouseButtonDown(0))
        {
            _dragStartPosition = GetNormalizedMousePosition();
        }

        // If the mouse hasn't moved, no reason to do any actual drag-based code
        if (_dragStartPosition != _lastMousePosition)
        {
            // Cleanup old drag previews
            while (_dragPreviewGameObjects.Count > 0)
            {
                GameObject go = _dragPreviewGameObjects[0];
                _dragPreviewGameObjects.RemoveAt(0);
                SimplePool.Despawn(go);
            }

            // Currently Dragging
            if (Input.GetMouseButton(0))
            {
                DoActionOnSelectedTiles((tile) =>
                    {
                        GameObject go = SimplePool.Spawn(circleCursorPrefab, new Vector3(tile.X, tile.Y, 0), Quaternion.identity);
                        go.transform.SetParent(this.transform);
                        _dragPreviewGameObjects.Add(go);
                    });
            }
        }

        // End drag
        if (Input.GetMouseButtonUp(0))
        {
            BuildModeController bmc = GameObject.FindObjectOfType<BuildModeController>();

            DoActionOnSelectedTiles((tile) =>
                {
                    bmc.DoBuild(tile);
                });
        }
    }

    private void UpdateKeyboardScroll()
    {
        float translationX = Input.GetAxis("Horizontal");
        float translationY = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Camera.main.transform.Translate(translationX * _fastSpeedMultiplier * _keyScrollSpeed, translationY * _fastSpeedMultiplier * _keyScrollSpeed, 0);
        }
        else
        {
            Camera.main.transform.Translate(translationX * _keyScrollSpeed, translationY * _keyScrollSpeed, 0);
        }
    }

    private void UpdateZoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.orthographicSize > _zoomMax) // Zoom in
        {
            _targetZoom -= _zoomSpeed;
            _zoomTransition = true;
            _zoomElapsed = 0.0f;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.orthographicSize < _zoomMin) // Zoom out
        {
            _targetZoom += _zoomSpeed;
            _zoomTransition = true;
            _zoomElapsed = 0.0f;
        }

        if (_zoomTransition)
        {
            _zoomElapsed += Time.deltaTime / _zoomDuration;

            float currentOrtho = Camera.main.orthographicSize;
            currentOrtho = Mathf.Lerp(currentOrtho, _targetZoom, _zoomElapsed);
            currentOrtho = (currentOrtho > _zoomMin) ? _zoomMin : (currentOrtho < _zoomMax) ? _zoomMax : currentOrtho;
            Camera.main.orthographicSize = currentOrtho;
            
            if (_zoomElapsed > 1.0f)
            {
                _targetZoom = Camera.main.orthographicSize;
                _zoomTransition = false;
                _zoomElapsed = 0.0f;
            }
        }
    }

    private void UpdateScreenDrag()
    {
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            // TODO: How to make this feel better? Interpolation of some sort most likely.
            Camera.main.transform.Translate(_lastMousePosition - GetCurrentMousePosition());
        }
    }


    private void DoActionOnSelectedTiles(Action<Tile> action)
    {
        Vector3 currentMousePosition = GetNormalizedMousePosition();

        int start_x = (int)_dragStartPosition.x;
        int end_x = (int)currentMousePosition.x;
        int start_y = (int)_dragStartPosition.y;
        int end_y = (int)currentMousePosition.y;

        if (end_x < start_x) { SwapInts(ref start_x, ref end_x); }
        if (end_y < start_y) { SwapInts(ref start_y, ref end_y); }

        World world = WorldController.WorldData;

        for (int x = start_x; x <= end_x; x++)
        {
            for (int y = start_y; y <= end_y; y++)
            {
                Tile tile = world.GetTileAt(x, y);
                if (tile != null)
                {
                    action(tile);
                }
            }
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
        Vector3 curMousePos = GetCurrentMousePosition();
        curMousePos.x = Mathf.RoundToInt(curMousePos.x);
        curMousePos.y = Mathf.RoundToInt(curMousePos.y);
        return curMousePos;
    }

    private void SwapInts(ref int a, ref int b)
    {
        int temp = b;
        b = a;
        a = temp;
    }
}
