using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravityObject))]
[ExecuteInEditMode]
public class WieghtManagement : MonoBehaviour
{
    public bool UseSurfaceGravity;
    public float surfaceGravity = 1;
    float cachedGrav = 1;
    GravityObject managed;
    float radius;
    // Start is called before the first frame update
    void Start()
    {
        radius = GetComponent<PlanetLOD>().radius;
        managed = GetComponent<GravityObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!UseSurfaceGravity) return;
        else if(cachedGrav != surfaceGravity)
        {
            cachedGrav = surfaceGravity;
            managed.mass = radius * surfaceGravity / 6.67E-11f;
        }
    }
}
