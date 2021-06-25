using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum SpawnPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public static GameManager Instance;
    
    [Header("References")] 
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _gridTilePrefab;
    [SerializeField] private GameObject _mainCameraRef;
    [SerializeField] private Transform  _gameBoardParent;
    
    [Header("Game Settings")]
    [Range(6, 33)]
    [SerializeField] private int _gridSize = 9;
    [SerializeField] private SpawnPosition _playerOneSpawnPosition = SpawnPosition.TopLeft;
    
    public Color basicGridColor1 = Color.gray;
    public Color basicGridColor2 = Color.black;
    
    [SerializeField] private float _colorChangeSpeed = 1.5f;

    [Header("Dev Tools")]
    [SerializeField] private bool _resetGameBoard;
    [SerializeField] private bool _enableFallingBlocks;

    private GameBoardTileData[,] _gameBoardTiles;
    private GameObject _playerOneGO;
    private GamePlayer _playerOneRef;
    
    
    private void Start()
    {
        Instance = this;
        
        InitializeGame();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R) || _resetGameBoard)
        {
            ResetGameBoard();
        }
    }

    private void InitializeGame()
    {
        if (_gameBoardTiles == null)
        {
            //Generate Game Board
            _gameBoardTiles = new GameBoardTileData[_gridSize, _gridSize];

            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    _gameBoardTiles[i, j] = Instantiate(_gridTilePrefab, _gameBoardParent).GetComponent<GameBoardTileData>();
                    _gameBoardTiles[i, j].SetCoordinates(i, j);
                }
            }
            
            //Position Camera
            _mainCameraRef.transform.position = new Vector3(_gridSize / 2.0f, _gridSize * 1.5f, _gridSize * 1.01f);
            
            //Spawn & Position PLayer
            _playerOneGO  = _playerOneGO == null ? Instantiate(_playerPrefab, transform) : _playerOneGO;
            _playerOneRef = _playerOneGO.GetComponent<GamePlayer>();
            
            int spawnPlayerPosX = _playerOneSpawnPosition == SpawnPosition.BottomLeft  || _playerOneSpawnPosition == SpawnPosition.TopLeft     ? _gridSize - 1 : 0;
            int spawnPlayerPosY = _playerOneSpawnPosition == SpawnPosition.BottomRight || _playerOneSpawnPosition == SpawnPosition.BottomLeft  ? _gridSize - 1 : 0;

            _playerOneRef.SetPosition(spawnPlayerPosX, spawnPlayerPosY);
            
            _playerOneRef.SetRotation(_playerOneSpawnPosition == SpawnPosition.BottomLeft || _playerOneSpawnPosition == SpawnPosition.TopLeft ? GamePlayer.FacingDirection.Right : GamePlayer.FacingDirection.Left);

            _playerOneGO.name = "Player One";
        }
        else
        {
            Debug.LogError("[GameManager] InitializeGame() - Game Board Already Initialized!");
        }
    }

    private void ResetGameBoard()
    {
        //TODO Agon - Destroy() is inefficient. We should use a pooling system.
        foreach (var tile in _gameBoardTiles)
        {
            Destroy(tile.gameObject); 
        }

        _gameBoardTiles = null;

        InitializeGame();

        _resetGameBoard = false;
    }

    public void MovePlayer(int newX, int newY)
    {
        if  (newX < 0 || newY < 0 || newX >= _gridSize || newY >= _gridSize)
        {
            return;
        }

        if (_gameBoardTiles[newX, newY].active)
        {
            _playerOneRef.SetPosition(newX, newY);
        }
    }

    //Changes the color of grid tiles in-front of the player to match that player's color
    public void PlayerAction_ColorLine(GamePlayer.FacingDirection fDirection, GamePlayer playerRef)
    {
        int x = fDirection == GamePlayer.FacingDirection.Left  ? 1 : fDirection == GamePlayer.FacingDirection.Right ? -1 : 0;
        int y = fDirection == GamePlayer.FacingDirection.Down  ? 1 : fDirection == GamePlayer.FacingDirection.Up    ? -1 : 0;

        float colorChangeDelay = 0.0f;
        
        if (x != 0)
        {
            for (int i = playerRef.positionX + x; i < _gridSize && i >= 0; i += x)
            {
                _gameBoardTiles[i, playerRef.positionY].LerpToColor(playerRef.playerColor, _colorChangeSpeed, colorChangeDelay, _enableFallingBlocks);
                colorChangeDelay += 0.1f;
            }
        }
        else if (y != 0)
        {
            for (int i = playerRef.positionY + y; i < _gridSize && i >= 0; i += y)
            {
                _gameBoardTiles[playerRef.positionX, i].LerpToColor(playerRef.playerColor, _colorChangeSpeed, colorChangeDelay, _enableFallingBlocks);
                colorChangeDelay += 0.1f;
            }
        }
    }

}
