using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Test_Weapon
{
    [SetUp]
    public void Setup()
    {
        Debug.ClearDeveloperConsole();
    }

    #region Start
    [Test]
    public void Test_WeaponStarts()
    {
        //Weapon weapon = (Weapon)ScriptableObject.CreateInstance("Weapon");
    }
    #endregion Start
}
