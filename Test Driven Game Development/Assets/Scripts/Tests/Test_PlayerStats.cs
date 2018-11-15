using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Test_PlayerStats
{

    [Test]
    public void Test_PlayerBeginsWithMaximumHealth()
    {
        // Setup
		PlayerStatsClass stats = new PlayerStatsClass();
        // Action?
        Assert.AreEqual(stats.GetMaxHealth(), stats.GetCurrentHealth());
        // Teardown

    }

    [Test]
    public void Test_PlayerBeginsWithZeroPoints()
    {
        const int startPoints = 0;
        PlayerStatsClass stats = new PlayerStatsClass();

        Assert.AreEqual(startPoints, stats.GetCurrentPoints());
    }

    [Test]
    public void Test_PlayerCanBeDamaged()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        const int damage = 10;
        int expectedHealth = stats.GetMaxHealth() - damage - damage;

        stats.ReceiveDamage(damage);
        stats.ReceiveDamage(damage);

        Assert.AreEqual(expectedHealth, stats.GetCurrentHealth());
    }

    [Test]
    public void Test_PlayerHealthCannotDropUnderZero()
    {
        const int heavyDamage = 200;
        PlayerStatsClass stats = new PlayerStatsClass();

        stats.ReceiveDamage(heavyDamage);

        Assert.AreEqual(0, stats.GetCurrentHealth());
    }

    [Test]
    public void Test_PlayerCanBeHealed()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        const int damage = 30;
        const int heal = 20;
        int expectedHealth = stats.GetMaxHealth() - damage + heal;

        stats.ReceiveDamage(damage);
        stats.GetHealedBy(heal);

        Assert.AreEqual(expectedHealth, stats.GetCurrentHealth());
    }

    [Test]
    public void Test_PlayerHealthCannotExceedMaxHealth()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        const int heal = 20;
        
        stats.GetHealedBy(heal);

        Assert.AreEqual(stats.GetMaxHealth(), stats.GetCurrentHealth());
    }

    [Test]
    public void Test_PlayerCannotReceiveNegativeHealing()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        const int damage = 30;
        const int wrongHeal = -20;
        int expectedHealth = stats.GetMaxHealth() - damage;

        stats.ReceiveDamage(damage);
        stats.GetHealedBy(wrongHeal);

        LogAssert.Expect(LogType.Warning, "Player cannot be healed by a negative amount. Health will not be modified.");
        Assert.AreEqual(expectedHealth, stats.GetCurrentHealth());
    }

    [Test]
    public void Test_PlayerCannotReceiveNegativeDamage()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        const int wrongDamage = -30;

        stats.ReceiveDamage(wrongDamage);

        LogAssert.Expect(LogType.Warning, "Player cannot receive negative damage. Health will not be modified.");
        Assert.AreEqual(stats.GetMaxHealth(), stats.GetCurrentHealth());
    }

    [Test]
    public void Test_PlayerCanGainPoints()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        const int gainedPoints = 10;
        int expectedPoints = stats.GetCurrentPoints() + gainedPoints;

        stats.ModifyPoints(gainedPoints);

        Assert.AreEqual(expectedPoints, stats.GetCurrentPoints());
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

        Assert.AreEqual(expectedPoints, stats.GetCurrentPoints());
    }

    [Test]
    public void Test_PlayerPointsCannotDropBelowZero()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        const int lostPoints = -10;

        stats.ModifyPoints(lostPoints);

        Assert.AreEqual(0, stats.GetCurrentPoints());
    }

    [Test]
    public void Test_PlayerStartsGameAlive()
    {
        PlayerStatsClass stats = new PlayerStatsClass();

        Assert.AreEqual(PlayerState.alive, stats.GetCurrentPlayerState());
    }

    [Test]
    public void Test_PlayerDiesIfHealthDropsToZero()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        int killDamage = stats.GetCurrentHealth() + 10;

        stats.ReceiveDamage(killDamage);

        Assert.AreEqual(PlayerState.dead, stats.GetCurrentPlayerState());
    }

    [Test]
    public void Test_PlayerIsOnLastBreathWhileHealthBelowThreshhold()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        int lastBreathDamage = Mathf.FloorToInt(stats.GetMaxHealth() - (stats.GetLastBreathThreshold() * stats.GetMaxHealth()));
        //Debug.Log(lastBreathDamage);

        stats.ReceiveDamage(lastBreathDamage);

        Assert.AreEqual(PlayerState.lastBreath, stats.GetCurrentPlayerState());
    }

    [Test]
    public void Test_PlayerCanRevoverFromLastBreath()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        int lastBreathDamage = Mathf.FloorToInt(stats.GetMaxHealth() - (stats.GetLastBreathThreshold() * stats.GetMaxHealth()));
        //Debug.Log(lastBreathDamage);
        const int heal = 10;

        stats.ReceiveDamage(lastBreathDamage);
        Assert.AreEqual(PlayerState.lastBreath, stats.GetCurrentPlayerState());

        stats.GetHealedBy(heal);
        Assert.AreEqual(PlayerState.alive, stats.GetCurrentPlayerState());
    }

    [Test]
    public void Test_PlayerCanDealAttackDamage()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        int dummyEnemyLife = 100;

        dummyEnemyLife -= stats.GetCurrentAttackDamage();

        Assert.IsTrue(dummyEnemyLife < 100);
    }

    [Test]
    public void Test_PlayerCanChargeForHeavyDamage()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        int dummyEnemyLifeNormalDamage = 100;
        int dummyEnemyLifeHeavyDamage = 100;

        dummyEnemyLifeNormalDamage -= stats.GetCurrentAttackDamage();
        stats.UseChargeForDamageBoost();
        dummyEnemyLifeHeavyDamage -= stats.GetCurrentAttackDamage();

        Assert.AreNotEqual(dummyEnemyLifeHeavyDamage, dummyEnemyLifeNormalDamage);
        Assert.IsTrue(dummyEnemyLifeHeavyDamage < dummyEnemyLifeNormalDamage);
    }

    [Test]
    public void Test_PlayerChargeHeavyDamageIsRemovedAfterAttack()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        int dummyEnemyLifeNormalDamage1 = 100;
        int dummyEnemyLifeHeavyDamage = 100;
        int dummyEnemyLifeNormalDamage2 = 100;

        dummyEnemyLifeNormalDamage1 -= stats.GetCurrentAttackDamage();
        //Debug.Log(stats.GetCurrentAttackDamage(false));
        stats.UseChargeForDamageBoost();
        //Debug.Log(stats.GetCurrentAttackDamage(false));
        dummyEnemyLifeHeavyDamage -= stats.GetCurrentAttackDamage();
        dummyEnemyLifeNormalDamage2 -= stats.GetCurrentAttackDamage();
        //Debug.Log(stats.GetCurrentAttackDamage(false));

        Assert.AreNotEqual(dummyEnemyLifeHeavyDamage, dummyEnemyLifeNormalDamage2);
        Assert.AreEqual(dummyEnemyLifeNormalDamage1, dummyEnemyLifeNormalDamage2);
    }

    [Test]
    public void Test_PlayerGetCurrentAttackDamageCanBeDisplayedWithoutReset()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        stats.UseChargeForDamageBoost();
        int attackValueForDisplay = stats.GetCurrentAttackDamage(false);
        int attackValueAfterDisplayForAttack = stats.GetCurrentAttackDamage();
        int attackValueForDisplayAfterAttack = stats.GetCurrentAttackDamage(false);


        Assert.AreEqual(attackValueForDisplay, attackValueAfterDisplayForAttack);
        Assert.AreNotEqual(attackValueAfterDisplayForAttack, attackValueForDisplayAfterAttack);
    }

    [Test]
    public void Test_PlayerCanChargeUpToThreeTimesButNotMore()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        stats.UseChargeForDamageBoost();
        int firstAttackValue = stats.GetCurrentAttackDamage(false);
        stats.UseChargeForDamageBoost();
        int secondAttackValue = stats.GetCurrentAttackDamage(false);
        stats.UseChargeForDamageBoost();
        int thirdAttackValue = stats.GetCurrentAttackDamage(false);
        stats.UseChargeForDamageBoost();
        int forthAttackValue = stats.GetCurrentAttackDamage(false);

        // TODO: Rechnung irgendwie hier rausziehen?
        int expectedDamage = Mathf.FloorToInt(stats.GetDefaultAttackDamage() * ( 1 + stats.GetMaxAmountOfChargings() * stats.GetChargeDamageBoost()));

        Assert.IsTrue(firstAttackValue < secondAttackValue && secondAttackValue < thirdAttackValue);
        Assert.AreEqual(thirdAttackValue, forthAttackValue);
        Assert.AreEqual(expectedDamage, forthAttackValue);
    }

    //TODO Lasting Damage boost

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
