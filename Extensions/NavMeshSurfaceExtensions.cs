using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Landmines.Extensions;

public static class NavMeshSurfaceExtensions
{
    public static Vector3 GetRandomPosition(this NavMeshSurface navMeshSurface, float minY, float maxY)
    {
        var bounds = navMeshSurface.navMeshData.sourceBounds;

        var randomPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(minY, maxY),
            Random.Range(bounds.min.z, bounds.max.z)
        );

        return NavMesh.SamplePosition(randomPosition, out var finalHit, Mathf.Infinity, NavMesh.AllAreas) ? finalHit.position : Vector3.zero;
    }
}