using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;
using Random = UnityEngine.Random;

public class TetrisManager : MonoBehaviour
{
    private int _majorSegments;
    private int _minorSegments;
    private int[,] _landed;

    private float _elapsedFrames = 0f;
    private float _framesPerTick = 20f;
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

        for (int i = 0; i < _minorSegments; i++)
        {
            _landed[_majorSegments - 1, i] = 1;
        }

        // Spawns a tetromino at the top left
        _currentTet = new Tetromino(Block.T, new Vector2(2, 2));
    }

    // Update is called once per frame
    void Update()
    {

        // Executes functions on normal ticks
        _elapsedFrames += 1;
        if (_elapsedFrames >= _framesPerTick)
        {
            _elapsedFrames = _elapsedFrames % _framesPerTick;
            // Do things with each tick here
            MoveTetromino(0, 1);

            // TODO: implement gravity switching
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

        if (Input.GetKeyDown(KeyCode.UpArrow))
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
        _currentTet.Position = new Vector2(MapWrapX((int)_currentTet.Position.x + x), MapWrapY((int)_currentTet.Position.y + y));
        if (HasConflict())
        {
            _currentTet.Position = new Vector2(MapWrapX((int)_currentTet.Position.x - x), MapWrapY((int)_currentTet.Position.y - y));
            if (y > 0)
                LockTetromino();
        }
    }

    void RotateTetromino(bool rotatingRight)
    {
        int[,] oldShape = _currentTet.Shape;
        Vector2 oldPos = _currentTet.Position;

        // Simulates rotation
        Vector2 posDisplace = _currentTet.RotationToPosDisplace(_currentTet.BlockType, _currentTet.Orientation, rotatingRight);
        _currentTet.Position = new Vector2(MapWrapX((int)_currentTet.Position.x + (int)posDisplace.x), MapWrapY((int)_currentTet.Position.y + (int)posDisplace.y));
        _currentTet.Orientation += rotatingRight ? 1 : -1;
        _currentTet.Shape = _currentTet.BlockToShape(_currentTet.BlockType, _currentTet.Orientation);

        // Reverts changes if the rotation is invalid
        if (HasConflict())
        {
            // If wall kick fails, revert rotation
            if (!AttemptWallKick(rotatingRight))
            {
                _currentTet.Orientation -= rotatingRight ? 1 : -1;
                _currentTet.Shape = oldShape;
                _currentTet.Position = oldPos;
            }
        }
    }

    void LockTetromino()
    {
        // Transfers the tetromino to the board
        for (int i = 0; i < _currentTet.Height; i++)
            for (int j = 0; j < _currentTet.Width; j++)
                if (_currentTet.Shape[i, j] != 0)
                    _landed[MapWrapY(i + (int)_currentTet.Position.y), MapWrapX(j + (int)_currentTet.Position.x)] = _currentTet.Shape[i, j];
        SpawnTet();
    }

    void SpawnTet()
    {
        _currentTet = new Tetromino((Block)Random.Range(2, 8), new Vector2(2, 2));
    }

    // Returns whether a configuration would conflict with the current board
    public bool HasConflict()
    {
        for (int i = 0; i < _currentTet.Height; i++)
            for (int j = 0; j < _currentTet.Width; j++)
                if (_currentTet.Shape[i, j] != 0)
                    if (_landed[MapWrapY(i + (int)_currentTet.Position.y), MapWrapX(j + (int)_currentTet.Position.x)] != 0)
                        return true;
        return false;
    }

    // Attempts a wall kick, returns false if fails
    public bool AttemptWallKick(bool rotatingRight)
    {
        // Gets the right configuration of test cases
        Vector2[] testCases = new Vector2[4];
        if (_currentTet.BlockType == Block.I)
        {
            switch (_currentTet.Orientation)
            {
                case 0:
                    testCases = rotatingRight ? (new Vector2[] { new Vector2(1, 0), new Vector2(-2, 0), new Vector2(1, 2), new Vector2(-2, -1) }) : (new Vector2[] { new Vector2(2, 0), new Vector2(-1, 0), new Vector2(2, -1), new Vector2(-1, 2) });
                    break;
                case 1:
                    testCases = rotatingRight ? (new Vector2[] { new Vector2(-2, 0), new Vector2(1, 0), new Vector2(-2, 1), new Vector2(1, -2) }) : (new Vector2[] { new Vector2(1, 0), new Vector2(-2, 0), new Vector2(1, 2), new Vector2(-2, -1) });
                    break;
                case 2:
                    testCases = rotatingRight ? (new Vector2[] { new Vector2(-1, 0), new Vector2(2, 0), new Vector2(-1, -2), new Vector2(1, 2) }) : (new Vector2[] { new Vector2(-2, 0), new Vector2(1, 0), new Vector2(-2, 1), new Vector2(1, -2) });
                    break;
                case 3:
                    testCases = rotatingRight ? (new Vector2[] { new Vector2(2, 0), new Vector2(-1, 0), new Vector2(2, -1), new Vector2(-1, 2) }) : (new Vector2[] { new Vector2(-1, 0), new Vector2(2, 0), new Vector2(-1, -2), new Vector2(2, 1) });
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (_currentTet.Orientation)
            {
                case 0:
                    testCases = rotatingRight ? (new Vector2[] { new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, -2), new Vector2(-1, -2) }) : (new Vector2[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, -2), new Vector2(1, -2) });
                    break;
                case 1:
                    testCases = rotatingRight ? (new Vector2[] { new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0, 2), new Vector2(-1, 2) }) : (new Vector2[] { new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0, 1), new Vector2(-1, 2) });
                    break;
                case 2:
                    testCases = rotatingRight ? (new Vector2[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, -2), new Vector2(1, -2) }) : (new Vector2[] { new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, -2), new Vector2(-1, -2) });
                    break;
                case 3:
                    testCases = rotatingRight ? (new Vector2[] { new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, 2), new Vector2(1, 2) }) : (new Vector2[] { new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, 2), new Vector2(1, 2) });
                    break;
                default:
                    break;
            }
        }

        // Runs through test cases, applies earliest one that works
        foreach (Vector2 testCase in testCases)
        {
            Vector2 oldPos = _currentTet.Position;
            _currentTet.Position = new Vector2(MapWrapX((int)_currentTet.Position.x + (int)testCase.x), MapWrapY((int)_currentTet.Position.y + (int)testCase.y));
            if (HasConflict())
                _currentTet.Position = oldPos;
            else
                return true;
        }
        return false;
    }

    public int MapWrapX(int x) => mod(x, _landed.GetLength(1));
    public int MapWrapY(int y) => mod(y, _landed.GetLength(0));

    // Helper function for modulo that works consistently with negative numbers
    int mod(int x, int m) => (x % m + m) % m;
    
    /*
    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width - 100, 10, 100, 20), $"Fast: {_fastTickCount}");
        GUI.Box(new Rect(Screen.width - 100, 35, 100, 20), $"Normal: {_normalTickCount}");
    }
    */
}
