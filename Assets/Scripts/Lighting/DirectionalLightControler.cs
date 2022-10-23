using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalLightControler : MonoBehaviour
{
    Camera main;
    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.forward = main.transform.position - transform.position;
    }
}
