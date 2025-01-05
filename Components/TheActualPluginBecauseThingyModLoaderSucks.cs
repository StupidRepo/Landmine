using System.Collections;
using MyceliumNetworking;
using UnityEngine;

namespace Landmines.Components;

public class TheActualPluginBecauseThingyModLoaderSucks : MonoBehaviour
{
	private void Awake()
	{
		var nMine = LandminesPlugin.Bundle.GetAssetByName<GameObject>("NormalMinePrefab");
		var iMine = LandminesPlugin.Bundle.GetAssetByName<GameObject>("ImpulseMinePrefab");
		
		if(nMine == null || iMine == null)
		{
			Debug.LogError("Failed to load mine prefabs");
			return;
		}
		
		MyceliumNetwork.RegisterNetworkObject(this, LandminesPlugin.ModID);
		
		MyceliumObjects.MyceliumObjects.RegisterGameObject(PhotonLandmineTypes.Normal, nMine);
		MyceliumObjects.MyceliumObjects.RegisterGameObject(PhotonLandmineTypes.Impulse, nMine);
	}
}