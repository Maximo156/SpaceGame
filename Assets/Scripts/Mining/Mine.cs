using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public float reach = 3.5f;
    public float radius = 1;
    public float strength = 1;
    public bool drawGizmos = false;
    Transform player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(Physics.Raycast(player.position, player.forward, out RaycastHit hitInfo, reach, 1<<8))
            {
                MinePosition(hitInfo);
            }
        }
    }

    void MinePosition(RaycastHit hit)
    {
        hit.collider.transform.parent.GetComponent<MarchingMesh>().Mine(hit.point, radius, strength);
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            if(Physics.Raycast(player.position, player.forward, out RaycastHit hitInfo, reach))
            {
                Gizmos.DrawSphere(hitInfo.point, radius);
            }
        }
    }

}
