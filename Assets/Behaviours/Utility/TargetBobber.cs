using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBobber : MonoBehaviour
{
    public bool active { get { return this.gameObject.activeSelf; } }

    [Header("Parameters")]
    public Vector3 offset;

    private Vector3 vec_target;
    private Transform trans_target;

    
    public void SetTarget(Vector3 _target)
    {
        vec_target = _target;
        trans_target = null;

        this.transform.position = _target + offset;
        this.gameObject.SetActive(true);
    }


    public void SetTarget(Transform _target)
    {
        vec_target = Vector3.zero;
        trans_target = _target;

        this.gameObject.SetActive(true);
    }


    public void Deactivate()
    {
        vec_target = Vector3.zero;
        trans_target = null;

        this.gameObject.SetActive(false);
    }

    
    void Update()
    {
        UpdatePosition();
    }


    void UpdatePosition()
    {
        transform.position = (trans_target != null ?
            trans_target.transform.position : vec_target) + offset;
    }

}
