using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{
    public PlayerStatsClass stats;
    public PlayerInventoryClass inventory;

	// Use this for initialization
	void Start ()
    {
        stats = new PlayerStatsClass();
        inventory = new PlayerInventoryClass();
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
}

[Serializable]
public class PlayerStatsClass : FighterStatsClass
{
    private int currentPoints;

    public PlayerStatsClass()
    {
        currentPoints = 0;
    }

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