using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Test_EnemyStats
{
    #region Attack

    [Test]
    public void Test_EnemyStatsHasDeclaredOwnShowDodgeFunction()
    {
        EnemyStatsClass stats = new EnemyStatsClass();
        stats.ShowDodge();

        LogAssert.NoUnexpectedReceived();
    }

    #endregion Attack
}
