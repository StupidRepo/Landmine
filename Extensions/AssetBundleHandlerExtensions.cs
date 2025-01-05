using UnityEngine;
using Object = UnityEngine.Object;

namespace Landmines.Extensions;

public static class AssetBundleHandlerExtensions
{
	public static GameObject MakeExplosion(this AssetBundleHandler abHandler, Vector3 position, 
		float force, float fall, float innerRadius, float outerRadius, float damage)
	{
		var explosionPrefab = LandminesPlugin.Bundle.GetAssetByName<GameObject>("Explosion")!;
		var go = Object.Instantiate(explosionPrefab, position, Quaternion.identity, null);
		var aoe = go.GetComponent<AOE>();
		
		aoe.force = force;
		aoe.innerRadius = innerRadius;
		aoe.radius = outerRadius;
		aoe.damage = damage;
		aoe.fall = fall;
		
		return go;
	}
}