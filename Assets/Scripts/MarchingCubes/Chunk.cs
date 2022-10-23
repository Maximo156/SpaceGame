using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    Material mat;
    const int threadGroupSize = 8;
     int numPointsPerAxis;

    ComputeShader shader;
    ComputeShader densityShader;
    ComputeShader mineShader;

    float isoLevel;
    float radius;
    float MetersBetweenPoints;

    // Buffers
    ComputeBuffer triangleBuffer;
    ComputeBuffer pointsBuffer;
    ComputeBuffer triCountBuffer;

    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    Vector3 offset;
    Vector3 PlanetCenter;

    NoiseSettings noise;

    bool draw;

    List<GameObject> localAssets = new List<GameObject>();

    private void Start()
    {
        if (Application.isPlaying)
        {
            ReleaseBuffers();
            CreateBuffers();

            SetDensity();


            UpdateChunkMesh();

        }
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            ReleaseBuffers();
        }
    }

    public void Setup(Vector3 o, Vector3 c, float i, float r, float m, int n, Material mat, ComputeShader s, ComputeShader d, ComputeShader mineShader, bool g, NoiseSettings noise)
    {
        PlanetCenter = c;
        offset = o;
        isoLevel = i;
        radius = r;
        MetersBetweenPoints = m;
        numPointsPerAxis = n;
        this.mat = mat;
        shader = s;
        densityShader = d;
        draw = g;
        this.mineShader = mineShader;
        this.noise = noise;

        SetupMesh();

        CreateBuffers();

        SetDensity();

        UpdateChunkMesh();


    }

    private void OnApplicationQuit()
    {
        ReleaseBuffers();
    }

    void ReleaseBuffers()
    {
        if (triangleBuffer != null)
        {
            triangleBuffer.Release();
            pointsBuffer.Release();
            triCountBuffer.Release();

            triangleBuffer = null;
            pointsBuffer = null;
            triCountBuffer = null;

        }
    }

    public void OnDestroy()
    {
        if (!Application.isPlaying)
        {
            ReleaseBuffers();
        }
    }

    private void SetupMesh()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;

        mesh = meshFilter.sharedMesh;

        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshFilter.sharedMesh = mesh;

        meshRenderer.material = mat;
        meshCollider.sharedMesh = mesh;
    }

    void CreateBuffers()
    {
        int numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
        int maxTriangleCount = numVoxels * 5;

        // Always create buffers in editor (since buffers are released immediately to prevent memory leak)
        // Otherwise, only create if null or if size has changed
        if (!Application.isPlaying || (pointsBuffer == null || numPoints != pointsBuffer.count))
        {
            if (Application.isPlaying)
            {
                ReleaseBuffers();
            }
            triangleBuffer = new ComputeBuffer(maxTriangleCount, sizeof(float) * 3 * 3, ComputeBufferType.Append);
            pointsBuffer = new ComputeBuffer(numPoints, sizeof(float) * 4);
            triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        }
    }

    private void SetDensity()
    {
        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)threadGroupSize);

        densityShader.SetInt("octaves", noise.octaves);
        densityShader.SetFloat("lacunarity", noise.lacunarity);
        densityShader.SetFloat("persistence", noise.persistence);
        densityShader.SetFloat("hightNoiseScale", noise.hightNoiseScale);
        densityShader.SetFloat("caveNoiseScale", noise.caveNoiseScale);
        densityShader.SetFloat("caveNoiseCutoff", noise.caveNoiseCutoff);
        densityShader.SetFloat("maxHeight", noise.maxHeight);

        densityShader.SetBuffer(0, "points", pointsBuffer);
        densityShader.SetInt("numPointsPerAxis", numPointsPerAxis);
        densityShader.SetFloat("radius", radius);
        densityShader.SetFloat("MetersBetweenPoints", MetersBetweenPoints);
        densityShader.SetVector("offset", offset);
        densityShader.SetVector("center", PlanetCenter);

        densityShader.SetVector("location", transform.parent.position);

        densityShader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);
    }



    public void UpdateChunkMesh()
    {
        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)threadGroupSize);


        triangleBuffer.SetCounterValue(0);
        shader.SetBuffer(0, "points", pointsBuffer);
        shader.SetBuffer(0, "triangles", triangleBuffer);
        shader.SetInt("numPointsPerAxis", numPointsPerAxis);
        shader.SetFloat("isoLevel", isoLevel);

        shader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

        // Get number of triangles in the triangle buffer
        ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);
        int[] triCountArray = { 0 };
        triCountBuffer.GetData(triCountArray);
        int numTris = triCountArray[0];

        // Get triangle data from shader
        Triangle[] tris = new Triangle[numTris];
        triangleBuffer.GetData(tris, 0, 0, numTris);

        mesh.Clear();

        var vertices = new Vector3[numTris * 3];
        var meshTriangles = new int[numTris * 3];

        for (int i = 0; i < numTris; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                meshTriangles[i * 3 + j] = i * 3 + j;
                vertices[i * 3 + j] = tris[i][j];
            }
        }
        mesh.vertices = vertices;
        mesh.triangles = meshTriangles;

        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
    }

    public void AddLocal(GameObject obj)
    {
        localAssets.Add(obj);
        obj.SetActive(false);
    }

    float width = 2000;

    public void CheckLocal(Vector3 Pos, float dist, Camera cam)
    {
        foreach (var obj in localAssets)
        {
            if ((obj.transform.position - Pos).magnitude < dist)
            {
                Vector3 screenPos = cam.WorldToScreenPoint(obj.transform.position);
                if(screenPos.x>-width && screenPos.x < cam.pixelWidth + width && screenPos.y > -width && screenPos.y < cam.pixelHeight + width && screenPos.z > 0)
                {
                    obj.SetActive(true);
                    continue;
                }
            }
            obj.SetActive(false);
        }
    }

    public float GetWidth()
    {
        float side = numPointsPerAxis * MetersBetweenPoints;
        return Mathf.Sqrt(3f * side * side);
    }

    public float SideLength()
    {
        return numPointsPerAxis * MetersBetweenPoints;
    }

    public void Mine(Vector3 Pos, float radius, float strength)
    {
        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int numThreadsPerAxis = Mathf.CeilToInt(numVoxelsPerAxis / (float)threadGroupSize);

        
        mineShader.SetBuffer(0, "points", pointsBuffer);
        mineShader.SetFloat("MetersBetweenPoints", MetersBetweenPoints);
        mineShader.SetVector("offset", offset);
        mineShader.SetVector("center", PlanetCenter);
        mineShader.SetInt("numPointsPerAxis", numPointsPerAxis);
        mineShader.SetVector("mineLocation", (Pos - transform.position));
        mineShader.SetFloat("radius", radius);
        mineShader.SetFloat("mineStrength", strength);

        mineShader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);


        UpdateChunkMesh();
    }

    struct Triangle
    {
#pragma warning disable 649 // disable unassigned variable warning
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Vector3 this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return a;
                    case 1:
                        return b;
                    default:
                        return c;
                }
            }
        }
    }

    public void UpdateGizmo(bool g)
    {
        draw = g;
    }

    private void OnDrawGizmos()
    {
        if(draw)
            Gizmos.DrawWireCube(transform.position+ new Vector3(1, 1, 1)*(numPointsPerAxis-1) * MetersBetweenPoints/2, new Vector3(1, 1, 1) * (numPointsPerAxis - 1) * MetersBetweenPoints);
    }

}
