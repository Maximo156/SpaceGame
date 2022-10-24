using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControll : MonoBehaviour, IInteract
{
    public Vector3 camOffset = new Vector3(0, 0, 1.5f);
    public float sensitivity;
    public float acceleration;
    public float rotationRate;
    GameObject player;
    Rigidbody playerRB;
    bool control = false;
    public void Interact(GameObject Player)
    {
        Player.transform.parent = transform;
        Player.GetComponent<PlayerMovement>().enabled = false;
        player = Player;
        playerRB = player.GetComponent<Rigidbody>();
        playerRB.detectCollisions = false;
        playerRB.isKinematic = true;
        player.transform.position = transform.position + camOffset;
        player.transform.forward = transform.forward;
        player.transform.up = transform.up;

        Player.GetComponent<PlayerMovement>().cam.transform.forward = transform.forward;
        Player.GetComponent<PlayerMovement>().cam.transform.up = transform.up;
        control = true;
    }

    private void Exit()
    {
        player.transform.parent = null;
        player.GetComponent<PlayerMovement>().enabled = true;
        player.GetComponent<GravityObject>().OverrideVelocity(selfRB.velocity);
        playerRB.detectCollisions = true;
        playerRB.isKinematic = false;
        control = false;
    }
    Rigidbody selfRB;
    // Start is called before the first frame update
    void Start()
    {
        selfRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (control)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Exit();
            }
            move();
            turn();
        }
    }

    void move()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetButton("Jump")? 1: Input.GetKey(KeyCode.LeftControl)? -1 : 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        selfRB.AddForce(moveDirection * acceleration, ForceMode.Acceleration);

    }

    void turn()
    {
        float turner = Input.GetAxis("Mouse X") * sensitivity;
        float looker = -Input.GetAxis("Mouse Y") * sensitivity;
        if (turner != 0)
        {
            transform.RotateAround(transform.position, transform.up, Mathf.Sign(turner)* rotationRate * Time.deltaTime);
        }
        if (looker != 0)
        {
            transform.RotateAround(transform.position, transform.right, Mathf.Sign(looker) * rotationRate * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.RotateAround(transform.position, transform.forward, rotationRate*Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.RotateAround(transform.position, transform.forward, -rotationRate * Time.deltaTime);
        }
    }
}
