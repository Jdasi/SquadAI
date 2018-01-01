using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    [Header("Paramters")]
    [SerializeField] float speed = 10;
    [SerializeField] float noclip_speed = 15;
    [SerializeField] float strafe_speed_modifier = 1;
    [SerializeField] float back_speed_modifier = 1;
    [SerializeField] float sprint_speed_modifier = 1.5f;
    [SerializeField] float crouch_speed_modifier = 0.5f;
    [SerializeField] Vector3 crouch_scale;
    [SerializeField] float jump_force;

    [Header("References")]
    [SerializeField] Rigidbody rigid_body;
    [SerializeField] Transform perspective_transform;
    [SerializeField] Transform torso_transform;
    [SerializeField] Transform legs_transform;

    private float horizontal;
    private float vertical;
    private bool noclip;
    private bool crouched;
    private bool sprinting;
    private bool grounded;

    private Vector3 original_leg_scale;


    public void ToggleNoclip()
    {
        noclip = !noclip;
        rigid_body.isKinematic = noclip;
    }
    

    void Start()
    {
        original_leg_scale =  legs_transform.localScale;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            ToggleNoclip();

        horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * CurrentSpeed() * strafe_speed_modifier;
        vertical = Input.GetAxis("Vertical") * Time.deltaTime * CurrentSpeed();

        crouched = Input.GetButton("Crouch");
        sprinting = !crouched && Input.GetButton("Sprint");

        grounded = Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0),
            Vector3.down, 0.2f, ~0);

        legs_transform.localScale = crouched ? crouch_scale : original_leg_scale;

        if (grounded && Input.GetButtonDown("Jump"))
        {
            grounded = false;
            rigid_body.AddForce(Vector3.up * jump_force * 1000);
        }

        if (vertical < 0)
            vertical *= back_speed_modifier;
    }


    void FixedUpdate()
    {
        if (perspective_transform == null)
        {
            transform.position += new Vector3(horizontal, 0, vertical);
        }
        else
        {
            Vector3 move = new Vector3();

            if (noclip)
            {
                move = (horizontal * perspective_transform.transform.right) +
                       (vertical * perspective_transform.forward);
            }
            else
            {
                move = (horizontal * perspective_transform.transform.right) +
                    (vertical * legs_transform.forward);
            }

            if (horizontal != 0 && vertical != 0)
                move *= strafe_speed_modifier;

            rigid_body.MovePosition(rigid_body.position + move);
        }
    }


    float CurrentSpeed()
    {
        return (noclip ? noclip_speed : speed) * (1 +
            (sprinting && !crouched ? sprint_speed_modifier : 0)) *
            (crouched ? crouch_speed_modifier : 1);
    }

}
