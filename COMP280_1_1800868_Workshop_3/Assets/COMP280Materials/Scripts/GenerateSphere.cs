﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The code below came from: http://wiki.unity3d.com/index.php/ProceduralPrimitives and has been modified 

[RequireComponent(typeof(MeshFilter))]
public class GenerateSphere : MonoBehaviour
{
    [SerializeField]
    private float radius = 1f;

    // Longitude |||
    [SerializeField]
    public int nbLong = 24;

    // Latitude ---
    [SerializeField]
    public int nbLat = 16;

    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = new Mesh();
        mesh.name = "GeneratedMesh";

        GetComponent<MeshFilter>().mesh = mesh;

        #region Vertices
        Vector3[] vertices = new Vector3[(nbLong + 1) * nbLat + 2];
        float _pi = Mathf.PI;
        float _2pi = _pi * 2f;

        vertices[0] = Vector3.up * radius;
        for (int lat = 0; lat < nbLat; lat++)
        {
            // Manipulate height of sphere
            float a1 = _pi * (float)(lat + 2) / (nbLat + 1);
            float sin1 = Mathf.Sin(a1);
            float cos1 = Mathf.Cos(a1);

            for (int lon = 0; lon <= nbLong; lon++)
            {
                // Manipulate width of sphere
                float a2 = _2pi * (float)(lon == nbLong ? 0 : lon) / nbLong;
                float sin2 = Mathf.Sin(a2);
                float cos2 = Mathf.Cos(a2);

                //Sets position of each vertex based on longistude and latitude
                //Applied perlin noise to affect phsycial shape of sphere
                vertices[lon + lat * (nbLong + 1)] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * Mathf.PerlinNoise(Random.Range(sin1/2, cos1/2), Random.Range(cos1/2, cos2/2)) * radius;
            }
        }

        vertices[vertices.Length - 1] = Vector3.up * -radius;
        #endregion

        //Set the normal value for each vertex
        #region Normales		
        Vector3[] normales = new Vector3[vertices.Length];
        for (int n = 0; n < vertices.Length; n++)
            normales[n] = vertices[n].normalized;
        #endregion

        //Set up UV Mapping
        #region UVs
        Vector2[] uvs = new Vector2[vertices.Length];
        uvs[0] = Vector2.up;
        uvs[uvs.Length - 1] = Vector2.zero;
        for (int lat = 0; lat < nbLat; lat++)
            for (int lon = 0; lon <= nbLong; lon++)
                uvs[lon + lat * (nbLong + 1) + 1] = new Vector2((float)lon / nbLong, 1f - (float)(lat + 1) / (nbLat + 1));
        #endregion

        #region Triangles
        int nbFaces = vertices.Length;
        int nbTriangles = nbFaces * 2;
        int nbIndexes = nbTriangles * 3;
        int[] triangles = new int[nbIndexes];

        //Top Cap
        int i = 0;
        for (int lon = 0; lon < nbLong; lon++)
        {
            triangles[i++] = lon + 2;
            triangles[i++] = lon + 1;
            triangles[i++] = 0;
        }

        //Middle
        for (int lat = 0; lat < nbLat - 1; lat++)
        {
            for (int lon = 0; lon < nbLong; lon++)
            {
                int current = lon + lat * (nbLong) + 1;
                int next = current + nbLong + 1;

                /*if (lat == vertices.Length - 4)
                    current = lon + lat * (nbLong + 1) + 1;*/

                triangles[i++] = current;
                triangles[i++] = current + 1;
                triangles[i++] = next + 1;

                triangles[i++] = current;
                triangles[i++] = next + 1;
                triangles[i++] = next;

            }
        }

        //Bottom Cap
        for (int lon = 0; lon < nbLong; lon++)
        {
            triangles[i++] = vertices.Length - 1;
            triangles[i++] = vertices.Length - (lon + 2) - 1;
            triangles[i++] = vertices.Length - (lon + 1) - 1;
        }
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }
}
