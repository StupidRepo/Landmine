using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Landmine.Components;

public class ThisIsALandmine : MonoBehaviour
{
	[SerializeField] public SFX_PlayOneShot beep;
	[SerializeField] public SFX_PlayOneShot press;
	[SerializeField] public SFX_PlayOneShot explosion;

	[SerializeField] public Light light;
	
	private float beepTimer = 0f;
#pragma warning disable CS0414
	private bool exploding = false;
#pragma warning restore CS0414

	private void Awake()
	{
		Destroy(GetComponentInParent<Pickup>());
	}

	private void OnTriggerEnter(Collider other)
	{
		if (exploding)
			return;
		
		Debug.LogError(other.name);
		
		var player = other.GetComponentInParent<Player>();
		if (player == null) return;
		if (!player.photonView.IsMine) return;
		
		exploding = true;
		
		press.Play();
		Explode();
			
		Destroy(gameObject);
	}

	private void Update()
	{
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
		// flash light
		light.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.35f);
		light.gameObject.SetActive(false);
	}
	
	public void Explode()
	{
		var explosionPrefab = LandminePlugin.Bundle.GetAssetByName<GameObject>("Explosion");
		Instantiate(explosionPrefab, null);
		
		explosion.Play();
	}
}