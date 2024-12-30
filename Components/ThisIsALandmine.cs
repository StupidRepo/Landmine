using System.Collections;
using MyceliumNetworking;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Landmine.Components;

public class ThisIsALandmine : MonoBehaviour
{
	[SerializeField] public SFX_PlayOneShot beep;
	[SerializeField] public SFX_PlayOneShot press;
	[SerializeField] public SFX_PlayOneShot explosion;

	[SerializeField] public Light light;

	public int viewId = 0;
	
	private float beepTimer = 0f;
#pragma warning disable CS0414
	private bool exploding = false;
#pragma warning restore CS0414

	private float timeSinceSpawn = 0;
	// private int viewId;

	private void Awake()
	{
		var pv = gameObject.GetComponent<PhotonView>();
		
		viewId = pv.ViewID;
		
		Debug.LogError("HELLOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOoOOOOOOOOOOOOOOOOOOOOOOOOO WE ARE PV PV PV PV PV: " + pv.ViewID);
		MyceliumNetwork.RegisterNetworkObject(this, LandminePlugin.ModID, pv.ViewID);
	}
	
	private void OnCollisionEnter(Collision other)
	{
		if (exploding || !enabled || !PhotonNetwork.IsMasterClient)
			return;
		
		Debug.LogError(other.gameObject.name);
		
		var player = other.gameObject.GetComponentInParent<Player>();
		if (player == null)
		{
			var iInstance = other.gameObject.GetComponentInChildren<ItemInstance>();
			if (iInstance == null || iInstance.isHeldByMe)
			{
				Debug.LogError("Not a player or item instance");
				return; // if itemInstance and player is null, not mine, or is held, return
			};
		}
		
		if (timeSinceSpawn < 0.5f)
		{
			Debug.LogError("Just spawned in and collided with something, must be invalid spawn");
			Destroy(gameObject); // if spawned in and collided with something, destroy
			
			return;
		}
		
		exploding = true;
		CallRPCExplode();
	}
	
	private void CallRPCExplode()
	{
		MyceliumNetwork.RPCMasked(LandminePlugin.ModID, nameof(Explode), ReliableType.Reliable, viewId);
	}
	
	[CustomRPC]
	public void Explode()
	{
		StartCoroutine(ExplodeCoroutine());
	}
	
	private IEnumerator ExplodeCoroutine()
	{
		press.Play();
		yield return new WaitForSeconds(0.2f);

		var explosionPrefab = LandminePlugin.Bundle.GetAssetByName<GameObject>("Explosion");
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
			Beep();
	}
	
	public void Beep()
	{
		beepTimer = 0f;
		beep.Play();
		StartCoroutine(BeepLight());
	}
	
	private IEnumerator BeepLight()
	{
		light.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.35f);
		light.gameObject.SetActive(false);
	}
	
	private void OnDestroy()
	{
		Debug.LogError("DE REG");
		MyceliumNetwork.DeregisterNetworkObject(this, LandminePlugin.ModID, viewId);
	}
}