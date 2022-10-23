using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class NoiseSettings
{
    public int octaves;
    public float lacunarity;
    public float persistence;
    public float hightNoiseScale;
    public float maxHeight;

    public float caveNoiseScale;
    public float caveNoiseCutoff;

}

[ExecuteInEditMode]
public class MarchingMesh : MonoBehaviour
{
    int numPointsPerAxis;

    public Material mat;
    public ComputeShader shader;
    public ComputeShader densityShader;

    public ComputeShader mineShader;


    [Header("Voxel Settings")]
    public float isoLevel;
    
    public float MetersBetweenPoints;

    float radius;

    [Header("Chunk Settings")]
    public float chunkWidth;
    public bool DrawGizmos;
    bool cachedG;

    [Header("Color Settings")]
    public Color SlopeColor;
    public Color FlatColor;
    public Color RockColor;
    public float noiseFrequency;

    [Header("Noise Settings")]
    public NoiseSettings noiseSettings;

    int chunksPerAxis;
    int pointsPerChunkAxis;

    public bool UpdateMesh;

    public bool UpdateShader;

    public bool generateCollision;
    bool cachedCol;
    Chunk[] storedChunks;

    public void OnEnable()
    {
        if (Application.isPlaying)
        {
            mat = new Material(mat);
            radius = transform.parent.GetComponent<PlanetLOD>().radius;
            UpdateMesh = false;
            genChunks();
            UpdateMaterial(mat);
        }
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            UpdatePos();
        }
        if (!Application.isPlaying)
        {
            if (UpdateMesh)
            {
                mat = new Material(mat);
                radius = transform.parent.GetComponent<PlanetLOD>().radius;
                UpdateMesh = false;
                genChunks();
            }
            if (UpdateShader)
            {
                UpdateShader = false;
                UpdateMaterial(mat);
            }
            if (cachedG != DrawGizmos)
            {
                cachedG = DrawGizmos;
                foreach (Transform c in transform)
                {
                    var chunk = c.GetComponent<Chunk>();
                    if (chunk != null)
                    {
                        chunk.UpdateGizmo(DrawGizmos);
                    }
                }
            }
        }
    }


    private void killChildren()
    {
        int childs = transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            
            transform.GetChild(i).gameObject.GetComponent<Chunk>()?.OnDestroy();
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public void genChunks()
    {
        killChildren();
        
        cachedCol = false;
        chunksPerAxis = Mathf.CeilToInt(radius * 2 / chunkWidth);

        numPointsPerAxis = Mathf.CeilToInt(radius * 2f / MetersBetweenPoints);

        numPointsPerAxis += chunksPerAxis - numPointsPerAxis % chunksPerAxis;

        pointsPerChunkAxis = numPointsPerAxis / chunksPerAxis;

        storedChunks = new Chunk[chunksPerAxis*chunksPerAxis*chunksPerAxis];

        for (int i = 0; i< chunksPerAxis; i++)
        {
            for (int j = 0; j < chunksPerAxis; j++)
            {
                for (int k = 0; k < chunksPerAxis; k++)
                {
                   
                    Vector3 offset = new Vector3(i  - 1f * chunksPerAxis / 2, j  - 1f  * chunksPerAxis / 2, k  - 1f  * chunksPerAxis / 2)*MetersBetweenPoints * pointsPerChunkAxis;
                    Vector3 center = new Vector3(1,1,1)*radius/2;
                    string cName = name + " chunk " + i + " " + j + " " + k;


                    GameObject chunkGO = new GameObject(cName);

                    chunkGO.transform.parent = transform;
                    Chunk chunk = chunkGO.AddComponent<Chunk>();
                    chunkGO.layer = 7;
                    chunkGO.tag = "Terrain";
                    chunk.Setup(offset, center, isoLevel, radius, MetersBetweenPoints, pointsPerChunkAxis+1, mat, shader, densityShader, mineShader, DrawGizmos, noiseSettings);
                    chunkGO.transform.position = transform.position+offset;
                    storedChunks[i* chunksPerAxis * chunksPerAxis+ k* chunksPerAxis+ j] = chunk;
                }
            }
        }
    }

    public void Mine(Vector3 pos, float radius, float strength)
    {
        float width = storedChunks[0].SideLength();
        Bounds bounds = new Bounds(Vector3.zero, Vector3.one * (width+radius));
        foreach(Chunk chunk in storedChunks)
        {
            Vector3 centre = chunk.transform.position + new Vector3(1, 1, 1) * (pointsPerChunkAxis) * MetersBetweenPoints / 2;
            
            bounds.center = centre;
            if (bounds.Contains(pos)){
                chunk.Mine(pos, radius, strength);
            }
        }
        
    }


    public void UpdateMaterial(Material mat)
    {
        mat.SetVector("PlanetPosition", transform.position);
        mat.SetFloat("Radius", radius);
        mat.SetFloat("MaxHieght", noiseSettings.maxHeight);
        mat.SetFloat("NoiseFrequency", noiseFrequency);

        mat.SetColor("RockColor", RockColor);
        mat.SetColor("SlopeColor", SlopeColor);
        mat.SetColor("FlatColor", FlatColor);

        mat.SetVector("PlanetPosition", transform.position);
    }

    private void UpdatePos()
    {
        mat.SetVector("PlanetPosition", transform.position);
    }
}
