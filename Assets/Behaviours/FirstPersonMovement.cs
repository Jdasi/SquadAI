using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    [Header("Paramters")]
    [SerializeField] float speed = 30;
    [SerializeField] float strafe_speed_modifier = 1;
    [SerializeField] float back_speed_modifier = 1;
    [SerializeField] bool noclip;
    [SerializeField] LayerMask player_layer;
    [SerializeField] LayerMask noclip_layer;

    [Header("References")]
    [SerializeField] Rigidbody rigid_body;
    [SerializeField] Transform movement_relation;

    private float horizontal;
    private float vertical;


    public void ToggleNoclip()
    {
        noclip = !noclip;
        rigid_body.isKinematic = noclip;

        this.gameObject.layer = noclip ? noclip_layer : player_layer;
    }
    

    void Start()
    {

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            ToggleNoclip();

        horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * speed * strafe_speed_modifier;
        vertical = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        if (vertical < 0)
            vertical *= back_speed_modifier;
    }


    void FixedUpdate()
    {
        if (movement_relation == null)
        {
            transform.position += new Vector3(horizontal, 0, vertical);
        }
        else
        {
            Vector3 move = new Vector3();

            if (noclip)
            {
                move = (horizontal * movement_relation.transform.right) +
                       (vertical * movement_relation.transform.forward);
            }
            else
            {
                Vector3 forward = movement_relation.forward;

                forward.y = 0;
                forward.Normalize();

                move = (horizontal * movement_relation.transform.right) +
                    (vertical * forward);
            }

            if (move.magnitude > speed)
                move = move.normalized * speed;

            rigid_body.MovePosition(rigid_body.position + move);
        }
    }

}
