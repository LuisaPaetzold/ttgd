using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Test_FighterStats
{

    public void Setup()
    {
        Debug.ClearDeveloperConsole(); // doesn't work?
    }

    #region Health
    [Test]
    public void Test_FighterBeginsWithMaximumHealth()
    {
        // Arrange
        FighterStatsClass stats = new FighterStatsClass();
        // Act

        // Assert
        Assert.AreEqual(stats.GetMaxHealth(), stats.GetCurrentHealth(), "Fighter didn't start with maximum health!");

    }

    [Test]
    public void Test_FighterCanBeDamaged()
    {
        FighterStatsClass stats = new FighterStatsClass();
        const int damage = 10;
        int expectedHealth = stats.GetMaxHealth() - damage - damage;

        stats.ReceiveDamage(damage);
        stats.ReceiveDamage(damage);

        Assert.AreNotEqual(stats.GetMaxHealth(), stats.GetCurrentHealth(), "Fighter health didn't change after being damaged!");
        Assert.AreEqual(expectedHealth, stats.GetCurrentHealth(), "Fighter health is not at the expected value after being damaged!");
    }

    [Test]
    public void Test_FighterHealthCannotDropUnderZero()
    {
        const int heavyDamage = 200;
        FighterStatsClass stats = new FighterStatsClass();

        stats.ReceiveDamage(heavyDamage);

        Assert.Zero(stats.GetCurrentHealth(), "Fighter health is not 0 after receiving massive damage!");
    }

    [Test]
    public void Test_FighterCanBeHealed()
    {
        FighterStatsClass stats = new FighterStatsClass();
        const int damage = 30;
        const int heal = 20;
        int damagedHealth = stats.GetMaxHealth() - damage;
        int expectedHealth = damagedHealth + heal;

        stats.ReceiveDamage(damage);
        stats.GetHealedBy(heal);

        Assert.AreEqual(expectedHealth, stats.GetCurrentHealth(), "Fighter health is not at the expected value after healing!");
        Assert.AreNotEqual(damagedHealth, stats.GetCurrentHealth(), "Fighter health didn't change after healing!");
    }

    [Test]
    public void Test_FighterHealthCannotExceedMaxHealth()
    {
        FighterStatsClass stats = new FighterStatsClass();
        const int heal = 20;

        stats.GetHealedBy(heal);

        Assert.AreEqual(stats.GetMaxHealth(), stats.GetCurrentHealth(), "Fighter health changed after healing with full health!");
        Assert.LessOrEqual(stats.GetCurrentHealth(), stats.GetMaxHealth(), "Fighter health exceeds full health after healing!");
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
        Assert.AreEqual(expectedHealth, stats.GetCurrentHealth(), "Fighter health was modified by negative healing!");
    }

    [Test]
    public void Test_FighterCannotReceiveNegativeDamage()
    {
        FighterStatsClass stats = new FighterStatsClass();
        const int wrongDamage = -30;

        stats.ReceiveDamage(wrongDamage);

        LogAssert.Expect(LogType.Warning, "Fighter cannot receive negative damage. Health will not be modified.");
        Assert.AreEqual(stats.GetMaxHealth(), stats.GetCurrentHealth(), "Fighter health was modified by negative damage!");
    }

    #endregion Health

    #region FighterState

    [Test]
    public void Test_FighterStartsGameAlive()
    {
        FighterStatsClass stats = new FighterStatsClass();

        Assert.AreEqual(FighterState.alive, stats.GetCurrentFighterState(), "Fighter didn't start the game alive!");
    }

    [Test]
    public void Test_FighterDiesIfHealthDropsToZero()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int killDamage = stats.GetCurrentHealth() + 10;

        stats.ReceiveDamage(killDamage);

        Assert.AreEqual(FighterState.dead, stats.GetCurrentFighterState(), "Fighter didn't die after health dropped to zero!");
    }

    [Test]
    public void Test_FighterIsOnLastBreathWhileHealthBelowThreshhold()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int lastBreathDamage = Mathf.FloorToInt(stats.GetMaxHealth() - (stats.GetLastBreathThreshold() * stats.GetMaxHealth()));
        //Debug.Log(lastBreathDamage);

        stats.ReceiveDamage(lastBreathDamage);

        Assert.AreEqual(FighterState.lastBreath, stats.GetCurrentFighterState(), "Fighter didn't get to last breath after his health dropped below the threshold!");
    }

    [Test]
    public void Test_FighterCanRevoverFromLastBreath()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int lastBreathDamage = Mathf.FloorToInt(stats.GetMaxHealth() - (stats.GetLastBreathThreshold() * stats.GetMaxHealth()));
        //Debug.Log(lastBreathDamage);
        const int heal = 10;

        stats.ReceiveDamage(lastBreathDamage);
        Assert.AreEqual(FighterState.lastBreath, stats.GetCurrentFighterState(), "Fighter didn't get to last breath after his health dropped below the threshold!");

        stats.GetHealedBy(heal);
        Assert.AreEqual(FighterState.alive, stats.GetCurrentFighterState(), "Fighter didn't recover from last breath after his health exceeded the threshold!");
    }

    #endregion FighterState

    #region Attack

    [Test]
    public void Test_FighterCanDealAttackDamage()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int dummyEnemyLife = 100;

        dummyEnemyLife -= stats.GetCurrentAttackDamage();

        Assert.Less(dummyEnemyLife, 100, "Enemy didn't receive any damage from fighter, no attack damage dealt!");
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

        Assert.AreNotEqual(dummyEnemyLifeHeavyDamage, dummyEnemyLifeNormalDamage, "Heavy damage and normal attack damage are equal!");
        Assert.Less(dummyEnemyLifeHeavyDamage, dummyEnemyLifeNormalDamage, "Heavy damage wasn't more that normal attack damage!");
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

        Assert.Less(dummyEnemyLifeHeavyDamage, dummyEnemyLifeNormalDamage2, "Heavy damage wasn't more that normal attack damage!");
        Assert.AreEqual(dummyEnemyLifeNormalDamage1, dummyEnemyLifeNormalDamage2, "Heavy damage wasn't reset after one attack!");
    }

    [Test]
    public void Test_FighterGetCurrentAttackDamageCanBeDisplayedWithoutReset()
    {
        FighterStatsClass stats = new FighterStatsClass();
        stats.UseChargeForDamageBoost();
        int chargedValueDisplayed = stats.GetCurrentAttackDamage(false);
        int chargedValueForAttack = stats.GetCurrentAttackDamage();
        int resettedValue = stats.GetCurrentAttackDamage(false);


        Assert.AreEqual(chargedValueDisplayed, chargedValueForAttack, "Displaying the attack damage resetted heavy damage even if it shouldn't have done that!");
        Assert.Less(resettedValue, chargedValueForAttack, "Heavy damage wasn't reset after one attack!");
    }

    [Test]
    public void Test_FighterCanChargeUpToThreeTimesButNotMore()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int normalAttackValue = stats.GetCurrentAttackDamage(false);
        stats.UseChargeForDamageBoost();
        int firstBoostValue = stats.GetCurrentAttackDamage(false);
        stats.UseChargeForDamageBoost();
        int secondBoostValue = stats.GetCurrentAttackDamage(false);
        stats.UseChargeForDamageBoost();
        int thirdBoostValue = stats.GetCurrentAttackDamage(false);
        stats.UseChargeForDamageBoost();
        int forthBoostValue = stats.GetCurrentAttackDamage(false);

        // TODO: Rechnung irgendwie hier rausziehen?
        int expectedDamage = Mathf.FloorToInt(stats.GetDefaultAttackDamage() * (1 + stats.GetMaxAmountOfChargings() * stats.GetChargeDamageBoost()));

        Assert.Less(normalAttackValue, firstBoostValue, "First charge for damage boost didn't increase damage!");
        Assert.Less(firstBoostValue, secondBoostValue, "Second charge for damage boost didn't increase damage!");
        Assert.Less(secondBoostValue, thirdBoostValue, "Third charge for damage boost didn't increase damage!");
        Assert.AreEqual(thirdBoostValue, forthBoostValue, "Forth charge for damage boost increased damage, but three boosts is the maximum!");
        Assert.AreEqual(expectedDamage, forthBoostValue, "Fighter didn't deal 3x boost damage after trying to boost 4 times");
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

        Assert.Less(oldDamage, newDamage, "Lasting damage boost didn't increase attack damage!");
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

        Assert.Less(normalDamage, firstBoostDamage, "First lasting damage boost didn't increase attack damage!");
        Assert.Less(firstBoostDamage, secondBoostDamage, "Second lasting damage boost didn't increase attack damage");
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
        Assert.AreEqual(boostedDamage, damageAfterAttemptedDuplication, "Managed to add two lasting damage boosts from the same source and increase damage with both, that shouldn't be possible.");
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

        Assert.Less(normalDamage, boostedDamage, "Lasting damage boost didn't increase attack damage!");
        Assert.AreEqual(normalDamage, damageAfterRemoval, "Lasting damage boost couldn't be removed!");
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

        Assert.Less(normalDamage, boostedDamage, "Lasting damage boost didn't increase attack damage!");
        Assert.AreEqual(boostedDamage, damageAfterAttemptedRemoval, "Attack damage was modified after trying to remove a non-existant lasting damage boost!");
        LogAssert.Expect(LogType.Warning, "Fighter cannot remove lasting damage boost of a source that never gave him a boost. Attacke damage will not be modified.");
    }

    #endregion Attack
}
