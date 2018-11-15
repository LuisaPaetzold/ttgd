using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour 
{

    PlayerStatsClass stats;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}


public class PlayerStatsClass
{
    public int MaxHealth = 100;
    [Range(0,1)]
    public float LastBreathThreshold = 0.1f;
    public int AttackDamage = 10;
    public float ChargeDamageBoost = 1;
    public int MaxAmountOfChargings = 3;

    private int currentHealth;
    private int currentPoints;
    private PlayerState currentState;
    private float lastingDamageBoost;
    private float oneTimeDamageBoost;

    public PlayerStatsClass()
    {
        currentHealth = MaxHealth;
        currentPoints = 0;
        currentState = PlayerState.alive;
    }

    public int GetMaxHealth()
    {
        return MaxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetCurrentPoints()
    {
        return currentPoints;
    }

    public PlayerState GetCurrentPlayerState()
    {
        return currentState;
    }

    public float GetLastBreathThreshold()
    {
        return LastBreathThreshold;
    }

    public float GetDefaultAttackDamage()
    {
        return AttackDamage;
    }

    public int GetCurrentAttackDamage(bool attackAndReset = true)
    {
        int damage = Mathf.FloorToInt(AttackDamage * (1 + lastingDamageBoost + oneTimeDamageBoost));
        if (attackAndReset)
        {
            oneTimeDamageBoost = 0;
        }
        return damage;
    }

    public float GetChargeDamageBoost()
    {
        return ChargeDamageBoost;
    }

    public int GetMaxAmountOfChargings()
    {
        return MaxAmountOfChargings;
    }




    public void ReceiveDamage(int damage)
    {
        if (damage > 0)
        {
            currentHealth -= damage;
            if (currentHealth < 0)
            {
                currentHealth = 0;
                currentState = PlayerState.dead;
            }
            else if(currentHealth <= MaxHealth * LastBreathThreshold)
            {
                currentState = PlayerState.lastBreath;
            }
        }
        else
        {
            Debug.LogWarning("Player cannot receive negative damage. Health will not be modified.");
        }
    }

    public void GetHealedBy(int heal)
    {
        if (heal > 0)
        {
            currentHealth += heal;
            if (currentHealth > MaxHealth)
            {
                currentHealth = MaxHealth;
            }
            else if (currentHealth > MaxHealth * LastBreathThreshold)
            {
                currentState = PlayerState.alive;
            }
        }
        else
        {
            Debug.LogWarning("Player cannot be healed by a negative amount. Health will not be modified.");
        }
    }

    public void ModifyPoints(int mod)
    {
        currentPoints += mod;
        if (currentPoints < 0)
        {
            currentPoints = 0;
        }
    }

    public void ReceiveLastingDamageBoost(float mod)
    {
        // to rework in liste
        lastingDamageBoost = mod;
    }

    public void RemoveLastingDamageBoost()
    {
        // to rework in liste
        lastingDamageBoost = 0;
    }

    public void UseChargeForDamageBoost()
    {
        if (oneTimeDamageBoost < MaxAmountOfChargings * ChargeDamageBoost)
        {
            oneTimeDamageBoost += ChargeDamageBoost;
        }
    }

}

public enum PlayerState
{
    alive, dead, lastBreath
}
