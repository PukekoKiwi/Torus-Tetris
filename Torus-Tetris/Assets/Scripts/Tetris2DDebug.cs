using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public class Tetris2DDebug : MonoBehaviour
{
    private Vector2 _position;
    private int[,] _gameBoard;
    private int _majorSegments;
    private int _minorSegments;
    private Vector2 _mapScale;
    private Dictionary<int, Color> _colorMap = new Dictionary<int, Color>();

    // Start is called before the first frame update
    void Start()
    {
        _position = new Vector2(10, 10);
        _gameBoard = GameObject.Find("Torus").GetComponent<TetrisManager>().Landed;
        _majorSegments = _gameBoard.GetLength(0);
        _minorSegments = _gameBoard.GetLength(1);
        _mapScale = new Vector2(10f, 10f);

        // Assigns values in color map
        _colorMap.Add(0, new Color(0, 0, 0));       // Free tile
        _colorMap.Add(1, new Color(.95f, .95f, .95f)); // Obstacle
        _colorMap.Add(2, new Color(0, .95f, .95f));   // I Block (Cyan)
        _colorMap.Add(3, new Color(0, 0, .95f));     // J Block (Blue)
        _colorMap.Add(4, new Color(.95f, .625f, 0));   // L Block (Orange)
        _colorMap.Add(5, new Color(.95f, .95f, 0));   // O Block (Yellow)
        _colorMap.Add(6, new Color(0, .95f, 0));     // S Block (Green)
        _colorMap.Add(7, new Color(.625f, 0, .95f));   // T Block (Purple)
        _colorMap.Add(8, new Color(.95f, 0, 0));     // Z Block (Red)
    }

    private GUIStyle currentStyle = null;

    void OnGUI()
    {
        _gameBoard = GameObject.Find("Torus").GetComponent<TetrisManager>().Landed;
        currentStyle = new GUIStyle(GUI.skin.box);
        currentStyle.normal.background = MakeTex();

        GUI.Box(new Rect(_position.x, _position.y, _minorSegments * _mapScale.x, _majorSegments * _mapScale.y), "", currentStyle);
    }

    // Generates a texture representative of a 2D tetris game
    private Texture2D MakeTex()
    {
        // Creates a list of pixels to be assigned to the texture
        Color[] pix = new Color[_minorSegments * _majorSegments];

        // Renders the current tetris game board
        for (int i = 0; i < _majorSegments; i++)
        {
            for (int j = 0; j < _minorSegments; j++)
            {
                Color pixelColor = _colorMap[_gameBoard[_majorSegments - i - 1, j]];
                pixelColor.a = .8f;
                pix[i * _minorSegments + j] = pixelColor;
            }
        }
        Texture2D result = new Texture2D(_minorSegments, _majorSegments);
        result.SetPixels(pix);
        result.Apply();

        return Resize(result, _minorSegments * (int)_mapScale.x, _majorSegments * (int)_mapScale.y);
    }

    public Texture2D Resize(Texture2D source, int newWidth, int newHeight)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Point;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D nTex = new Texture2D(newWidth, newHeight);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        nTex.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return nTex;
    }
}
