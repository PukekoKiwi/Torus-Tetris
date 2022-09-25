using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class TetrisManager : MonoBehaviour
{
    private int _majorSegments;
    private int _minorSegments;
    private int[,] _landed;

    // Properties
    public int[,] Landed { get => _landed; set => _landed = value; }

    // Start is called before the first frame update
    void Start()
    {
        _majorSegments = GetComponent<TorusManager>().MajorSegments;
        _minorSegments = GetComponent<TorusManager>().MinorSegments;
        // Creates 2D array for tetris representation
        _landed = new int[_majorSegments, _minorSegments];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
