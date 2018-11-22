using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Test_PlayerStats
{
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

    /*[Test]
    public void Test_PlayerCanEquipWeapon()
    {
        Weapon weapon = new Weapon();
        PlayerStatsClass stats = new PlayerStatsClass();

        int damageWithoutWeapon = stats.GetCurrentAttackDamage();
        stats.EquipWeapon(weapon);
        int damageAfterEquip = stats.GetCurrentAttackDamage();

        Assert.IsTrue(stats.GetEquippedWeapon() != null);
        Assert.IsTrue(damageWithoutWeapon < damageAfterEquip);
    }*/

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
