using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Test_PlayerInventory
{
    [Test]
    public void Test_PlayerStartsWithEmptyInventory()
    {
        PlayerInventoryClass inventory = new PlayerInventoryClass();

        Assert.IsNull(inventory.GetEquippedWeapon(), "Player didn't start with an empty inventory!");
    }


    [Test]
    public void Test_PlayerCanEquipWeapon()
    {
        Weapon weapon = new Weapon();
        PlayerInventoryClass inventory = new PlayerInventoryClass();
        
        inventory.EquipWeapon(weapon);

        Assert.IsNotNull(inventory.GetEquippedWeapon(), "Player didn't equip a weapon!");
    }

}
