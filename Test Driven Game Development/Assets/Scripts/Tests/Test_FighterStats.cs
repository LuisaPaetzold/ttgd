using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Test_FighterStats {

    #region Health
    [Test]
    public void Test_FighterBeginsWithMaximumHealth()
    {
        // Setup
        FighterStatsClass stats = new FighterStatsClass();
        // Action?
        Assert.AreEqual(stats.GetMaxHealth(), stats.GetCurrentHealth());
        // Teardown

    }

    [Test]
    public void Test_FighterCanBeDamaged()
    {
        FighterStatsClass stats = new FighterStatsClass();
        const int damage = 10;
        int expectedHealth = stats.GetMaxHealth() - damage - damage;

        stats.ReceiveDamage(damage);
        stats.ReceiveDamage(damage);

        Assert.AreEqual(expectedHealth, stats.GetCurrentHealth());
    }

    [Test]
    public void Test_FighterHealthCannotDropUnderZero()
    {
        const int heavyDamage = 200;
        FighterStatsClass stats = new FighterStatsClass();

        stats.ReceiveDamage(heavyDamage);

        Assert.AreEqual(0, stats.GetCurrentHealth());
    }

    [Test]
    public void Test_FighterCanBeHealed()
    {
        FighterStatsClass stats = new FighterStatsClass();
        const int damage = 30;
        const int heal = 20;
        int expectedHealth = stats.GetMaxHealth() - damage + heal;

        stats.ReceiveDamage(damage);
        stats.GetHealedBy(heal);

        Assert.AreEqual(expectedHealth, stats.GetCurrentHealth());
    }

    [Test]
    public void Test_FighterHealthCannotExceedMaxHealth()
    {
        FighterStatsClass stats = new FighterStatsClass();
        const int heal = 20;

        stats.GetHealedBy(heal);

        Assert.AreEqual(stats.GetMaxHealth(), stats.GetCurrentHealth());
    }

    [Test]
    public void Test_FighterCannotReceiveNegativeHealing()
    {
        FighterStatsClass stats = new FighterStatsClass();
        const int damage = 30;
        const int wrongHeal = -20;
        int expectedHealth = stats.GetMaxHealth() - damage;

        stats.ReceiveDamage(damage);
        stats.GetHealedBy(wrongHeal);

        LogAssert.Expect(LogType.Warning, "Fighter cannot be healed by a negative amount. Health will not be modified.");
        Assert.AreEqual(expectedHealth, stats.GetCurrentHealth());
    }

    [Test]
    public void Test_FighterCannotReceiveNegativeDamage()
    {
        FighterStatsClass stats = new FighterStatsClass();
        const int wrongDamage = -30;

        stats.ReceiveDamage(wrongDamage);

        LogAssert.Expect(LogType.Warning, "Fighter cannot receive negative damage. Health will not be modified.");
        Assert.AreEqual(stats.GetMaxHealth(), stats.GetCurrentHealth());
    }

    #endregion Health

    #region FighterState

    [Test]
    public void Test_FighterStartsGameAlive()
    {
        FighterStatsClass stats = new FighterStatsClass();

        Assert.AreEqual(FighterState.alive, stats.GetCurrentFighterState());
    }

    [Test]
    public void Test_FighterDiesIfHealthDropsToZero()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int killDamage = stats.GetCurrentHealth() + 10;

        stats.ReceiveDamage(killDamage);

        Assert.AreEqual(FighterState.dead, stats.GetCurrentFighterState());
    }

    [Test]
    public void Test_FighterIsOnLastBreathWhileHealthBelowThreshhold()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int lastBreathDamage = Mathf.FloorToInt(stats.GetMaxHealth() - (stats.GetLastBreathThreshold() * stats.GetMaxHealth()));
        //Debug.Log(lastBreathDamage);

        stats.ReceiveDamage(lastBreathDamage);

        Assert.AreEqual(FighterState.lastBreath, stats.GetCurrentFighterState());
    }

    [Test]
    public void Test_FighterCanRevoverFromLastBreath()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int lastBreathDamage = Mathf.FloorToInt(stats.GetMaxHealth() - (stats.GetLastBreathThreshold() * stats.GetMaxHealth()));
        //Debug.Log(lastBreathDamage);
        const int heal = 10;

        stats.ReceiveDamage(lastBreathDamage);
        Assert.AreEqual(FighterState.lastBreath, stats.GetCurrentFighterState());

        stats.GetHealedBy(heal);
        Assert.AreEqual(FighterState.alive, stats.GetCurrentFighterState());
    }

    #endregion FighterState

    #region Attack

    [Test]
    public void Test_FighterCanDealAttackDamage()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int dummyEnemyLife = 100;

        dummyEnemyLife -= stats.GetCurrentAttackDamage();

        Assert.Less(dummyEnemyLife, 100);
    }

    [Test]
    public void Test_FighterCanChargeForHeavyDamage()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int dummyEnemyLifeNormalDamage = 100;
        int dummyEnemyLifeHeavyDamage = 100;

        dummyEnemyLifeNormalDamage -= stats.GetCurrentAttackDamage();
        stats.UseChargeForDamageBoost();
        dummyEnemyLifeHeavyDamage -= stats.GetCurrentAttackDamage();

        Assert.AreNotEqual(dummyEnemyLifeHeavyDamage, dummyEnemyLifeNormalDamage);
        Assert.Less(dummyEnemyLifeHeavyDamage, dummyEnemyLifeNormalDamage);
    }

    [Test]
    public void Test_FighterChargeHeavyDamageIsRemovedAfterAttack()
    {
        FighterStatsClass stats = new FighterStatsClass();
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

        Assert.Less(dummyEnemyLifeHeavyDamage, dummyEnemyLifeNormalDamage2);
        Assert.AreEqual(dummyEnemyLifeNormalDamage1, dummyEnemyLifeNormalDamage2);
    }

    [Test]
    public void Test_FighterGetCurrentAttackDamageCanBeDisplayedWithoutReset()
    {
        FighterStatsClass stats = new FighterStatsClass();
        stats.UseChargeForDamageBoost();
        int chargedValueDisplayed = stats.GetCurrentAttackDamage(false);
        int chargedValueForAttack = stats.GetCurrentAttackDamage();
        int resettedValue = stats.GetCurrentAttackDamage(false);


        Assert.AreEqual(chargedValueDisplayed, chargedValueForAttack);
        Assert.Less(resettedValue, chargedValueForAttack);
    }

    [Test]
    public void Test_FighterCanChargeUpToThreeTimesButNotMore()
    {
        FighterStatsClass stats = new FighterStatsClass();
        stats.UseChargeForDamageBoost();
        int firstAttackValue = stats.GetCurrentAttackDamage(false);
        stats.UseChargeForDamageBoost();
        int secondAttackValue = stats.GetCurrentAttackDamage(false);
        stats.UseChargeForDamageBoost();
        int thirdAttackValue = stats.GetCurrentAttackDamage(false);
        stats.UseChargeForDamageBoost();
        int forthAttackValue = stats.GetCurrentAttackDamage(false);

        // TODO: Rechnung irgendwie hier rausziehen?
        int expectedDamage = Mathf.FloorToInt(stats.GetDefaultAttackDamage() * (1 + stats.GetMaxAmountOfChargings() * stats.GetChargeDamageBoost()));

        Assert.Less(firstAttackValue, secondAttackValue); 
        Assert.Less(secondAttackValue, thirdAttackValue);
        Assert.AreEqual(thirdAttackValue, forthAttackValue);
        Assert.AreEqual(expectedDamage, forthAttackValue);
    }

    [Test]
    public void Test_FighterCanReceiveLastingDamageBoost()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int oldDamage = stats.GetCurrentAttackDamage();

        string source = "Weapon X";
        float boost = 0.2f;
        stats.AddLastingDamageBoost(source, boost);

        int newDamage = stats.GetCurrentAttackDamage();

        Assert.Less(oldDamage, newDamage);
    }

    [Test]
    public void Test_FighterCanReceiveMultipleLastingDamageBoosts()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int normalDamage = stats.GetCurrentAttackDamage();

        string sourceA = "Item A";
        float boostA = 0.2f;
        stats.AddLastingDamageBoost(sourceA, boostA);
        int firstBoostDamage = stats.GetCurrentAttackDamage();

        string sourceB = "Item B";
        float boostB = 0.2f;
        stats.AddLastingDamageBoost(sourceB, boostB);
        int secondBoostDamage = stats.GetCurrentAttackDamage();

        Assert.Less(normalDamage, firstBoostDamage);
        Assert.Less(firstBoostDamage, secondBoostDamage);
    }


    [Test]
    public void Test_FighterCanReceiveOnlyOneLastingDamageBoostFromSameSource()
    {
        FighterStatsClass stats = new FighterStatsClass();

        string sourceA = "Item A";
        float boostA = 0.2f;
        stats.AddLastingDamageBoost(sourceA, boostA);
        int boostedDamage = stats.GetCurrentAttackDamage();
        try
        {
            stats.AddLastingDamageBoost(sourceA, boostA);
            Assert.Fail("Managed to add two lasting damage boosts from the same source without ArgumentException, that shouldn't be possible.");
        }
        catch (System.ArgumentException)
        {
            // do nothing
        }
        int damageAfterAttemptedDuplication = stats.GetCurrentAttackDamage();

        LogAssert.Expect(LogType.Warning, "Fighter cannot receive multiple lasting damage boosts from the same source. Attacke damage will not be modified.");
        Assert.AreEqual(boostedDamage, damageAfterAttemptedDuplication);
    }

    [Test]
    public void Test_FighterCanRemoveLastingDamageBoost()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int normalDamage = stats.GetCurrentAttackDamage();

        string sourceA = "Item A";
        float boostA = 0.2f;
        stats.AddLastingDamageBoost(sourceA, boostA);
        int boostedDamage = stats.GetCurrentAttackDamage();

        stats.RemoveLastingDamageBoost(sourceA);
        int damageAfterRemoval = stats.GetCurrentAttackDamage();

        Assert.Less(normalDamage, boostedDamage);
        Assert.AreEqual(normalDamage, damageAfterRemoval);
    }

    [Test]
    public void Test_FighterCannotRemoveNonexistantLastingDamageBoost()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int normalDamage = stats.GetCurrentAttackDamage();

        string sourceA = "Item A";
        float boostA = 0.2f;
        stats.AddLastingDamageBoost(sourceA, boostA);
        int boostedDamage = stats.GetCurrentAttackDamage();

        stats.RemoveLastingDamageBoost("wrongSource");
        int damageAfterAttemptedRemoval = stats.GetCurrentAttackDamage();

        Assert.Less(normalDamage, boostedDamage);
        Assert.AreEqual(boostedDamage, damageAfterAttemptedRemoval);
        LogAssert.Expect(LogType.Warning, "Fighter cannot remove lasting damage boost of a source that never gave him a boost. Attacke damage will not be modified.");
    }

    #endregion Attack
}
