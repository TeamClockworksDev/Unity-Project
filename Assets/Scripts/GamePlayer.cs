using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
    public enum FacingDirection
    {
        Up, Down, Left, Right
    }

    public Color playerColor = Color.white;

    [Space]
    
    public FacingDirection facing = FacingDirection.Right;
    
    public int positionX = -1;
    public int positionY = -1;
    
    [Space]
    [Range(0, 3)]
    public  float   movementSpeed = 0.35f;
    private bool    _isMoving;
    private Vector3 _oldPosition;
    private Vector3 _newPosition;
    private float   _movingLerpValue;
    
    [Header("Movement Keys")]
    [SerializeField] private KeyCode movementKeyUp    = KeyCode.W;
    [SerializeField] private KeyCode movementKeyDown  = KeyCode.S;
    [SerializeField] private KeyCode movementKeyLeft  = KeyCode.A;
    [SerializeField] private KeyCode movementKeyRight = KeyCode.D;
    
    [Header("Alternative Movement Keys")]
    [SerializeField] private KeyCode altMovementKeyUp    = KeyCode.UpArrow;
    [SerializeField] private KeyCode altMovementKeyDown  = KeyCode.DownArrow;
    [SerializeField] private KeyCode altMovementKeyLeft  = KeyCode.LeftArrow;
    [SerializeField] private KeyCode altMovementKeyRight = KeyCode.RightArrow;
    
    [Header("Action Keys")]
    [SerializeField] private KeyCode actionKey1 = KeyCode.Z;
    [SerializeField] private KeyCode actionKey2 = KeyCode.Space;

    void Start()
    {
        GetComponentInChildren<MeshRenderer>().material.color = playerColor;
        GetComponentInChildren<Light>().color = playerColor;
    }

    private void Update()
    {
        DetectMovementInput();
        DetectActionInput();

        DoSlowMovement();
    }

    private void DetectMovementInput()
    {
        if (_isMoving)
        {
            return;
        }

        int moveX = 0;
        int moveY = 0;
        
        if (Input.GetKeyDown(movementKeyUp) || Input.GetKeyDown(altMovementKeyUp))
        {
            moveY = -1;
            SetRotation(FacingDirection.Up);
        }
        else if (Input.GetKeyDown(movementKeyDown) || Input.GetKeyDown(altMovementKeyDown))
        {
            moveY =  1;
            SetRotation(FacingDirection.Down);
        }
        else if (Input.GetKeyDown(movementKeyLeft) || Input.GetKeyDown(altMovementKeyLeft))
        {
            moveX =  1;
            SetRotation(FacingDirection.Left);
        }
        else if (Input.GetKeyDown(movementKeyRight) || Input.GetKeyDown(altMovementKeyRight))
        {
            moveX = -1;
            SetRotation(FacingDirection.Right);
        }

        if (moveX == 0 && moveY == 0)
        {
            return;
        }
        
        GameManager.Instance.MovePlayer(positionX + moveX, positionY + moveY);
    }

    private void DetectActionInput()
    {
        if (Input.GetKeyDown(actionKey1) || Input.GetKeyDown(actionKey2))
        {
            GameManager.Instance.PlayerAction_ColorLine(facing, this);
        }
    }

    public void SetPosition(int x, int y)
    {
        positionX = x;
        positionY = y;

        _isMoving = true;
        _movingLerpValue = 0.0f;
        
        _newPosition = new Vector3(x, 0.5f, y);
        _oldPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }

    public void SetRotation(FacingDirection fDir)
    {
        float rotationNewY = 0.0f;
        
        switch (fDir)
        {
            case FacingDirection.Up   : rotationNewY = -90.0f; break;
            case FacingDirection.Down : rotationNewY =  90.0f; break;
            case FacingDirection.Left : rotationNewY = 180.0f; break;
            case FacingDirection.Right: rotationNewY =   0.0f; break;
        }

        facing = fDir;
        
        transform.eulerAngles = new Vector3(0.0f, rotationNewY, 0.0f);
    }

    private void DoSlowMovement()
    {
        if (_isMoving)
        {
            _movingLerpValue += Time.deltaTime;

            if (_movingLerpValue >= movementSpeed)
            {
                gameObject.transform.localPosition = _newPosition;
                _isMoving = false;
            }
            else
            {
                float lerpVal = _movingLerpValue / movementSpeed;
                gameObject.transform.localPosition = Vector3.Lerp(_oldPosition, _newPosition, lerpVal);
            }
        }
    }
}
