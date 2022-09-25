using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds data necessary for a tetromino
public class Tetromino
{
    private int[] _shape;
    private Vector2 _position;

    // Properties
    public int[] Shape { get => _shape; set => _shape = value; }
    public Vector2 Position { get => _position; set => _position = value; }

    // Constructor
    public Tetromino(int[] shape, Vector2 position)
    {
        _shape = shape;
        _position = position;
    }
}
