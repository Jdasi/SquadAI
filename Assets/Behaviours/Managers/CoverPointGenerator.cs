﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoverPointGenerator : MonoBehaviour
{
    public List<CoverPoint> cover_points = new List<CoverPoint>();

    [Header("Parameters")]
    [SerializeField] Vector3 extents;
    [Range(2, 50)][SerializeField] int segments = 25;
    [SerializeField] float nav_search_radius = 0.5f;
    [SerializeField] LayerMask hit_layers;
    [SerializeField] CoverPointSettings cover_point_settings;

    [Header("Debug")]
    [SerializeField] Color grid_color = Color.yellow;
    [SerializeField] float cover_point_size = 0.5f;
    [SerializeField] bool update_on_select;

    private Vector3 half_extents;
    private List<RaycastPackage> ray_packs = new List<RaycastPackage>();


    // Public only so the Editor script can access it.
    public void GenerateCoverPoints()
    {
        // Prepare for new data.
        ClearCoverPoints();

        // Update important variables.
        half_extents = extents / 2;

        // Generate cover points.
        for (int i = 0; i < segments; ++i)
        {
            // Forward.
            ProcessLine(new Vector3(((extents.x / segments) * i) - half_extents.x, 0, -half_extents.z),
                new Vector3(0, 0, extents.z));

            // Backward.
            ProcessLine(new Vector3(((extents.x / segments) * i) - half_extents.x, 0, half_extents.z),
                new Vector3(0, 0, -extents.z));

            // Right.
            ProcessLine(new Vector3(-half_extents.x, 0, ((extents.z / segments) * i) - half_extents.z),
                new Vector3(extents.x, 0, 0));

            // Left.
            ProcessLine(new Vector3(half_extents.x, 0, ((extents.z / segments) * i) - half_extents.z),
                new Vector3(-extents.x, 0, 0));
        }
    }


    // Public only so the Editor script can access it.
    public void ClearCoverPoints()
    {
        ray_packs.Clear();
        cover_points.Clear();
    }


    void ProcessLine(Vector3 _from_offset, Vector3 _to_offset)
    {
        RaycastPackage ray_pack = new RaycastPackage();

        ray_pack.from = transform.position + _from_offset;
        ray_pack.to = ray_pack.from + _to_offset;
        ray_pack.direction = (ray_pack.to - ray_pack.from).normalized;
        ray_pack.length = Vector3.Distance(ray_pack.from, ray_pack.to);

        EnumerateCoverPoints(ray_pack);

        ray_packs.Add(ray_pack);
    }


    void EnumerateCoverPoints(RaycastPackage _ray_pack)
    {
        var hits = Physics.RaycastAll(_ray_pack.from, _ray_pack.direction,
            _ray_pack.length, hit_layers);

        foreach (var hit in hits)
        {
            NavMeshHit nav_hit;

            if (NavMesh.SamplePosition(hit.point + hit.normal, out nav_hit,
                nav_search_radius, NavMesh.AllAreas))
            {
                CoverPoint cover_point = new CoverPoint(cover_point_settings);

                cover_point.position = hit.point + hit.normal;
                cover_point.normal = hit.normal;

                cover_points.Add(cover_point);
            }
        }
    }


    void Awake()
    {
        GenerateCoverPoints();
    }


    void Update()
    {
        foreach (CoverPoint cover_point in cover_points)
            cover_point.DetermineOccupiedStatus();
    }


    void OnDrawGizmosSelected()
    {
        // Debug update.
        if (update_on_select)
            GenerateCoverPoints();

        // Draw volume and grid.
        Gizmos.color = grid_color;
        Gizmos.DrawWireCube(transform.position, extents);

        foreach (RaycastPackage ray_pack in ray_packs)
            Gizmos.DrawLine(ray_pack.from, ray_pack.to);

        // Draw cover points.
        foreach (CoverPoint cover_point in cover_points)
        {
            Gizmos.color = cover_point.occupied ? cover_point_settings.occupied_color :
                cover_point_settings.unoccupied_color;

            Gizmos.DrawSphere(cover_point.position, cover_point_size);
        }
    }

}
