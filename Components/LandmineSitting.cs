using System.Collections;
using MyceliumNetworking;
using Photon.Pun;
using UnityEngine;

namespace Landmines.Components;

public class LandmineSitting : Sittable
{
	public ThisIsALandmine landmine;
	
	public new void UnSit()
	{
		base.UnSit();
		
		Debug.Log("Player got up from landmine");
		
		if(!PhotonNetwork.IsMasterClient)
			return;
		
		Debug.LogWarning("Exploding landmine because player got up");
		landmine.CallRPCExplode();
	}
}