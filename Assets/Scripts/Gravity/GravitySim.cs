using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class GravitySim : MonoBehaviour
{
    class simpleBody
    {
        public float mass;
        public Vector3 position;
        public Vector3 velocity;
        public bool exerter;
        public bool stationary;
        public string name;
        GameObject lineRenderer = new GameObject();
        LineRenderer lr;
        List<Vector3> positions = new List<Vector3>();

        public simpleBody(GravityManager man, GravityObject obj, Material mat)
        {
            mass = obj.mass;
            position = obj.transform.position;
            velocity = obj.startVelocity;
            exerter = obj.exerter;
            lr = lineRenderer.AddComponent<LineRenderer>();
            lineRenderer.transform.parent = man.transform;
            lineRenderer.name = obj.name + " Sim";
            stationary = obj.Stationary;
            lr.startWidth = 20;
            lr.endWidth = 20;
            lr.material = mat;
            lr.startColor = Color.green;
            lr.endColor = Color.red;
            name = obj.name;
        }
        public void clean()
        {
            DestroyImmediate(lineRenderer);
        }

        public void sim(simpleBody refrence, Vector3 origin)
        {
            if (refrence != null)
            {
                positions.Add(position - refrence.position + origin);
            }
            else
            {
                positions.Add(position);
            }
        }

        public void Display()
        {
            lr.positionCount = positions.Count;
            lr.SetPositions(positions.ToArray());
        }
    }

    public bool suppress;
    public int Steps;
    public float timeStep;
    List<simpleBody> bodies = new List<simpleBody>();
    GravityManager manager;
    public GravityObject realRef;
    public Material lineMat;
    simpleBody simRef = null;

    // Update is called once per frame
    void Update()
    {
        if (suppress) return;
        simRef = null;
        LineRenderer[] toClean = FindObjectsOfType<LineRenderer>();
        foreach (var r in toClean)
        {
            DestroyImmediate(r.gameObject);
        }
        if (Application.isPlaying) return;
        bodies.Clear();
        manager = FindObjectOfType<GravityManager>();
        bodies = FindObjectsOfType<GravityObject>().ToList().Where(x => x.exerter).ToList().ConvertAll<simpleBody>(x => {
                    var tmp = new simpleBody(manager, x, lineMat);
                    if (x == realRef) simRef = tmp;
                    return tmp;
                });

        for (int i = 0; i < Steps; i++)
        {
            foreach (var body in bodies)
            {
                updateVelocity(body);
            }
            foreach (var body in bodies)
            {
                updatePosition(body);
            }
        }

        foreach (var body in bodies)
        {
            body.Display();
        }
    }

    void updateVelocity(simpleBody sim)
    {
        if (sim.stationary) return;
        Vector3 acceleration = new Vector3(0, 0, 0);
        foreach (var obj in bodies)
        {
            if (obj != sim && obj.exerter)
            {
                float sqrDist = (obj.position - sim.position).sqrMagnitude;
                Vector3 dir = (obj.position - sim.position).normalized;
                acceleration += dir * Mathf.Pow(GravityManager.GVal, GravityManager.GPow) * obj.mass / sqrDist;

                if (sim.name == "Moon")
                {
                    //print(obj.name + " : " + (dir * Mathf.Pow(manager.GVal, manager.GPow) * obj.mass / sqrDist).magnitude);
                }
            }
        }
        sim.velocity += acceleration * 10 * timeStep;
    }

    void updatePosition(simpleBody sim)
    {
        sim.position += sim.velocity * timeStep;
        sim.sim(simRef, realRef.transform.position);
    }
}
