using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    public PlayerStatsClass stats;
    public PlayerInventoryClass inventory;

	void Start ()
    {
        stats.SetUpStats(this);
        inventory.SetUpInventory(this);

        foreach(Item i in inventory.items)
        {
            i.SetUpItem();
        }
    }

    void Update ()
    {
		// TMP
        if(Input.GetKeyDown(KeyCode.Space))
        {
            EnemyStatsClass enemy = FindObjectOfType<Enemy>().stats;
            stats.AttackOpponent(enemy);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            stats.UseChargeForDamageBoost();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            stats.ReceiveDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            inventory.UseItem(0);
        }
    }

    #region Implementation IPlayer

    public int GetAllDamageBonus()
    {
        int bonus = 0;

        if (inventory != null)
        {
            Weapon wpn = inventory.GetEquippedWeapon();
            if (wpn != null)
            {
                bonus += wpn.damage;
            }
        }

        return bonus;
    }

    public PlayerStatsClass GetPlayerStats()
    {
        return stats;
    }

    public PlayerInventoryClass GetPlayerInventory()
    {
        return inventory;
    }

    #endregion Implementation IPlayer
}

public interface IPlayer
{
    int GetAllDamageBonus();
    PlayerStatsClass GetPlayerStats();
    PlayerInventoryClass GetPlayerInventory();
}

[Serializable]
public class PlayerStatsClass : FighterStatsClass
{
    private IPlayer playerAddition;

    private int currentPoints;

    #region Setup
    public PlayerStatsClass()
    {
        currentPoints = 0;
    }

    public override void SetUpStats(IPlayer player = null)
    {
        base.SetUpStats();
        SetPlayerAddition(player);
    }

    public void SetPlayerAddition(IPlayer addition)
    {
        this.playerAddition = addition;
    }
    #endregion Setup

    #region Points
    public int GetCurrentPoints()
    {
        return currentPoints;
    }

    public void ModifyPoints(int mod)
    {
        currentPoints += mod;
        if (currentPoints < 0)
        {
            currentPoints = 0;
        }
    }
    #endregion Points

    #region Attack
    public override int GetCurrentAttackDamage(bool attackAndReset = true)
    {
        int baseDamage = base.GetCurrentAttackDamage(attackAndReset);
        int bonusDamage = 0;

        if (playerAddition != null)
        {
            bonusDamage = playerAddition.GetAllDamageBonus();
        }

        return baseDamage + bonusDamage;
    }
    #endregion Attack

    #region Health
    public override void Die()
    {
        // End game? 
    }
    #endregion Health
}


[Serializable]
public class PlayerInventoryClass
{
    private IPlayer playerAddition;
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
            items[index].Use(playerAddition.GetPlayerStats());
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

    public void CollectItem(Item item)
    {
        items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }
    #endregion Items
}