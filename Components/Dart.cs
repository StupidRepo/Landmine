using Photon.Pun;
using UnityEngine;

namespace Landmine.Components;

public class Dart : ItemInstanceBehaviour
{
	public override void ConfigItem(ItemInstanceData data, PhotonView playerView)
	{
	}

	private void Update()
	{
		if (isHeldByMe && !Player.localPlayer.HasLockedInput() && Player.localPlayer.input.clickWasPressed)
		{
			Instantiate(LandminePlugin.Bundle.GetAssetByName<GameObject>("Explosion"), transform.position, transform.rotation);
		}
	}
}