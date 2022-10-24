using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityObject : MonoBehaviour
{
    public bool exerter = false;

    
    
    public bool Stationary = false;
    public bool Orient = false;
    public float radius { get; private set; }
    public float mass;
    public Vector3 startVelocity;
    public Vector3 velocity { get; private set;  }
    public Rigidbody rb { get; private set; }

    private void OnEnable()
    {
        radius = transform.localScale.x/2;
        var tmp = GetComponent<PlanetLOD>();
        if (tmp != null)
        {
            radius = tmp.radius;
        }
        velocity = startVelocity;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        if (!exerter)
        {
            rb.isKinematic = false;
            rb.velocity = startVelocity;
        }
        else
        {
            rb.isKinematic = true;
            velocity = startVelocity;
        }
    }

    public void OverrideVelocity(Vector3 vel)
    {
        if (!exerter)
        {
            rb.velocity = vel;
        }
    }

    public void scaleMass(float min)
    {
        rb.mass = mass / min;
    }

    public void UpdateVelocity(List<GravityObject> objs, float G, float timestep = -1)
    {
        if (timestep == -1) timestep = Time.deltaTime;
        if (Stationary) return;

        Vector3 acceleration = new Vector3(0,0,0);
        foreach(GravityObject obj in objs)
        {
            if (obj != this && obj != null && obj.rb.position != null && rb != null)
            {
                float sqrDist = (obj.rb.position - 
                    rb.position).sqrMagnitude;
                Vector3 dir = (obj.rb.position - rb.position).normalized;

                
                if (Orient && Mathf.Pow(sqrDist, 0.5f) - obj.radius < 100 && !rb.isKinematic)
                {
                    transform.rotation = Quaternion.LookRotation(-dir, -transform.forward);
                    transform.Rotate(Vector3.right, 90f);
                }
                if (Orient)
                {
                    //print(obj.name +" "+ dir * G * obj.mass / sqrDist);
                }

                acceleration += dir * G * obj.mass / sqrDist;
            }
        }

        if (!exerter)
        {
            rb.AddForce(acceleration*rb.mass * 10, ForceMode.Force);
        }
        else
        {
            velocity += acceleration * 10 * timestep;
        }
    }

    public void UpdatePosition(float timestep = -1)
    {
        if (timestep == -1) timestep = Time.deltaTime;
        rb.MovePosition(rb.position + velocity * timestep);
    }

}
