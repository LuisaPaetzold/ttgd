using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    PlayerStatsClass stats;
    PlayerInventoryClass inventory;

	// Use this for initialization
	void Start ()
    {
        stats.SetPlayerAddition(this);
    }
	
	// Update is called once per frame
	void Update ()
    {
		// TMP
        if(Input.GetKeyDown(KeyCode.Space))
        {
            int damage = stats.GetCurrentAttackDamage();
            FindObjectOfType<Enemy>().stats.ReceiveDamage(damage);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            stats.UseChargeForDamageBoost();
        }
    }

    /* Implementation IPlayer */

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

    /* End Implementation IPlayer */
}

public interface IPlayer
{
    int GetAllDamageBonus();
}

[Serializable]
public class PlayerStatsClass : FighterStatsClass
{
    private IPlayer playerAddition;

    private int currentPoints;

    public PlayerStatsClass()
    {
        currentPoints = 0;
    }

    public int GetCurrentPoints()
    {
        return currentPoints;
    }

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

    public void ModifyPoints(int mod)
    {
        currentPoints += mod;
        if (currentPoints < 0)
        {
            currentPoints = 0;
        }
    }

    public void SetPlayerAddition(IPlayer addition)
    {
        this.playerAddition = addition;
    }
}


[Serializable]
public class PlayerInventoryClass
{

    public Weapon equippedWeapon;
    public List<Item> items = new List<Item>();

    public Weapon GetEquippedWeapon()
    {
        return equippedWeapon;
    }

    public List<Item> GetCollectedItems()
    {
        return items;
    }

    public bool PlayerHasItem(Item item)
    {
        return items.Contains(item);
    }

    public void EquipWeapon(Weapon weapon)
    {
        equippedWeapon = weapon;
    }

    public void CollectItem(Item item)
    {
        items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }
}