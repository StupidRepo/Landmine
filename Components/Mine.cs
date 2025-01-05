using System.Collections;
using Landmines.Extensions;
using MyceliumNetworking;
using MyceliumObjects.Components;
using Photon.Pun;
using Steamworks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Landmines.Components;

// ReSharper disable Unity.PerformanceCriticalCodeNullComparison

public class Mine : MonoBehaviour
{
	[SerializeField] public SFX_PlayOneShot? beep;
	[SerializeField] public SFX_PlayOneShot? press;
	[SerializeField] public SFX_PlayOneShot? explosion;

	[SerializeField] public Light? light;

	private int networkId;
	
	private float beepTimer;
	private float timeSinceSpawn;
	
	private List<Player> playersOnTheMine = [];
	
	[NonSerialized] public bool IsExploding;
	
	[Header("Explosion Settings")]
	[FormerlySerializedAs("shouldExplodeOnTouch")] [SerializeField] public bool ShouldExplodeOnPlayerTouch = true;
	
	[SerializeField] public float force = 20f;
	[SerializeField] public float fall = 2f;
	
	[SerializeField] public float innerRadius = 4f;
	[SerializeField] public float outerRadius = 10f;
	
	[SerializeField] public float damage = 150f;
	
	private float local_SteppedOnTimestamp;
	private float local_SteppedOffTimestamp;
	
	private Player? PlayerWhoSteppedOnMeAndGotUsIntoThisMessAnywayInTheFirstPlace;
	
	private void Start()
	{
		var pv = gameObject.GetComponent<MyceliumView>();
		if (pv == null)
		{
			Debug.LogError("MyceliumView is null, destroying");
			
			Destroy(gameObject);
			return;
		}
		
		networkId = pv.NetworkId;
		MyceliumNetwork.RegisterNetworkObject(this, LandminesPlugin.ModID, networkId);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!enabled)
			return;
		
		// Debug.LogError(other.gameObject.name);
		
		var player = other.gameObject.GetComponentInParent<Player>();
		if (player == null)
		{
			var iInstance = other.gameObject.GetComponentInChildren<ItemInstance>();
			if (iInstance == null || iInstance.isHeldByMe) return; // if itemInstance and player is null, or is held, return

			if(!IsExploding) CallRPCExplode(); // explode if touched by item
			return;
		}
		
		if (timeSinceSpawn < 0.5f)
		{
			Debug.LogWarning("Just spawned in and collided with something, must be invalid spawn");
			Destroy(gameObject); // if spawned in and collided with something, destroy
			
			return;
		}
		
		if (!ShouldExplodeOnPlayerTouch)
		{
			if (playersOnTheMine.Contains(player)) return;
			playersOnTheMine.Add(player);

			if (player == Player.localPlayer)
			{
				Debug.LogWarning("I just stepped on mine: " + Time.time);
				local_SteppedOnTimestamp = Time.time;
			}
		}
		
		if(IsExploding) return;
		CallRPCSteppedOn(player.photonView.ViewID);
	}

	public virtual void CallRPCSteppedOn(int playerWhoSteppedOn)
	{
		GetComponent<Rigidbody>().isKinematic = true;
		if(!PhotonNetwork.IsMasterClient) return;
		
		MyceliumNetwork.RPCMasked(LandminesPlugin.ModID, nameof(SteppedOn), ReliableType.Reliable, networkId, playerWhoSteppedOn);
	}
	
	public virtual void CallRPCExplode()
	{
		IsExploding = true;
		
		if(!PhotonNetwork.IsMasterClient) return;
		MyceliumNetwork.RPCMasked(LandminesPlugin.ModID, nameof(Explode), ReliableType.Reliable, networkId);
	}
	
	[CustomRPC]
	public virtual void SteppedOn(int playerWhoSteppedOn)
	{
		GetComponent<Rigidbody>().isKinematic = true;

		var player = PhotonView.Find(playerWhoSteppedOn).GetComponent<Player>();
		if (player == null)
		{
			Debug.LogError("Player is null :P");
			return;
		};
		PlayerWhoSteppedOnMeAndGotUsIntoThisMessAnywayInTheFirstPlace = player;
		Debug.LogWarning("Player stepped on mine: " + player.photonView.Owner.NickName);
		
		if(press != null)
			press.Play();
		
		if(ShouldExplodeOnPlayerTouch) Explode(); // just call explode because steppedon is already called on all clients,
                                            // so we can just call explode as that'll be called on all clients
	}
	
	[CustomRPC]
	public virtual void Explode()
	{
		IsExploding = true;
		
		LandminesPlugin.Bundle.MakeExplosion(transform.position,
			force, fall, innerRadius, outerRadius, damage);
		
		if(explosion != null)
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

		if(ShouldExplodeOnPlayerTouch) return;
		if (!playersOnTheMine.Any()) return;
		
		// ReSharper disable once Unity.PreferNonAllocApi
		Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f);
		playersOnTheMine = colliders
			.Select(collider => collider.GetComponentInParent<Player>())
			.Where(collider => collider != null).ToList();

		var playerStillOnMine = playersOnTheMine.Any();
		if(playersOnTheMine.All(player => player != Player.localPlayer) && !IsExploding && local_SteppedOnTimestamp > 0f && PlayerWhoSteppedOnMeAndGotUsIntoThisMessAnywayInTheFirstPlace == Player.localPlayer)
		{
			local_SteppedOffTimestamp = Time.time;
			var timeOnMine = local_SteppedOffTimestamp - local_SteppedOnTimestamp;
			
			Debug.LogWarning("TIME ON MINE: " + timeOnMine);
			SteamTimeline.AddTimelineEvent(
				"steam_bolt",
				"Landmine",
				"This can't be good :o",
				1,
				(-timeOnMine)-LandminesPlugin.SecToRecordBeforeStep,
				LandminesPlugin.SecToRecordBeforeStep+timeOnMine+LandminesPlugin.SecToRecordAfterStep, // duration is:
				// [configged] seconds before stepping on the mine
				// + time on mine
				// [configged] seconds after stepping off the mine
				ETimelineEventClipPriority.k_ETimelineEventClipPriority_Standard
			);

			SteamTimeline.AddTimelineEvent(
				"steam_explosion",
				"Landmine Exploded",
				"There's no way bro is recovering from this",
				3,
				0,
				0,
				ETimelineEventClipPriority.k_ETimelineEventClipPriority_Featured
			);
			
			local_SteppedOnTimestamp = 0f;
			local_SteppedOffTimestamp = 0f;
		}

		if (!playerStillOnMine && !IsExploding)
			CallRPCExplode();
	}
	
	private IEnumerator BeepLight()
	{
		if(beep != null)
			beep.Play();

		if (light == null) yield break;
		
		light.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.35f);
		light.gameObject.SetActive(false);
	}
	
	private void OnDestroy()
	{
		MyceliumNetwork.DeregisterNetworkObject(this, LandminesPlugin.ModID, networkId);
	}
}