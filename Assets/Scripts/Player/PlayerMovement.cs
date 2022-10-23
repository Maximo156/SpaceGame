using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Stick
{
    public float jumpStrength;
    public float moveSpeed;
    public float ShiftMultiplier;
    public float sensitivity;
    public float maxAngle = 45;
     Camera cam;
    bool running = false;

    Vector3 velocity;
    // Start is called before the first frame update
    protected override void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        base.Start();
        cam = Camera.main;
        angle = maxAngle;
    }

    // Update is called once per frame
    void Update()
    {
        move();
        turn();
        if (Input.GetKeyDown(KeyCode.T))
        {
            transform.GetChild(0).GetChild(0).gameObject.SetActive(!transform.GetChild(0).GetChild(0).gameObject.activeSelf);
        }
    }

    int counter = 0;
    private void FixedUpdate()
    {
        rb.position += velocity * Time.deltaTime;
        //print(Grounded());
    }

    float cooldown = 0.1f;
    float last;
    void move()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            running = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            running = false;
        }
        
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        moveDirection.y = 0;
        moveDirection = transform.TransformDirection(moveDirection);
        if (grounded && Time.time - last > cooldown)
        {
            if (Input.GetButtonDown("Jump"))
            {
                last = Time.time;
                Vector3 jmp = (transform.up * jumpStrength) + moveDirection * moveSpeed * (running ? ShiftMultiplier : 1);
                rb.velocity += jmp;
                velocity = new Vector3(0,0,0);
            }
            else
            {
                velocity = moveDirection * moveSpeed * (running ? ShiftMultiplier : 1);
            }
        }
        else if(moveDirection == Vector3.zero)
        {
            velocity = moveDirection;
        }
        
    }

    private bool Grounded()
    {
        bool hit = Physics.Raycast(transform.position, -transform.up, out RaycastHit info, 1.1f);
        return hit;// && Vector3.Angle(transform.up, info.normal) < angle;
    }


    void turn()
    {
        float turner = Input.GetAxis("Mouse X") * sensitivity;
        float looker = -Input.GetAxis("Mouse Y") * sensitivity;
        if (turner != 0)
        {
            transform.RotateAround(transform.position, transform.up, turner);
        }
        if (looker != 0)
        {
                cam.transform.localEulerAngles += new Vector3(looker, 0, 0);
        }
    }
}
