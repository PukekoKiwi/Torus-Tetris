using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds data necessary for a tetromino
public class Tetromino
{
    private int[,] _shape;
    private Vector2 _position;
    private Vector2 _forecastPos;

    // Properties
    public int[,] Shape { get => _shape; set => _shape = value; }
    public int Width { get => _shape.GetLength(1); }
    public int Height { get => _shape.GetLength(0); }
    public Vector2 Position { get => _position; set => _position = value; }
    public Vector2 ForecastPos { get => _forecastPos; set => _forecastPos = value; }

    // Constructor
    public Tetromino(int[,] shape, Vector2 position)
    {
        _shape = shape;
        _position = position;
        _forecastPos = new Vector2(position.x, position.y + 1);
    }
}
