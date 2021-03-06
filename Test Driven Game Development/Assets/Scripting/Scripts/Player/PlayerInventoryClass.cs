﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInventoryClass
{
    private IPlayer playerAddition;
    public int MaxItemSlots = 5;
    public Weapon equippedWeapon;
    public List<Item> items = new List<Item>();

    #region Setup
    public PlayerInventoryClass()
    {
        SetUpInventory();
    }

    public void SetUpInventory(IPlayer player = null)
    {
        SetPlayerAddition(player);
    }

    public void SetPlayerAddition(IPlayer addition)
    {
        this.playerAddition = addition;
    }
    #endregion Setup

    #region Weapon
    public Weapon GetEquippedWeapon()
    {
        return equippedWeapon;
    }
    public void EquipWeapon(Weapon weapon)
    {
        equippedWeapon = weapon;
    }

    #endregion Weapon

    #region Items
    public List<Item> GetCollectedItems()
    {
        CheckItemsForRemoval();
        return items;
    }

    public void UseItem(int index)
    {
        if (index < items.Count && items[index] != null)
        {
            items[index].Use(playerAddition.GetPlayerStats(), playerAddition.GetGameController());
            playerAddition.GetPlayerStats().currentTurnTime = 0;
        }

        CheckItemsForRemoval();
    }

    public void CheckItemsForRemoval()
    {
        items.RemoveAll(item => item.GetUsesLeft() == 0);
    }

    public bool PlayerHasItem(Item item)
    {
        return items.Contains(item);
    }

    public bool CanCollectItem(Item item)
    {
        return items.Count < MaxItemSlots || PlayerHasItem(item);
    }

    public void CollectItem(Item item)
    {
        if (CanCollectItem(item))
        {
            if (PlayerHasItem(item))
            {
                foreach (Item i in items)
                {
                    if (i == item)
                    {
                        i.AddUses(item.MaxUses);
                    }
                }
            }
            else
            {
                items.Add(item);
            }
            

            if (playerAddition != null
                && playerAddition.GetGameController() != null
                && playerAddition.GetGameController().GetInventoryUI() != null)
            {
                playerAddition.GetGameController().GetInventoryUI().UpdateInventoryUI();
            }
        }
        else
        {
            Debug.LogWarning("Inventory full, could not collect item!");
        }
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }
    #endregion Items
}
