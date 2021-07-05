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
	private Vector3 _originalPos;
	
	private float _sequenceLerpDelay;
	
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
				//LerpToColor(_originalColor);
				BeginSequence(new IEnumerator[]
				{
					LerpToNewPosition(_originalPos, 1.0f, _sequenceLerpDelay),
					LerpToNewColor(_originalColor, 1.0f, 0.0f)
				});
				active = true;
			}
		}
	}

	private void BeginSequence(IEnumerator[] sequence)
    {
		StartCoroutine(RunEventSequence(sequence));
	}

	private IEnumerator RunEventSequence(IEnumerator[] sequence)
    {
		for (int i = 0; i < sequence.Length; i++)
    		yield return StartCoroutine(sequence[i]);
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
		_originalPos = _gameObjectRef.transform.position;
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

	private void SetPosition(Vector3 pos)
    {
		if (_gameObjectRef != null)
        {
			_gameObjectRef.transform.position = pos;
        }
		else
        {
			Debug.LogError("[GameBoardTileData] Tile " + gameObject.name + " - Failed to SetPosition(" + pos + ") - GameObject Reference Was Null!");
        }
    }

	private Vector3 GetPosition()
	{
		if (_gameObjectRef != null)
			return _gameObjectRef.transform.position;
		
		Debug.LogError("[GameBoardTileData] Tile " + gameObject.name + " - Failed to GetPosition() - GameObject Reference Was Null!");
		return default;
	}

	// - Restructured function to allow for a blend between original and new for the updated colour. (Beastie)
	public void FallAwayToColor(Color c, float lerpTime, float blendFactor = 1.0f, float delay = 0.0f, bool fallingEnabled = false, float fallBackCD = 3.0f)
	{
		if (!active) return;
		
		_sequenceLerpDelay = delay;
		
		active = !fallingEnabled;
		_fallBackCountdown = fallBackCD;

		Color blendColor = Color.Lerp(_originalColor, c, blendFactor);
		Vector3 fallPos = _originalPos + Vector3.down * 50f;
		
		IEnumerator[] fallSequence = new IEnumerator[]
		{
			LerpToNewColor(blendColor, lerpTime, delay),
			LerpToNewPosition(fallPos, lerpTime, 0.5f)
		};

		BeginSequence(fallSequence);
	}

	private void LerpToColor(Color c, float duration, float delay)
	{
		StartCoroutine(LerpToNewColor(c, duration, delay));
	}
	
	private IEnumerator LerpToNewColor(Color endColor, float duration, float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		
		Color startColor = currentColor;
		float elapsedTime = 0.0f;

		
		//Change Color
		while (elapsedTime <= duration)
		{
			SetColor(Color.Lerp(startColor, endColor, elapsedTime / duration));

			elapsedTime += Time.deltaTime;
			
			yield return new WaitForEndOfFrame();
		}

		//Fade Away (In place of falling, for now.)
		//	- Keeping this while I work on an event sequencer, since I like the effect and it works well to hide the blocks after they fall. (Beastie)
		/*
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
		*/

		SetColor(endColor);
	}

	private void LerpToPosition(Vector3 pos, float duration, float delay)
    {
		StartCoroutine(LerpToNewPosition(pos, duration, delay));
    }

	private IEnumerator LerpToNewPosition(Vector3 endPos, float duration, float delay)
    {
		yield return new WaitForSecondsRealtime(delay);

		Vector3 startPos = GetPosition();
		float elapsedTime = 0.0f;

		while (elapsedTime <= duration)
        {
			SetPosition(Vector3.Lerp(startPos, endPos, elapsedTime / duration));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
        }

		SetPosition(endPos);
    }
}
