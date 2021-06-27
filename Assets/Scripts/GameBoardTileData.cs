using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardTileData : MonoBehaviour
{
    // -------------------
    
    [Header("Tile Set Up")]
    public float size   = 1.0f;

    [Header("Tile Status")]
    public bool  active = true;
    [Space]
    public int coordinatesX = -1;
    public int coordinatesY = -1;
    [Space]
    public Color currentColor  = Color.gray;
    
    // -------------------
    
    private Renderer _renderer;
    private GameObject _gameObjectRef;
    
    private Color _originalColor;
    
    private float _colorLerpTime;
    private float _colorLerpDelay;
    
    private float _fallBackCountdown;

    // -------------------
    
    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (_fallBackCountdown > 0)
        {
            _fallBackCountdown -= Time.deltaTime;

            if (_fallBackCountdown <= 0)
            {
                LerpToColor(_originalColor);
                active = true;
            }
        }
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
        _originalColor = new Color(currentColor.r, currentColor.g, currentColor.b);
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

    public void FallAwayToColor(Color c, float lerpTime, float delay = 0.0f, bool fallingEnabled = false, float fallBackCD = 3.0f)
    {
        if (!active) return;
        
        _colorLerpTime = lerpTime;
        _colorLerpDelay = delay;
        
        active = !fallingEnabled;
        _fallBackCountdown = fallBackCD;
        
        LerpToColor(c);
    }

    private void LerpToColor(Color c)
    {
        StartCoroutine("LerpToNewColor", c);
    }
    
    
    private IEnumerator LerpToNewColor(Color endColor)
    {
        yield return new WaitForSecondsRealtime(_colorLerpDelay);
        
        Color startColor = currentColor;
        float elapsedTime = 0.0f;

        
        //Change Color
        while (elapsedTime <= _colorLerpTime)
        {
            SetColor(Color.Lerp(startColor, endColor, elapsedTime / _colorLerpTime));

            elapsedTime += Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }
        
        //Fade Away (In place of falling, for now.)
        if (!active && elapsedTime > 0)
        {
            elapsedTime = 0.0f;

            while (elapsedTime <= _colorLerpTime)
            {
                endColor.a = 1 - (elapsedTime / _colorLerpTime);
                
                SetColor(endColor);

                elapsedTime += Time.deltaTime;
            
                yield return new WaitForEndOfFrame();
            }
        }

        SetColor(endColor);
    }

}
