using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    public PlayerStatsClass stats;
    public PlayerInventoryClass inventory;
    public IUnityStaticService staticService;

	void Start ()
    {
        stats.SetUpPlayerStats(this);
        inventory.SetUpInventory(this);
        if (staticService == null)  // only setup staticServe anew if it's not there already (a playmode test might have set a substitute object here that we don't want to replace)
        {
            staticService = new UnityStaticService();
        }

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




        float horizontal = staticService.GetInputAxisRaw("Horizontal");
        float vertical = (-1) * staticService.GetInputAxisRaw("Vertical");

        transform.position += stats.CalcMovement(horizontal, vertical, staticService.GetDeltaTime());
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