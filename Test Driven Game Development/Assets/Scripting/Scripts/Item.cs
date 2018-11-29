using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TTGD/Item")]
public class Item : ScriptableObject
{
    public ItemType type;
    public int MaxUses = 3;
    private int usesLeft;

    public Item()
    {
        SetUpItem();
    }

    public void SetUpItem()
    {
        usesLeft = MaxUses;
    }

    public void Use(FighterStatsClass user) 
    {
        if (usesLeft == 0)
        {
            Debug.LogWarning("Tried to use item when it had 0 uses left. No effect!");
            return;
        }
        if (user == null)
        {
            Debug.LogWarning("Tried to use item without a user. No effect!");
            return;
        }

        switch (type)
        {
            case ItemType.AttackBoost:
                break;
            case ItemType.Healing:
                user.GetHealedBy(20);
                break;
            default:
                Debug.LogWarning("There was no behavior specified for item of type " + type.ToString());
                break;
        }
        usesLeft--;
    }


    public int GetMaxUses()
    {
        return MaxUses;
    }

    public int GetUsesLeft()
    {
        return usesLeft;
    }
}

public enum ItemType
{
    Healing,
    AttackBoost
}
