using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.PackageManager;
using UnityEngine;

public enum Block
{
    I = 2,
    J,
    L,
    O,
    S,
    T,
    Z
}

// Holds data necessary for a tetromino
public class Tetromino
{
    private Block _blockType;
    private int[,] _shape;
    private int _orientation; // 0 - Default, Upright S
    private Vector2 _position;

    // Properties
    public Block BlockType { get => _blockType; }
    public int[,] Shape { get => _shape; set => _shape = value; }
    public int Orientation { get => _orientation; set => _orientation = mod(value, 4); }
    public int Width { get => _shape.GetLength(1); }
    public int Height { get => _shape.GetLength(0); }
    public Vector2 Position { get => _position; set => _position = value; }

    // Constructor
    public Tetromino(Block block, Vector2 position)
    {
        _blockType = block;
        _shape = BlockToShape(block, 0);
        _orientation = 0;
        _position = position;
    }

    public int[,] BlockToShape(Block block, int orientation) => block switch
	{
        Block.I => orientation switch
                    {
                        0 or 2 => new int[,] { { 2, 2, 2, 2 } },
                        1 or 3 => new int[,] { { 2 },
                                               { 2 },
                                               { 2 },
                                               { 2 }
                        },
                        _ => throw new ArgumentOutOfRangeException(nameof(orientation), $"Unknown orientation value: {orientation}")
                    },
        Block.J => orientation switch
                    {
                        0 => new int[,] {{ 3, 0, 0 },
                                         { 3, 3, 3 }},
                        1 => new int[,] {{ 3, 3 },
                                         { 3, 0 },
                                         { 3, 0 }},
                        2 => new int[,] {{ 3, 3, 3 },
                                         { 0, 0, 3 }},
                        3 => new int[,] {{ 0, 3 },
                                         { 0, 3 },
                                         { 3, 3 }},
                        _ => throw new ArgumentOutOfRangeException(nameof(orientation), $"Unknown orientation value: {orientation}")
                    },
        Block.L => orientation switch
                    {
                        0 => new int[,] {{ 4, 4, 4 },
                                         { 4, 0, 0 }},
                        1 => new int[,] {{ 4, 4 },
                                         { 0, 4 },
                                         { 0, 4 }},
                        2 => new int[,] {{ 0, 0, 4 },
                                         { 4, 4, 4 }},
                        3 => new int[,] {{ 4, 0 },
                                         { 4, 0 },
                                         { 4, 4 }},
                        _ => throw new ArgumentOutOfRangeException(nameof(orientation), $"Unknown orientation value: {orientation}")
                    },
        Block.O => new int[,] {{ 5, 5 },
                               { 5, 5 }},
        Block.S => orientation switch
                    {
                        0 or 2 => new int[,] {{ 0, 6, 6 },
                                              { 6, 6, 0 }},
                        1 or 3 => new int[,] {{ 6, 0 },
                                              { 6, 6 },
                                              { 0, 6 }},
                        _ => throw new ArgumentOutOfRangeException(nameof(orientation), $"Unknown orientation value: {orientation}")
                    },
        Block.T => orientation switch
                    {
                        0 => new int[,] {{ 0, 7, 0 },
                                         { 7, 7, 7 }},
                        1 => new int[,] {{ 7, 0 },
                                         { 7, 7 },
                                         { 7, 0 }},
                        2 => new int[,] {{ 7, 7, 7 },
                                         { 0, 7, 0 }},
                        3 => new int[,] {{ 0, 7 },
                                         { 7, 7 },
                                         { 0, 7 }},
                        _ => throw new ArgumentOutOfRangeException(nameof(orientation), $"Unknown orientation value: {orientation}")
                    },
        Block.Z => orientation switch
                    {
                        0 or 2 => new int[,] {{ 8, 8, 0 },
                                              { 0, 8, 8 }},
                        1 or 3 => new int[,] {{ 0, 8 },
                                              { 8, 8 },
                                              { 8, 0 }},
                        _ => throw new ArgumentOutOfRangeException(nameof(orientation), $"Unknown orientation value: {orientation}")
                    },
        _ => throw new ArgumentOutOfRangeException(nameof(block), $"Unknown block value: {block}")
    };

    public Vector2 RotationToPosDisplace(Block block, int orientation, bool rotatingRight) => block switch
    {
        Block.I => orientation switch
        {
            0 => rotatingRight ? new Vector2(2, -1) : new Vector2(1, -1),
            1 => rotatingRight ? new Vector2(-2, 2) : new Vector2(-2, 1),
            2 => rotatingRight ? new Vector2(1, -2) : new Vector2(2, -2),
            3 => rotatingRight ? new Vector2(-1, 1) : new Vector2(-1, 2),
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), $"Unknown orientation value: {orientation}")
        },
        Block.J or Block.L or Block.S or Block.T or Block.Z => orientation switch
        {
            0 => rotatingRight ? new Vector2(1, 0) : new Vector2(0, 0),
            1 => rotatingRight ? new Vector2(-1, 1) : new Vector2(-1, 0),
            2 => rotatingRight ? new Vector2(0, -1) : new Vector2(1, -1),
            3 => rotatingRight ? new Vector2(0, 0) : new Vector2(0, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), $"Unknown orientation value: {orientation}")
        },
        Block.O => new Vector2(0, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(block), $"Unknown block value: {block}")
    };

    // Helper function for modulo that works consistently with negative numbers
    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }
}
