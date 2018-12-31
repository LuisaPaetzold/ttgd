using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NSubstitute;

public class Test_Item
{

    [SetUp]
    public void Setup()
    {
        Debug.ClearDeveloperConsole();
    }


    #region Uses

    [Test]
    public void Test_ItemStartsWithMaxUses()
    {
        Item item = ScriptableObject.CreateInstance<Item>();
        Assert.AreEqual(item.GetMaxUses(), item.GetUsesLeft(), "Item uses did not start at the expected value!");
    }

    [Test]
    public void Test_ItemCanBeUsed()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        Item item = ScriptableObject.CreateInstance<Item>();

        item.Use(stats);
        Assert.AreEqual(item.GetMaxUses() - 1, item.GetUsesLeft(), "Item uses did not decrease as the item was used!");
    }

    [Test]
    public void Test_ItemCannotBeUsedMoreThanMaxUses()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        Item item = ScriptableObject.CreateInstance<Item>();

        item.Use(stats);
        item.Use(stats);
        item.Use(stats);

        item.Use(stats);

        Assert.Zero(item.GetUsesLeft(), "Item could be used even though it had 0 uses left!");
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

        Assert.Greater(healedHealth, damagedHealth, "Using a healing item did not increase the players health!");
    }

    [Test]
    public void Test_ItemCanDamageEnemies()
    {
        EnemyStatsClass stats = new EnemyStatsClass();
        IGameController gameCtr = Substitute.For<IGameController>();
        gameCtr.When(x => x.PlayerThrowBomb()).Do(x => stats.ReceiveDamage(10));

        Item item = ScriptableObject.CreateInstance<Item>();
        item.type = ItemType.DealDamage;
        item.Use(stats, gameCtr);

        int damagedHealth = stats.GetCurrentHealth();

        Assert.Less(damagedHealth, stats.GetMaxHealth(), "Using a bomb item did not decrease the enemies health!");
    }

    #endregion Effects
}
