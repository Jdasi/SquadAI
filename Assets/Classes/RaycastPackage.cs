using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes a configuration for a Raycast call to use.
/// </summary>
public class RaycastPackage
{
    public Vector3 from;        // Start point of the ray.
    public Vector3 to;          // End point of the ray.
    public Vector3 direction;   // Direction the ray should travel.
    public float length;        // Maxmimum length of the ray.
}
