using System.Collections.Generic;
using UnityEngine;

public class FighterStatsClass
{
    public int MaxHealth = 100;
    [Range(0, 1)]
    public float LastBreathThreshold = 0.1f;
    public int AttackDamage = 10;
    public float ChargeDamageBoost = 1;
    public int MaxAmountOfChargings = 3;

    public int currentHealth;

    public FighterState currentState;
    public Dictionary<string, float> lastingDamageBoosts;
    public float oneTimeDamageBoost;

    public FighterStatsClass()
    {
        currentHealth = MaxHealth;
        currentState = FighterState.alive;
        lastingDamageBoosts = new Dictionary<string, float>();
    }

    public int GetMaxHealth()
    {
        return MaxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public FighterState GetCurrentFighterState()
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

    public virtual int GetCurrentAttackDamage(bool attackAndReset = true)
    {
        float lastingBoosts = 0f;
        foreach (KeyValuePair<string, float> pair in lastingDamageBoosts)
        {
            lastingBoosts += pair.Value;
        }

        int damage = Mathf.FloorToInt(AttackDamage * (1 + lastingBoosts + oneTimeDamageBoost));
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
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                currentState = FighterState.dead;
            }
            else if (currentHealth <= MaxHealth * LastBreathThreshold)
            {
                currentState = FighterState.lastBreath;
            }
        }
        else
        {
            Debug.LogWarning("Fighter cannot receive negative damage. Health will not be modified.");
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
                currentState = FighterState.alive;
            }
        }
        else
        {
            Debug.LogWarning("Fighter cannot be healed by a negative amount. Health will not be modified.");
        }
    }

    public void AddLastingDamageBoost(string source, float mod)
    {
        if (lastingDamageBoosts.ContainsKey(source))
        {
            Debug.LogWarning("Fighter cannot receive multiple lasting damage boosts from the same source. Attacke damage will not be modified.");
        }
        lastingDamageBoosts.Add(source, mod);
    }

    public void RemoveLastingDamageBoost(string source)
    {
        if (!lastingDamageBoosts.ContainsKey(source))
        {
            Debug.LogWarning("Fighter cannot remove lasting damage boost of a source that never gave him a boost. Attacke damage will not be modified.");
        }
        lastingDamageBoosts.Remove(source);
    }

    public void UseChargeForDamageBoost()
    {
        if (oneTimeDamageBoost < MaxAmountOfChargings * ChargeDamageBoost)
        {
            oneTimeDamageBoost += ChargeDamageBoost;
        }
    }

}

public enum FighterState
{
    alive, dead, lastBreath
}
