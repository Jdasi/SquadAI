using System.Collections;
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
        Clear();

        // Update important variables.
        wall_layer = LayerMask.NameToLayer("Wall");
        half_extents = extents / 2;

        // Generate cover points.
        for (int i = 0; i < segments; ++i)
        {
            ray_packs.Add(ForwardLines(i));
            ray_packs.Add(BackwardLines(i));
            ray_packs.Add(LeftLines(i));
            ray_packs.Add(RightLines(i));
        }
    }


    public void Clear()
    {
        ray_packs.Clear();
        cover_points.Clear();
    }


    RaycastPackage ForwardLines(int _i)
    {
        RaycastPackage ray_pack = new RaycastPackage();

        float x_offset = ((extents.x / segments) * _i) - half_extents.x;
        float z_offset = half_extents.z;

        ray_pack.from = transform.position - new Vector3(x_offset, 0, z_offset);
        ray_pack.to = ray_pack.from + new Vector3(0, 0, extents.z);
        ray_pack.direction = Vector3.forward;
        ray_pack.length = extents.z;
        ray_pack.layer = wall_layer;

        EnumerateCoverPoints(ray_pack);

        return ray_pack;
    }


    RaycastPackage BackwardLines(int _i)
    {
        RaycastPackage ray_pack = new RaycastPackage();

        float x_offset = ((extents.x / segments) * _i) - half_extents.x;
        float z_offset = -half_extents.z;

        ray_pack.from = transform.position - new Vector3(x_offset, 0, z_offset);
        ray_pack.to = ray_pack.from - new Vector3(0, 0, extents.z);
        ray_pack.direction = -Vector3.forward;
        ray_pack.length = extents.z;
        ray_pack.layer = wall_layer;

        EnumerateCoverPoints(ray_pack);

        return ray_pack;
    }


    RaycastPackage LeftLines(int _i)
    {
        RaycastPackage ray_pack = new RaycastPackage();

        float x_offset = -half_extents.x;
        float z_offset = ((extents.z / segments) * _i) - half_extents.z;

        ray_pack.from = transform.position + new Vector3(x_offset, 0, z_offset);
        ray_pack.to = ray_pack.from + new Vector3(extents.x, 0, 0);
        ray_pack.direction = Vector3.right;
        ray_pack.length = extents.x;
        ray_pack.layer = wall_layer;

        EnumerateCoverPoints(ray_pack);

        return ray_pack;
    }


    RaycastPackage RightLines(int _i)
    {
        RaycastPackage ray_pack = new RaycastPackage();

        float x_offset = half_extents.x;
        float z_offset = ((extents.z / segments) * _i) - half_extents.z;

        ray_pack.from = transform.position + new Vector3(x_offset, 0, z_offset);
        ray_pack.to = ray_pack.from - new Vector3(extents.x, 0, 0);
        ray_pack.direction = -Vector3.right;
        ray_pack.length = extents.x;
        ray_pack.layer = wall_layer;

        EnumerateCoverPoints(ray_pack);

        return ray_pack;
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


    void Update()
    {
        
    }


    void OnDrawGizmosSelected()
    {
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
