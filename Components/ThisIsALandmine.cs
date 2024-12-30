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
	
	[SerializeField] public ItemInstance itemInstance;
	
	private float beepTimer = 0f;
#pragma warning disable CS0414
	private bool exploding = false;
#pragma warning restore CS0414

	private void Awake()
	{
		Destroy(GetComponentInParent<Pickup>());
	}

	private void Start()
	{
		MyceliumNetwork.RegisterNetworkObject(this, LandminePlugin.ModID, itemInstance.m_syncer.m_photonView.ViewID);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (exploding || !enabled)
			return;
		
		// Debug.LogError(other.gameObject.name);
		
		var player = other.gameObject.GetComponentInParent<Player>();
		if (player == null)
		{
			var iInstance = other.gameObject.GetComponentInChildren<ItemInstance>();
			if (iInstance == null || !iInstance.m_syncer.isMine || iInstance.isHeld) 
			{
				return; // if itemInstance and player is null, not mine, or is held, return
			};
		}
		if (player != null && !player.photonView.IsMine) return; // if player exists but is not mine, return
		
		exploding = true;
		MyceliumNetwork.RPCMasked(LandminePlugin.ModID, nameof(Explode), ReliableType.Reliable, itemInstance.m_syncer.m_photonView.ViewID);
	}

	private void Update()
	{
		if(!enabled)
			return;
		
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
		MyceliumNetwork.DeregisterNetworkObject(this, LandminePlugin.ModID);
	}

	[CustomRPC]
	public void Explode()
	{
		exploding = true;
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
}