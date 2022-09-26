using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        for (int i = 0; i < _landed.GetLength(0); i++)
        {
            _landed[i, 0] = 1;
        }
        for (int i = 0; i < _landed.GetLength(1); i++)
        {
            _landed[2, i] = 1;
        }

        // Tetmap initialization
        InitializeTetMap();

        // Spawns a tetromino at the top left
        _currentTet = new Tetromino(_tetMap['S'], new Vector2(2, 2));
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
    }

    private void InitializeTetMap()
    {
        _tetMap.Add('I', new int[,] {{ 2, 2, 2, 2 }});
        _tetMap.Add('J', new int[,] {{ 3, 0, 0 },
                                     { 3, 3, 3 }});
        _tetMap.Add('L', new int[,] {{ 0, 0, 4 },
                                     { 4, 4, 4 }});
        _tetMap.Add('O', new int[,] {{ 5, 5 },
                                     { 5, 5 }});
        _tetMap.Add('S', new int[,] {{ 0, 6, 6 },
                                     { 6, 6, 0 }});
        _tetMap.Add('T', new int[,] {{ 0, 7, 0 },
                                     { 7, 7, 7 }});
        _tetMap.Add('Z', new int[,] {{ 8, 8, 0 },
                                     { 0, 8, 8 }});
    }

    /*
    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width - 100, 10, 100, 20), $"Fast: {_fastTickCount}");
        GUI.Box(new Rect(Screen.width - 100, 35, 100, 20), $"Normal: {_normalTickCount}");
    }
    */
}
