using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour
{
    protected bool grounded { get { return contacts > 0; } }
    protected Rigidbody rb;
    int contacts;
    GravityObject grav;
    GravityObject otherGrav;
    Vector3 normalForce;
    protected float angle;
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        grav = GetComponent<GravityObject>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(-normalForce * rb.mass * 10, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GravityObject otherGrav = collision.gameObject.GetComponent<GravityObject>();

        contacts++;
    }
    // Start is called before the first frame update
    void OnCollisionStay(Collision collision) {
        GravityObject otherGrav = collision.gameObject.GetComponent<GravityObject>();
        if (otherGrav != null && otherGrav.exerter)
        {
            float sqrDist = (otherGrav.rb.position - rb.position).sqrMagnitude;
            Vector3 dir = (otherGrav.rb.position - rb.position).normalized;
            normalForce = dir * Mathf.Pow(GravityManager.GVal, GravityManager.GPow) * otherGrav.mass / sqrDist;
        }        
    }


    void OnCollisionExit(Collision collision)
    {
        contacts--;
    }
}
