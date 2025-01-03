using HarmonyLib;
using UnityEngine;

namespace Landmines.Patches;

[HarmonyPatch(typeof(AOE))]
public class AOEPatches
{
    [HarmonyPatch(nameof(AOE.DoAOE))]
    [HarmonyPrefix]
    public static bool DoAOEPrefix(ref AOE __instance)
    {
        var playerList = new List<Player>();
        var itemList = new List<ItemInstance>();
        foreach (var collider in Physics.OverlapSphere(__instance.transform.position, __instance.radius))
        {
            if (collider.attachedRigidbody == null) continue;
            
            var playerComponent = collider.GetComponentInParent<Player>();
            var itemComponent = collider.GetComponentInParent<ItemInstance>();
            var rb = collider.attachedRigidbody;
            if(playerComponent != null && playerComponent.refs.view.IsMine && !playerList.Contains(playerComponent))
                playerList.Add(playerComponent);
            else if(itemComponent != null && !itemComponent.isHeld && !itemList.Contains(itemComponent))
                itemList.Add(itemComponent);
            else
            {
                if (rb)
                {
                    Debug.LogWarning("No player or item found but a rigidbody was, adding force to rigidbody");
                    rb.AddForce((rb.transform.position - __instance.transform.position).normalized * __instance.force, ForceMode.Impulse);
                }
                continue;
            }
            
            var num = Mathf.InverseLerp(__instance.radius, __instance.innerRadius, Vector3.Distance(__instance.transform.position, collider.transform.position));
            if (playerComponent != null)
            {
                var force = (playerComponent.Center() - __instance.transform.position).normalized * num * __instance.force;
                playerComponent.CallTakeDamageAndAddForceAndFall(__instance.damage * num, force, __instance.fall * num);
            }
            else if (itemComponent != null)
                rb.AddForce((itemComponent.transform.position - __instance.transform.position).normalized * __instance.force, ForceMode.Impulse);
        }
        return false;
    }
}