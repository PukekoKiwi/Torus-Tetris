using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public class TetrisManager : MonoBehaviour
{
    private int _majorSegments;
    private int _minorSegments;
    private int[,] _landed;

    // Used for tetris piece gravity snaps
    private float _elapsedFTT = 0f;
    private float _fastTickTime = 0.1f;
    //private int _fastTickCount = 0;
    private float _elapsedNTT = 0f;
    private float _normalTickTime = 0.8f;
    //private int _normalTickCount = 0;

    private Dictionary<char, int[,]> _tetMap = new Dictionary<char, int[,]>();
    private Tetromino _currentTet;

    // Properties
    public Tetromino CurrentTet { get => _currentTet; }
    public int[,] Landed { get => _landed; set => _landed = value; }

    // Start is called before the first frame update
    void Start()
    {
        _majorSegments = GetComponent<TorusManager>().MajorSegments;
        _minorSegments = GetComponent<TorusManager>().MinorSegments;
        // Creates 2D array for tetris representation
        _landed = new int[_majorSegments, _minorSegments];

        // Spawns a tetromino at the top left
        _currentTet = new Tetromino(Block.T, new Vector2(2, 2));
    }

    // Update is called once per frame
    void Update()
    {
        // Executes functions on fast ticks
        _elapsedFTT += Time.deltaTime;
        if (_elapsedFTT >= _fastTickTime)
        {
            _elapsedFTT = _elapsedFTT % _fastTickTime;
            // Do things during fast ticks here
            //_fastTickCount++;
        }

        // Executes functions on normal ticks
        _elapsedNTT += Time.deltaTime;
        if (_elapsedNTT >= _normalTickTime)
        {
            _elapsedNTT = _elapsedNTT % _normalTickTime;
            // Do things during normal ticks here
            //_normalTickCount++;
        }

        //TODO: Implement wrapping

        if (Input.GetKeyDown(KeyCode.Z))
        {
            RotateTetromino(false);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RotateTetromino(true);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveTetromino(0, 1);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveTetromino(-1, 0);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveTetromino(1, 0);
        }
    }

    void MoveTetromino(int x, int y)
    {
        _currentTet.Position += new Vector2(x, y);
        if (HasConflict())
        {
            _currentTet.Position -= new Vector2(x, y);
        }
    }

    void RotateTetromino(bool rotatingRight)
    {
        int[,] oldShape = _currentTet.Shape;
        Vector2 oldPos = _currentTet.Position;

        // Simulates rotation
        _currentTet.Position += _currentTet.RotationToPosDisplace(_currentTet.BlockType, _currentTet.Orientation, rotatingRight);
        _currentTet.Orientation += rotatingRight ? 1 : -1;
        _currentTet.Shape = _currentTet.BlockToShape(_currentTet.BlockType, _currentTet.Orientation);

        // Reverts changes if the rotation is invalid
        if (HasConflict())
        {
            // If wall kick fails, revert rotation
            if (!AttemptWallKick())
            {
                _currentTet.Orientation -= rotatingRight ? 1 : -1;
                _currentTet.Shape = oldShape;
                _currentTet.Position = oldPos;
            }
        }
    }

    // Returns whether a configuration would conflict with the current board
    public bool HasConflict()
    {
        for (int i = 0; i < _currentTet.Shape.GetLength(0); i++)
            for (int j = 0; j < _currentTet.Shape.GetLength(1); j++)
                if (_currentTet.Shape[i, j] != 0)
                    if (_landed[i + (int)_currentTet.Position.y, j + (int)_currentTet.Position.x] != 0)
                        return true;
        return false;
    }

    // Attempts a wall kick, returns false if fails
    public bool AttemptWallKick()
    {
        // Since a simple rotation fails, this attempts all the test cases
        _currentTet.Position = new Vector2(_currentTet.Position.x + 1, _currentTet.Position.y);
        if (HasConflict())
        {
            _currentTet.Position = new Vector2(_currentTet.Position.x, _currentTet.Position.y - 1);
            if (HasConflict())
            {
                _currentTet.Position = new Vector2(_currentTet.Position.x - 1, _currentTet.Position.y + 3);
                if (HasConflict())
                {
                    _currentTet.Position = new Vector2(_currentTet.Position.x + 1, _currentTet.Position.y);
                    if (HasConflict())
                    {
                        _currentTet.Position = new Vector2(_currentTet.Position.x - 1, _currentTet.Position.y - 2);
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public int mapWrapX(int x) => mod(x, _landed.GetLength(1));

    public int mapWrapY(int y) => mod(y, _landed.GetLength(0));

    // Helper function for modulo that works consistently with negative numbers
    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }
    /*
    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width - 100, 10, 100, 20), $"Fast: {_fastTickCount}");
        GUI.Box(new Rect(Screen.width - 100, 35, 100, 20), $"Normal: {_normalTickCount}");
    }
    */
}
