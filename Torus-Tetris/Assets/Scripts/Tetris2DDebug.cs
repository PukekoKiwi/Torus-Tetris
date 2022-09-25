using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetris2DDebug : MonoBehaviour
{
    private int[,] _gameBoard;

    // Start is called before the first frame update
    void Start()
    {
        _gameBoard = GameObject.Find("Torus").GetComponent<TetrisManager>().Landed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private GUIStyle currentStyle = null;

    private void InitStyles()
    {
        if (currentStyle == null)
        {
            currentStyle = new GUIStyle(GUI.skin.box);
            currentStyle.normal.background = MakeTex(2, 2, new Color(1f, 0f, 0f, 0.5f));
        }
    }

    void OnGUI()
    {
        InitStyles();
        GUI.Box(new Rect(Screen.width - 200, 20, 160, 250), "Hello", currentStyle);

        // Renders the current tetris game board
        for (int i = 0; i < _gameBoard.GetLength(0); i++)
        {
            for (int j = 0; j < _gameBoard.GetLength(1); j++)
            {
                // TODO: Make cels here
            }
        }
    }

    // Generates a solid color texture
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
