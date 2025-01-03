using Photon.Pun;
using UnityEngine;

namespace Landmines.Components;

public class LandmineSitting : Sittable
{
	public Mine landmine;

	public new void PlayerSit(Player player)
	{
		base.PlayerSit(player);
		if(!PhotonNetwork.IsMasterClient)
			return;
		
		Debug.LogWarning("Exploding landmine because player sat lmao");
		landmine.CallRPCExplode();
	}
}