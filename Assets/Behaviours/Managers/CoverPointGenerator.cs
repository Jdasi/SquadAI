﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoverPointGenerator : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] Vector3 extents;
    [Range(2, 50)][SerializeField] int segments = 25;
    [SerializeField] float nav_search_radius = 0.5f;

    [Header("References")]
    [SerializeField] int wall_layer;

    [Header("Debug")]
    [SerializeField] Color grid_color = Color.yellow;
    [SerializeField] Color cover_point_color = Color.cyan;
    [SerializeField] float cover_point_size = 0.5f;
    [SerializeField] bool update_on_select;

    private Vector3 half_extents;
    private List<RaycastPackage> ray_packs = new List<RaycastPackage>();
    private List<CoverPoint> cover_points = new List<CoverPoint>();


    public void GenerateCoverPoints()
    {
        // Prepare for new data.
        ClearCoverPoints();

        // Update important variables.
        wall_layer = LayerMask.NameToLayer("Wall");
        half_extents = extents / 2;

        // Generate cover points.
        for (int i = 0; i < segments; ++i)
        {
            // Forward.
            ProcessLine(new Vector3(((extents.x / segments) * i) - half_extents.x, 0, -half_extents.z),
                new Vector3(0, 0, extents.z), Vector3.forward, extents.z, wall_layer);

            // Backward.
            ProcessLine(new Vector3(((extents.x / segments) * i) - half_extents.x, 0, half_extents.z),
                new Vector3(0, 0, -extents.z), -Vector3.forward, extents.z, wall_layer);

            // Right.
            ProcessLine(new Vector3(-half_extents.x, 0, ((extents.z / segments) * i) - half_extents.z),
                new Vector3(extents.x, 0, 0), Vector3.right, extents.x, wall_layer);

            // Left.
            ProcessLine(new Vector3(half_extents.x, 0, ((extents.z / segments) * i) - half_extents.z),
                new Vector3(-extents.x, 0, 0), -Vector3.right, extents.x, wall_layer);
        }
    }


    public void ClearCoverPoints()
    {
        ray_packs.Clear();
        cover_points.Clear();
    }


    public List<CoverPoint> ClosestCoverPoints(Vector3 _position, float _distance)
    {
        List<CoverPoint> closest_cover_points = new List<CoverPoint>();

        foreach (CoverPoint cover_point in cover_points)
        {
            if (Vector3.Distance(_position, cover_point.position) <= _distance)
                closest_cover_points.Add(cover_point);
        }

        return closest_cover_points;
    }


    public List<CoverPoint> ClosestCoverPoints(Vector3 _position, Vector3 _required_normal, float _distance)
    {
        List<CoverPoint> closest_cover_points = new List<CoverPoint>();

        foreach (CoverPoint cover_point in cover_points)
        {
            if (cover_point.normal != _required_normal)
                continue;

            if (Vector3.Distance(_position, cover_point.position) <= _distance)
                closest_cover_points.Add(cover_point);
        }

        return closest_cover_points;
    }


    void ProcessLine(Vector3 _from_offset, Vector3 _to_offset, Vector3 _direction,
        float _length, int _layer)
    {
        RaycastPackage ray_pack = new RaycastPackage();

        ray_pack.from = transform.position + _from_offset;
        ray_pack.to = ray_pack.from + _to_offset;
        ray_pack.direction = _direction;
        ray_pack.length = _length;
        ray_pack.layer = _layer;

        EnumerateCoverPoints(ray_pack);

        ray_packs.Add(ray_pack);
    }


    void EnumerateCoverPoints(RaycastPackage _ray_pack)
    {
        var hits = Physics.RaycastAll(_ray_pack.from, _ray_pack.direction,
            _ray_pack.length, 1 << _ray_pack.layer);

        foreach (var hit in hits)
        {
            NavMeshHit nav_hit;

            if (NavMesh.SamplePosition(hit.point + hit.normal, out nav_hit,
                nav_search_radius, NavMesh.AllAreas))
            {
                CoverPoint cover_point = new CoverPoint();

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
        Gizmos.color = cover_point_color;
        foreach (CoverPoint cover_point in cover_points)
            Gizmos.DrawSphere(cover_point.position, cover_point_size);
    }

}