using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteract
{
    public void Interact(GameObject Player);
}

public class Interact : MonoBehaviour
{
    public float range;
    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = Camera.main.transform;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            print("try int");
            if (Physics.Raycast(player.position, player.forward, out RaycastHit hitInfo, range))
            {
                print("hit");
                IInteract hit = hitInfo.collider.GetComponent<IInteract>();
                if(hit != null)
                {
                    hit.Interact(gameObject);
                }
            }
        }
    }
}
