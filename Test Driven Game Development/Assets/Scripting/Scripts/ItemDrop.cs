﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemDrop : MonoBehaviour
{
    public Item droppedItem;
    public float maxDisplayDistance = 2.5f;

    private Canvas itemDisplay;
    private string normalText;
    private TextMeshProUGUI text;
    public Player player;
    public DoorControl door;

    void Start ()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
        
        itemDisplay = GetComponentInChildren<Canvas>();
        if (itemDisplay != null)
        {
            text = itemDisplay.GetComponentInChildren<TextMeshProUGUI>();
        }
        
        if (text != null)
        {
            normalText = text.text;
        }

        if (droppedItem != null)
        {
            droppedItem.SetUpItem();
        }
    }

	void Update ()
    {
		if (itemDisplay != null && player != null)
        {
            float dist = player.GetDistanceToPlayer(this.transform.position);
            if (dist < maxDisplayDistance)
            {
                itemDisplay.gameObject.SetActive(true);
            }
            else
            {
                itemDisplay.gameObject.SetActive(false);
            }

            if (text != null)
            {
                if (player.inventory != null
                    && player.inventory.items != null
                    && droppedItem != null
                    && !player.inventory.PlayerHasItem(droppedItem)
                    && !player.inventory.CanCollectItem(droppedItem))
                {
                    text.text = "Inventory full!";
                }
                else
                {
                    text.text = normalText;
                }
            }
        }
	}

    public void PlayerCollectItem()
    {
        if (droppedItem != null)
        {
            if (droppedItem.GetUsesLeft() == 0)
            {
                droppedItem.SetUpItem();
            }
            if (player.inventory != null)
            {
                player.inventory.CollectItem(droppedItem);
            }
            
            if (player.GameCtr != null)
            {
                SoundEffectControl sfx = player.GameCtr.GetSFXControl();
                if (sfx != null)
                {
                    sfx.ItemPickUp();
                }
            }
        }
        else
        {
            if (door != null)
            {
                // this is a key, react to collecting it 
                door.Open();

                if (player.GameCtr != null)
                {
                    SoundEffectControl sfx = player.GameCtr.GetSFXControl();
                    if (sfx != null)
                    {
                        sfx.KeyPickUp();
                    }
                }
            }
        }
        
        GameObject.Destroy(this.gameObject);
    }
}

