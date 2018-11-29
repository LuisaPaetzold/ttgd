using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Test_EnemyStats
{
    [Test]
    public void Test_EnemyStatsHasDeclaredOwnDieFunction()
    {
        PlayerStatsClass stats = new PlayerStatsClass();
        stats.Die();

        LogAssert.NoUnexpectedReceived();
    }
}
