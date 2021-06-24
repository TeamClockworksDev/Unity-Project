﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardTileData : MonoBehaviour
{
    [Header("Tile Set Up")]
    public float size   = 1.0f;

    [Header("Tile Status")]
    public bool  active = true;
    public Color currentColor  = Color.gray;
    public int coordinatesX = -1;
    public int coordinatesY = -1;
    
    private Renderer _renderer;
    private GameObject _gameObjectRef;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        //Init References
        _renderer = GetComponent<Renderer>();
        _gameObjectRef = gameObject;

        //Init Properties - Name, Position, Color
        _gameObjectRef.transform.position = new Vector3(coordinatesX * size, 0, coordinatesY * size);
        _gameObjectRef.name = "GridTile [" + coordinatesX + "," + coordinatesY + "]";
        
        SetColor((coordinatesX + coordinatesY) % 2 == 0 ? GameManager.Instance.basicGridColor1 : GameManager.Instance.basicGridColor2);
    }

    public void SetCoordinates(int x = -1, int y = -1)
    {
        coordinatesX = x;
        coordinatesY = y;
    }

    private void SetColor(Color c)
    {
        if (_renderer != null)
        {
            currentColor = c;
            _renderer.material.color = currentColor;
        }
        else
        {
            Debug.LogError("[GameBoardTileData] Tile " + gameObject.name + " - Failed to SetColor(" + c + ") - Renderer Reference Was Null!");
        }
    }

}