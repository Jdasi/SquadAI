using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    void LateUpdate()
    {
        transform.LookAt(JHelper.main_camera.transform);
    }

}
