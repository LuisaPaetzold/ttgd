using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Test_Item {

    #region Uses
    [Test]
    public void Test_ItemCannotBeUsedMoreThanMaxUses()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        Item item = ScriptableObject.CreateInstance<Item>();
        Assert.AreEqual(3, item.GetUsesLeft(), "Item uses did not start at the expected value!");

        item.Use(stats);
        Assert.AreEqual(2, item.GetUsesLeft(), "Item uses did not decrease as the item was used!");
        item.Use(stats);
        Assert.AreEqual(1, item.GetUsesLeft(), "Item uses did not decrease as the item was used!");
        item.Use(stats);
        Assert.AreEqual(0, item.GetUsesLeft(), "Item uses did not decrease as the item was used!");

        item.Use(stats);

        Assert.AreNotEqual(-1, item.GetUsesLeft(), "Item could be used even though it had 0 uses left!");
        LogAssert.Expect(LogType.Warning, "Tried to use item when it had 0 uses left. No effect!");
    }

    [Test]
    public void Test_ItemNeedsUserToBeUsed()
    {
        Item item = ScriptableObject.CreateInstance<Item>();
        item.Use(null);

        Assert.AreEqual(3, item.GetUsesLeft(), "Item uses were decreased even though no user was set!");
        LogAssert.Expect(LogType.Warning, "Tried to use item without a user. No effect!");
    }

    #endregion Uses

    #region Effects

    [Test]
    public void Test_ItemCanHealPlayer()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        stats.ReceiveDamage(50);
        int damagedHealth = stats.GetCurrentHealth();

        Item item = ScriptableObject.CreateInstance<Item>();
        item.type = ItemType.Healing;
        item.Use(stats);

        int healedHealth = stats.GetCurrentHealth();

        Assert.Less(damagedHealth, healedHealth, "Using a healing item did not increade the players health!");
    }

    #endregion Effects
}
