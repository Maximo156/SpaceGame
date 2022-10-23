using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlanetAssets : MonoBehaviour
{
    public int numPoints;
    public bool redraw;
    public GameObject[] assets;
    public float viewdistance;
    GameObject[] storedAssets;
    Chunk[] chunks;
    Camera cam;
    float radius;
    posInfo[] points;
    // Start is called before the first frame update
    void Start()
    {
        chunks = gameObject.GetComponentsInChildren<Chunk>();
        cam = Camera.main;
        points = new posInfo[numPoints];
        radius = transform.GetComponent<PlanetLOD>().radius;
        storedAssets = new GameObject[numPoints];

        genPoints();
        SpawnObjects();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(var chunk in chunks)
        {
            if (chunk == null) continue;
            if ((chunk.transform.position - cam.transform.position).magnitude < viewdistance + chunk.GetWidth())
            {
                
                chunk.gameObject.SetActive(true);
                chunk.CheckLocal(cam.transform.position, viewdistance, cam);
            }
            else
            {
                chunk.gameObject.SetActive(false);
            }
        }
    }

    void SpawnObjects()
    {
        for(int i = 0; i<numPoints; i++)
        {
            int index = Random.Range(0, assets.Length);
            storedAssets[i] = Instantiate(assets[index], points[i].position, Quaternion.identity, points[i].parent);
            storedAssets[i].transform.up = points[i].position - transform.position;
            points[i].parent.GetComponent<Chunk>().AddLocal(storedAssets[i]);
        }
    }

    void genPoints()
    {
        points = new posInfo[numPoints];
        for(int i = 0; i< numPoints; i++)
        {
            points[i].position = new Vector3(Random.Range(-1f,1), Random.Range(-1f, 1), Random.Range(-1f, 1)).normalized * (radius+10) + transform.position;

            Ray ray = new Ray(points[i].position, transform.position - points[i].position);

            Physics.Raycast(ray, out RaycastHit info, radius, Physics.AllLayers);
            
            points[i].position = info.point;
            points[i].parent = info.collider.transform;
        }
    }

    struct posInfo
    {
        public Transform parent;
        public Vector3 position;
    }

    private void OnDrawGizmos()
    {
        /*
        foreach(var point in points)
        {
            
            Gizmos.DrawSphere(point, 2);
        }*/
    }
}
