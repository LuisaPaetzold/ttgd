using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TTGD/Item")]
public class Item : ScriptableObject
{
    public ItemType type;
    public Sprite Icon;
    public int MaxUses = 3;
    private int usesLeft;

    #region Setup
    public Item()
    {
        SetUpItem();
    }

    public void SetUpItem()
    {
        usesLeft = MaxUses;
    }
    #endregion Setup

    public Sprite GetIcon()
    {
        return Icon;
    }

    #region Uses
    public void Use(FighterStatsClass user, IGameController gameCtr = null) 
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
                user.GetHealedBy(40);
                break;
            case ItemType.DealDamage:
                if (gameCtr != null)
                {
                    gameCtr.PlayerThrowBomb();
                }
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

    public void AddUses(int u)
    {
        usesLeft += u;
    }

    #endregion Uses
}

public enum ItemType
{
    Healing,
    AttackBoost,
    DealDamage
}
