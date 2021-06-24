using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
    public int positionX = -1;
    public int positionY = -1;
    
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
    [SerializeField] private KeyCode actionKey = KeyCode.Z;
    
    
    void Update()
    {
        DetectMovement();
        DetectAction();
    }

    private void DetectMovement()
    {
        int moveX = 0;
        int moveY = 0;
        
        //Move Up
        if (Input.GetKeyDown(movementKeyUp) || Input.GetKeyDown(altMovementKeyUp))
        {
            moveY = -1;
        }
        //Move Down
        else if (Input.GetKeyDown(movementKeyDown) || Input.GetKeyDown(altMovementKeyDown))
        {
            moveY =  1;
        }
        //Move Left
        else if (Input.GetKeyDown(movementKeyLeft) || Input.GetKeyDown(altMovementKeyLeft))
        {
            moveX =  1;
        }
        //Move Right
        else if (Input.GetKeyDown(movementKeyRight) || Input.GetKeyDown(altMovementKeyRight))
        {
            moveX = -1;
        }

        if (moveX == 0 && moveY == 0)
        {
            return;
        }
        
        GameManager.Instance.MovePlayer(positionX + moveX, positionY + moveY);
    }

    private void DetectAction()
    {
        if (Input.GetKeyDown(actionKey))
        {
            //DO ACTION
        }
    }

    public void SetPosition(int x, int y)
    {
        positionX = x;
        positionY = y;
        
        gameObject.transform.localPosition = new Vector3(x, 0.5f, y);
    }
}
