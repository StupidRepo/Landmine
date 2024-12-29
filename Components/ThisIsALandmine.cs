using Photon.Pun;
using UnityEngine;

namespace Landmine.Components;

public class ThisIsALandmine : MonoBehaviour
{
	[SerializeField] public SFX_PlayOneShot beep;
	[SerializeField] public SFX_PlayOneShot press;
	[SerializeField] public SFX_PlayOneShot explosion;
	
	private float beepTimer = 0f;
#pragma warning disable CS0414
	private bool exploding = false;
#pragma warning restore CS0414
	
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
		press.afterPlayAction = Explode;
			
		// Destroy(gameObject);
	}

	private void Update()
	{
		beepTimer += Time.deltaTime;
		if (beepTimer >= 1f)
			Beep();
	}
	
	public void Beep()
	{
		beepTimer = 0f;
		beep.Play();
	}
	
	public void Explode()
	{
		var explosionPrefab = LandminePlugin.Bundle.GetAssetByName<GameObject>("Explosion");
		var inst = Instantiate(explosionPrefab, transform.position, Quaternion.identity, transform);
		
		explosion.Play();
		Destroy(gameObject);
	}
}