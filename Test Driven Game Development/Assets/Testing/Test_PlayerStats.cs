using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NSubstitute;

public class Test_PlayerStats
{
    #region Points
    [Test]
    public void Test_PlayerBeginsWithZeroPoints()
    {
        const int startPoints = 0;
        PlayerStatsClass stats = new PlayerStatsClass();

        Assert.AreEqual(startPoints, stats.GetCurrentPoints(), "Player didn't start with 0 points!");
    }

    [Test]
    public void Test_PlayerCanGainPoints()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        const int gainedPoints = 10;
        int expectedPoints = stats.GetCurrentPoints() + gainedPoints;

        stats.ModifyPoints(gainedPoints);

        Assert.AreEqual(expectedPoints, stats.GetCurrentPoints(), "Player didn't gain the expected amount of points!");
        Assert.NotZero(stats.GetCurrentPoints(), "Player didn't gain any points!");
    }

    [Test]
    public void Test_PlayerCanLosePoints()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        const int gainedPoints = 20;
        const int lostPoints = -10;
        int expectedPoints = stats.GetCurrentPoints() + gainedPoints + lostPoints;

        stats.ModifyPoints(gainedPoints);
        stats.ModifyPoints(lostPoints);

        Assert.AreEqual(expectedPoints, stats.GetCurrentPoints(), "Player didn't lose the expected amount of points!");
        Assert.AreNotEqual(gainedPoints, stats.GetCurrentPoints(), "Player didn't lose any points!");
    }

    [Test]
    public void Test_PlayerPointsCannotDropBelowZero()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        const int lostPoints = -10;

        stats.ModifyPoints(lostPoints);

        Assert.Zero(stats.GetCurrentPoints(), "Player lost more points that he had and dropped below zero!");
    }

    [Test]
    public void Test_PlayerGainsPointsAfterKillingAnEnemy()
    {
        PlayerStatsClass player = new PlayerStatsClass();
        player.AttackDamage = 500;
        IGameController mockController = Substitute.For<IGameController>();
        mockController.GetPlayerStats().Returns(player);
        EnemyStatsClass enemy = new EnemyStatsClass();
        enemy.SetUpEnemyStats(mockController);


        int pointsBefore = player.GetCurrentPoints();
        player.AttackOpponent(enemy, false);
        int pointsAfter = player.GetCurrentPoints();

        Assert.Greater(pointsAfter, pointsBefore, "Player did not receive any points after killing an enemy!");
    }

    [Test]
    public void Test_PlayerGainsNoPointsAfterTryingToKillADeadEnemy()
    {
        PlayerStatsClass player = new PlayerStatsClass();
        player.AttackDamage = 500;
        IGameController mockController = Substitute.For<IGameController>();
        mockController.GetPlayerStats().Returns(player);
        EnemyStatsClass enemy = new EnemyStatsClass();
        enemy.SetUpEnemyStats(mockController);

        
        player.AttackOpponent(enemy, false);
        int pointsAfterFirstKill = player.GetCurrentPoints();
        player.AttackOpponent(enemy, false);
        int pointsAfterSecondKill = player.GetCurrentPoints();
        
        Assert.AreEqual(pointsAfterFirstKill, pointsAfterSecondKill, "Player received points after trying to kill an enemy that's already dead!");
        LogAssert.Expect(LogType.Warning, "Fighter tried to attack an opponent that already died. Can't attack dead opponents!");
    }

    #endregion Points

    #region Damage

    [Test]
    public void Test_PlayerReceivesDamageIncreaseFromEquippedWeapon()
    {
        Weapon weapon = ScriptableObject.CreateInstance<Weapon>();
        PlayerStatsClass stats = new PlayerStatsClass();
        PlayerInventoryClass inventory = new PlayerInventoryClass();

        int damageWithoutWeapon = stats.GetCurrentAttackDamage();
        inventory.EquipWeapon(weapon);

        IPlayer mockPlayer = Substitute.For<IPlayer>();
        mockPlayer.GetAllDamageBonus().Returns(inventory.GetEquippedWeapon().damage);
        stats.SetPlayerAddition(mockPlayer);

        int damageAfterEquip = stats.GetCurrentAttackDamage();

        Assert.IsNotNull(inventory.GetEquippedWeapon(), "Player didn't equip the weapon!");
        Assert.Greater(damageAfterEquip, damageWithoutWeapon, "Player damage did not increase after equipping a weapon!");
    }

    #endregion Damage

    #region Health

    // this test became out-dated, will keep it around for reference
    /*[Test]
    public void Test_PlayerStatsHasDeclaredOwnDieFunction()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        stats.HandleDeath();

        LogAssert.NoUnexpectedReceived();
    }*/

    #endregion Health

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    /*[UnityTest]
    public IEnumerator Test_PlayerStatsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }*/
}
