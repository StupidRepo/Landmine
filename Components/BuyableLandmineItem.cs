using Photon.Pun;
using UnityEngine;

namespace Landmine.Components;

public class BuyableLandmineItem : ItemInstanceBehaviour
{
	private OnOffEntry onOffEntry;
	
	public override void ConfigItem(ItemInstanceData data, PhotonView playerView)
	{
		if (data.TryGetEntry(out onOffEntry))
		{
			Debug.Log("Found OnOffEntry: We are " + onOffEntry.on);
		}
		else
		{
			onOffEntry = new OnOffEntry
			{
				on = false
			};
			data.AddDataEntry(onOffEntry);
			Debug.Log("OnOffEntry not found, adding new entry with false.");
		}
	}

	private void Update()
	{
		if (isHeldByMe && !onOffEntry.on
		               && !Player.localPlayer.HasLockedInput() && Player.localPlayer.input.clickWasPressed)
		{
			onOffEntry.on = true;
			onOffEntry.SetDirty();
			Debug.Log("Landmine armed!!");

			var inv = Player.localPlayer.GetComponent<PlayerInventory>();
			if (inv.TryGetSlot(Player.localPlayer.data.selectedItemSlot, out var slot))
				Player.localPlayer.refs.items.DropItem(slot.SlotID);
			
			gameObject.AddComponent<ThisIsALandmine>();
			enabled = false;
		}
	}
}