using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TorusManager : MonoBehaviour
{
    private Mesh _mesh;
    //Points on the center circle
    private Vector3[] _centerCirclePoints;
    //Final vertices on the donut
    private Vector3[] _vertices;
    //Indexes of triangle coords
    private int[] _triangles;

    //Centerpoint of the torus
    [SerializeField] private Vector3 _torusCenter = new Vector3(0f, 0f, 0f);

    [SerializeField] private float _majorRadius = 1f;
    [SerializeField] private int _majorSegments = 24;
    private float _u = 0f;
    [SerializeField] private float _minorRadius = .4f;
    [SerializeField] private int _minorSegments = 12;
    private float _v = 0f;

    // Properties
    public float MajorRadius { get => _majorRadius; set => _majorRadius = value; }
    public int MajorSegments { get => _majorSegments; set => _majorSegments = value; }
    public float MinorRadius { get => _minorRadius; set => _minorRadius = value; }
    public int MinorSegments { get => _minorSegments; set => _minorSegments = value; }

    // Start is called before the first frame update
    void Start()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        CreateShape();
        UpdateMesh();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.up * 90f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(-Vector3.up * 90f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            RotateOnCenterCircle(90f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            RotateOnCenterCircle(-90f * Time.deltaTime);
        }
    }

    void RotateOnCenterCircle(float angle)
    {
        // Loops through each segment of the center circle
        for (int i = 0; i < _centerCirclePoints.Length; i++)
        {
            // Loops through all the points on the minor circle
            for (int j = 0; j < _minorSegments; j++)
            {
                // Fetches the point to rotate around the center circle
                Vector3 pointToRotate = _vertices[i * _minorSegments + j];

                // Rotates the point around the tangent of the center circle at the current circle point
                Vector3 tangent = Vector3.Cross(Vector3.up, _centerCirclePoints[i] - _torusCenter).normalized;
                pointToRotate = Quaternion.AngleAxis(angle, tangent) * (pointToRotate - _centerCirclePoints[i]) + _centerCirclePoints[i];

                // Updates the position of the point
                _vertices[i * _minorSegments + j] = pointToRotate;
            }
        }
        UpdateMesh();
    }

    void CreateShape()
    {
        //List of vertices of the torus
        _vertices = new Vector3[_majorSegments * _minorSegments];
        //List of points around the major circle
        _centerCirclePoints = new Vector3[_majorSegments];
        //Calculates the points around the circle with the major radius, then calculates the vertices around those points
        var n = 0;
        for (int i = 0; i < _majorSegments; i++)
        {
            _u += 2 * (float)Math.PI / _majorSegments;
            Vector3 temp = new Vector3((float)Math.Cos(_u), 0f, (float)Math.Sin(_u));
            Vector3 newPoint = _torusCenter + _majorRadius * temp;
            _centerCirclePoints[i] = newPoint;
            //Calculates the vertices around the circle with the minor radius
            _v = 0f;
            for (int j = 0; j < _minorSegments; j++)
            {
                _v += 2 * (float)Math.PI / _minorSegments;
                Vector3 subTemp = newPoint - _torusCenter;
                Vector3 w = subTemp / (float)Math.Sqrt(subTemp.x * subTemp.x + subTemp.y * subTemp.y + subTemp.z * subTemp.z);
                _vertices[n] = newPoint + _minorRadius * (float)Math.Cos(_v) * w;
                _vertices[n].y = _minorRadius * (float)Math.Sin(_v);
                n++;
            }
        }

        //Appends the triangles on top of the points
        int numOfSegments = _minorSegments * _majorSegments;
        //The length of the triangle array is the number of segments * number of tris * number of vertices in tris
        _triangles = new int[numOfSegments * 3 * 2];
        //Offset of the triangle array index in relation to the minor and major segments
        int minorOffset = 0;
        int majorOffset;
        for (int k = 0; k < _majorSegments - 1; k++)
        {
            majorOffset = _minorSegments * k;
            for (int m = 0; m < _minorSegments - 1; m++)
            {
                _triangles[minorOffset] = majorOffset + m + _minorSegments + 1;
                _triangles[minorOffset + 1] = majorOffset + m + _minorSegments;
                _triangles[minorOffset + 2] = majorOffset + m;
                _triangles[minorOffset + 3] = majorOffset + m;
                _triangles[minorOffset + 4] = majorOffset + m + 1;
                _triangles[minorOffset + 5] = majorOffset + m + _minorSegments + 1;
                minorOffset += 6;
            }
            //Does the last segment to avoid overlap
            _triangles[minorOffset] = majorOffset + _minorSegments;
            _triangles[minorOffset + 1] = majorOffset + _minorSegments - 1 + _minorSegments;
            _triangles[minorOffset + 2] = majorOffset + _minorSegments - 1;
            _triangles[minorOffset + 3] = majorOffset + _minorSegments - 1;
            _triangles[minorOffset + 4] = majorOffset;
            _triangles[minorOffset + 5] = majorOffset + _minorSegments;
            minorOffset += 6;
        }
        //Does the last ring to avoid overlap
        majorOffset = _minorSegments * (_majorSegments - 1);
        for (int m = 0; m < _minorSegments - 1; m++)
        {
            _triangles[minorOffset] = m + 1;
            _triangles[minorOffset + 1] = m;
            _triangles[minorOffset + 2] = majorOffset + m;
            _triangles[minorOffset + 3] = majorOffset + m;
            _triangles[minorOffset + 4] = majorOffset + m + 1;
            _triangles[minorOffset + 5] = m + 1;
            minorOffset += 6;
        }
        //Does the last segment to avoid overlap
        _triangles[minorOffset] = 0;
        _triangles[minorOffset + 1] = _minorSegments - 1;
        _triangles[minorOffset + 2] = numOfSegments - 1;
        _triangles[minorOffset + 3] = numOfSegments - 1;
        _triangles[minorOffset + 4] = numOfSegments - _minorSegments;
        _triangles[minorOffset + 5] = 0;
    }

    void UpdateMesh()
    {
        _mesh.Clear();

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;

        _mesh.RecalculateNormals();
    }
}
