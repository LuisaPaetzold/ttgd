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
        stats.SetUpPlayerStats(this);
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