using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GravityManager : MonoBehaviour
{
    public static float GVal = 6.67f;
    public static float GPow =-11f;
    public float timeStep = 0.01f;
    private List<GravityObject> bigObjs = new List<GravityObject>();
    private List<GravityObject> objs;

    private void Start()
    {
        objs = new List<GravityObject>(FindObjectsOfType<GravityObject>());
        objs = objs.Where(x => x.isActiveAndEnabled).ToList();
        bigObjs = objs.Where(x => x!=null && x.exerter).ToList();


        float minMass = bigObjs[1].mass;
        foreach(var obj in bigObjs)
        {
            if (obj != null)
            {
                minMass = Mathf.Min(minMass, obj.mass);
            }
        }

        foreach (var obj in objs)
        {
            if (obj != null)
            {
                obj.scaleMass(minMass);
            }
        }
    }


    private void FixedUpdate()
    {
        foreach(var obj in objs)
        {
            if (obj != null)
                obj.UpdateVelocity(bigObjs, Mathf.Pow(GVal, GPow));
        }

        foreach (var obj in bigObjs)
        {
            if (obj != null)
                obj.UpdatePosition();
        }
    }
}
