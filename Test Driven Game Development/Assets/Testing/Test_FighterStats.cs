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
        int expectedHealth = stats.GetMaxHealth() - damage;

        stats.ReceiveDamage(damage);

        Assert.AreNotEqual(stats.GetMaxHealth(), stats.GetCurrentHealth(), "Fighter health didn't change after being damaged!");
        Assert.Less(stats.GetCurrentHealth(), stats.GetMaxHealth(), "Fighter health somehow increased after being damaged!");
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

    [Test]
    public void Test_FighterSubclassesMustDeclareOwnDieFunction()
    {
        FighterStatsClass stats = new FighterStatsClass();
        stats.Die();

        LogAssert.Expect(LogType.Error, "Die() must be implemented inside the sub-class!");
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
        int killDamage = stats.GetCurrentHealth();

        stats.ReceiveDamage(killDamage);

        Assert.AreEqual(FighterState.dead, stats.GetCurrentFighterState(), "Fighter didn't die after health dropped to zero!");
    }

    [Test]
    public void Test_FighterIsOnLastBreathWhileHealthBelowThreshhold()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int lastBreathDamage = Mathf.FloorToInt(stats.GetMaxHealth() - (stats.GetLastBreathThreshold() * stats.GetMaxHealth()));

        stats.ReceiveDamage(lastBreathDamage);

        Assert.AreEqual(FighterState.lastBreath, stats.GetCurrentFighterState(), "Fighter didn't get to last breath after his health dropped below the threshold!");
    }

    [Test]
    public void Test_FighterCanRevoverFromLastBreath()
    {
        FighterStatsClass stats = new FighterStatsClass();
        int lastBreathDamage = Mathf.FloorToInt(stats.GetMaxHealth() - (stats.GetLastBreathThreshold() * stats.GetMaxHealth()));
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
        stats.UseChargeForDamageBoost();
        dummyEnemyLifeHeavyDamage -= stats.GetCurrentAttackDamage();
        dummyEnemyLifeNormalDamage2 -= stats.GetCurrentAttackDamage();

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
        Assert.AreEqual(expectedDamage, thirdBoostValue, "Fighter didn't deal 3x boost damage after trying to boost 4 times");
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

        Assert.Contains(sourceA, stats.lastingDamageBoosts.Keys, "Lasting damage boosts don't contain a boost from source A!");
        Assert.Contains(sourceB, stats.lastingDamageBoosts.Keys, "Lasting damage boosts don't contain a boost from source B!");
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
            // do nothing - we expected ArgumentException
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

        stats.RemoveLastingDamageBoost(sourceA);
        int damageAfterRemoval = stats.GetCurrentAttackDamage();

        Assert.IsEmpty(stats.lastingDamageBoosts.Keys, "Lasting damage boosts still contain a boost after it was removed!");
        Assert.AreEqual(normalDamage, damageAfterRemoval, "Lasting damage boost couldn't be removed!");
    }

    [Test]
    public void Test_FighterCannotRemoveNonexistantLastingDamageBoost()
    {
        FighterStatsClass stats = new FighterStatsClass();

        string sourceA = "Item A";
        float boostA = 0.2f;
        stats.AddLastingDamageBoost(sourceA, boostA);
        int boostedDamage = stats.GetCurrentAttackDamage();

        stats.RemoveLastingDamageBoost("wrongSource");
        int damageAfterAttemptedRemoval = stats.GetCurrentAttackDamage();
        
        Assert.AreEqual(boostedDamage, damageAfterAttemptedRemoval, "Attack damage was modified after trying to remove a non-existant lasting damage boost!");
        Assert.IsNotEmpty(stats.lastingDamageBoosts.Keys, "Lasting damage boost of different source was removed when trying to remove one of a non-existant source!");
        LogAssert.Expect(LogType.Warning, "Fighter cannot remove lasting damage boost of a source that never gave him a boost. Attacke damage will not be modified.");
    }

    [Test]
    public void Test_FighterCanAttackAnotherFighter()
    {
        FighterStatsClass stats1 = new FighterStatsClass();
        FighterStatsClass stats2 = new FighterStatsClass();

        int fullHealth = stats2.GetCurrentHealth();
        stats1.AttackOpponent(stats2, false);
        int damagedHealth = stats2.GetCurrentHealth();

        Assert.Less(damagedHealth, fullHealth, "Fighter wasn't able to attack another fighter and damage them!");
    }

    [Test]
    public void Test_FighterCannotAttackNullPointer()
    {
        FighterStatsClass stats1 = new FighterStatsClass();
        FighterStatsClass stats2 = null;

        stats1.AttackOpponent(stats2, false);

        LogAssert.Expect(LogType.Warning, "Fighter tried to attack an opponent that's a nnullpointer. Can't attack non-existant opponents!");
    }

    [Test]
    public void Test_FighterCanDodgeIncomingAttack()
    {
        FighterStatsClass stats1 = new FighterStatsClass();
        FighterStatsClass stats2 = new FighterStatsClass();
        stats2.DodgePropability = 1f;

        int fullHealth = stats2.GetCurrentHealth();
        stats1.AttackOpponent(stats2);
        int afterAttack = stats2.GetCurrentHealth();

        Assert.AreEqual(fullHealth, afterAttack, "Fighter wasn't able to dodge incoming attack!");
    }

    [Test]
    public void Test_FighterCannotDodgeUnavoidableAttack()
    {
        FighterStatsClass stats1 = new FighterStatsClass();
        FighterStatsClass stats2 = new FighterStatsClass();
        stats2.DodgePropability = 1f;

        int fullHealth = stats2.GetCurrentHealth();
        stats1.AttackOpponent(stats2, false);
        int afterAttack = stats2.GetCurrentHealth();

        Assert.Less(afterAttack, fullHealth, "Fighter was able to dodge unavoidable attack!");
    }

    #endregion Attack
}
