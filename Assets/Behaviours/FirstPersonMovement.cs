using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    [Header("Paramters")]
    [SerializeField] float speed = 30;
    [SerializeField] float strafe_speed_modifier = 1;
    [SerializeField] float back_speed_modifier = 1;
    [SerializeField] bool allow_flying;

    [Header("References")]
    [SerializeField] Transform movement_relation;
    

    void Start()
    {

    }


    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * speed * strafe_speed_modifier;
        float vertical = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        if (vertical < 0)
            vertical *= back_speed_modifier;

        if (movement_relation == null)
        {
            transform.position += new Vector3(horizontal, 0, vertical);
        }
        else
        {
            Vector3 move = (horizontal * movement_relation.transform.right) +
                (vertical * movement_relation.transform.forward);

            transform.position += move;
        }
    }

}
