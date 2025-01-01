using System.Collections;
using MyceliumNetworking;
using Photon.Pun;
using UnityEngine;

namespace Landmines.Components;

public class ThisIsALandmine : MonoBehaviour
{
	[SerializeField] public SFX_PlayOneShot beep;
	[SerializeField] public SFX_PlayOneShot press;
	[SerializeField] public SFX_PlayOneShot explosion;

	[SerializeField] public Light light;

	private int viewId = 0;
	
	private float beepTimer = 0f;
	private float timeSinceSpawn = 0;
	
	private List<Player> players = new List<Player>();
	
	private void Awake()
	{
		var pv = gameObject.GetComponent<PhotonView>();
		viewId = pv.ViewID;
		
		MyceliumNetwork.RegisterNetworkObject(this, LandminesPlugin.ModID, pv.ViewID);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!enabled || !PhotonNetwork.IsMasterClient)
			return;
		
		// Debug.LogError(other.gameObject.name);
		
		var player = other.gameObject.GetComponentInParent<Player>();
		if (player == null)
		{
			var iInstance = other.gameObject.GetComponentInChildren<ItemInstance>();
			if (iInstance == null || iInstance.isHeldByMe)
			{
				return; // if itemInstance and player is null, or is held, return
			}

			CallRPCExplode(); // explode if touched by item
			return;
		}
		
		if (timeSinceSpawn < 0.5f)
		{
			Debug.LogWarning("Just spawned in and collided with something, must be invalid spawn");
			Destroy(gameObject); // if spawned in and collided with something, destroy
			
			return;
		}

		if(players.Any()) return;
		
		players.Add(player);
		CallRPCSteppedOn();
	}

	private void CallRPCSteppedOn()
	{
		MyceliumNetwork.RPCMasked(LandminesPlugin.ModID, nameof(SteppedOn), ReliableType.Reliable, viewId);
	}
	
	private void CallRPCExplode()
	{
		MyceliumNetwork.RPCMasked(LandminesPlugin.ModID, nameof(Explode), ReliableType.Reliable, viewId);
	}
	
	[CustomRPC]
	public void SteppedOn()
	{
		press.Play();
	}
	
	[CustomRPC]
	public void Explode()
	{
		var explosionPrefab = LandminesPlugin.Bundle.GetAssetByName<GameObject>("Explosion")!;
		
		Instantiate(explosionPrefab, transform.position, Quaternion.identity, null);
		
		explosion.Play();
		Destroy(gameObject);
	}

	private void Update()
	{
		if(!enabled)
			return;

		timeSinceSpawn += Time.deltaTime;
		beepTimer += Time.deltaTime;
		if (beepTimer >= 15f)
		{
			beepTimer = 0f;
			StartCoroutine(BeepLight());
		}

		if (!players.Any()) return;
		
		Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f);
		var playerStillOnMine = false;
		
		foreach (var collider in colliders)
		{
			if (collider.GetComponentInParent<Player>() == null) continue;
			
			playerStillOnMine = true;
			break;
		}

		if (!playerStillOnMine)
			CallRPCExplode();
	}
	
	private IEnumerator BeepLight()
	{
		beep.Play();
		
		light.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.35f);
		light.gameObject.SetActive(false);
	}
	
	private void OnDestroy()
	{
		MyceliumNetwork.DeregisterNetworkObject(this, LandminesPlugin.ModID, viewId);
	}
}