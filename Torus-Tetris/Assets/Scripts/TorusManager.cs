using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TorusManager : MonoBehaviour
{
    System.Random random = new System.Random();
    Mesh mesh;
    //Points on the center circle
    private Vector3[] centerCirclePoints;
    //Final vertices on the donut
    private Vector3[] vertices;
    //Indexes of triangle coords
    private int[] triangles;

    //Centerpoint of the torus
    Vector3 torusCenter = new Vector3(0f, 0f, 0f);

    private float majorRadius = 1f;
    public int majorSegments = 100;
    private float u = 0f;
    private float minorRadius = .4f;
    public int minorSegments = 50;
    private float v = 0f;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

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
        for (int i = 0; i < centerCirclePoints.Length; i++)
        {
            // Loops through all the points on the minor circle
            for (int j = 0; j < minorSegments; j++)
            {
                // Fetches the point to rotate around the center circle
                Vector3 pointToRotate = vertices[i * minorSegments + j];

                // Rotates the point around the tangent of the center circle at the current circle point
                Vector3 tangent = Vector3.Cross(Vector3.up, centerCirclePoints[i] - torusCenter).normalized;
                pointToRotate = Quaternion.AngleAxis(angle, tangent) * (pointToRotate - centerCirclePoints[i]) + centerCirclePoints[i];

                // Updates the position of the point
                vertices[i * minorSegments + j] = pointToRotate;
            }
        }
        UpdateMesh();
    }

    void CreateShape()
    {
        //List of vertices of the torus
        vertices = new Vector3[majorSegments * minorSegments];
        //List of points around the major circle
        centerCirclePoints = new Vector3[majorSegments];
        //Calculates the points around the circle with the major radius, then calculates the vertices around those points
        var n = 0;
        for (int i = 0; i < majorSegments; i++)
        {
            u += 2 * (float)Math.PI / majorSegments;
            Vector3 temp = new Vector3((float)Math.Cos(u), 0f, (float)Math.Sin(u));
            Vector3 newPoint = torusCenter + majorRadius * temp;
            centerCirclePoints[i] = newPoint;
            //Calculates the vertices around the circle with the minor radius
            v = 0f;
            for (int j = 0; j < minorSegments; j++)
            {
                v += 2 * (float)Math.PI / minorSegments;
                Vector3 subTemp = newPoint - torusCenter;
                Vector3 w = subTemp / (float)Math.Sqrt(subTemp.x * subTemp.x + subTemp.y * subTemp.y + subTemp.z * subTemp.z);
                vertices[n] = newPoint + minorRadius * (float)Math.Cos(v) * w;
                vertices[n].y = minorRadius * (float)Math.Sin(v);
                n++;
            }
        }

        //Appends the triangles on top of the points
        int numOfSegments = minorSegments * majorSegments;
        //The length of the triangle array is the number of segments * number of tris * number of vertices in tris
        triangles = new int[numOfSegments * 3 * 2];
        //Offset of the triangle array index in relation to the minor and major segments
        int minorOffset = 0;
        int majorOffset;
        for (int k = 0; k < majorSegments - 1; k++)
        {
            majorOffset = minorSegments * k;
            for (int m = 0; m < minorSegments - 1; m++)
            {
                triangles[minorOffset] = majorOffset + m + minorSegments + 1;
                triangles[minorOffset + 1] = majorOffset + m + minorSegments;
                triangles[minorOffset + 2] = majorOffset + m;
                triangles[minorOffset + 3] = majorOffset + m;
                triangles[minorOffset + 4] = majorOffset + m + 1;
                triangles[minorOffset + 5] = majorOffset + m + minorSegments + 1;
                minorOffset += 6;
            }
            //Does the last segment to avoid overlap
            triangles[minorOffset] = majorOffset + minorSegments;
            triangles[minorOffset + 1] = majorOffset + minorSegments - 1 + minorSegments;
            triangles[minorOffset + 2] = majorOffset + minorSegments - 1;
            triangles[minorOffset + 3] = majorOffset + minorSegments - 1;
            triangles[minorOffset + 4] = majorOffset;
            triangles[minorOffset + 5] = majorOffset + minorSegments;
            minorOffset += 6;
        }
        //Does the last ring to avoid overlap
        majorOffset = minorSegments * (majorSegments - 1);
        for (int m = 0; m < minorSegments - 1; m++)
        {
            triangles[minorOffset] = m + 1;
            triangles[minorOffset + 1] = m;
            triangles[minorOffset + 2] = majorOffset + m;
            triangles[minorOffset + 3] = majorOffset + m;
            triangles[minorOffset + 4] = majorOffset + m + 1;
            triangles[minorOffset + 5] = m + 1;
            minorOffset += 6;
        }
        //Does the last segment to avoid overlap
        triangles[minorOffset] = 0;
        triangles[minorOffset + 1] = minorSegments - 1;
        triangles[minorOffset + 2] = numOfSegments - 1;
        triangles[minorOffset + 3] = numOfSegments - 1;
        triangles[minorOffset + 4] = numOfSegments - minorSegments;
        triangles[minorOffset + 5] = 0;
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
