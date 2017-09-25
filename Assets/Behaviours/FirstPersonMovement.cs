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
    [SerializeField] bool noclip;
    [SerializeField] string player_layer;
    [SerializeField] string noclip_layer;

    [Header("References")]
    [SerializeField] Rigidbody rigid_body;
    [SerializeField] Transform movement_relation;

    private float horizontal;
    private float vertical;
    private bool sprinting;

    private int player_layer_value;
    private int noclip_layer_value;


    public void ToggleNoclip()
    {
        noclip = !noclip;
        rigid_body.isKinematic = noclip;

        this.gameObject.layer = noclip ? noclip_layer_value : player_layer_value;
    }
    

    void Start()
    {
        player_layer_value = LayerMask.NameToLayer(player_layer);
        noclip_layer_value = LayerMask.NameToLayer(noclip_layer);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            ToggleNoclip();

        horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * CurrentSpeed() * strafe_speed_modifier;
        vertical = Input.GetAxis("Vertical") * Time.deltaTime * CurrentSpeed();

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

            if (horizontal != 0 && vertical != 0)
                move *= strafe_speed_modifier;

            rigid_body.MovePosition(rigid_body.position + move);
        }
    }


    float CurrentSpeed()
    {
        return noclip ? noclip_speed : speed;
    }

}
