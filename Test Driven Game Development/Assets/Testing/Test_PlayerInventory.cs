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

        Assert.IsEmpty(inventory.GetCollectedItems(), "Player didn't start with an empty inventory!");
        Assert.IsNull(inventory.GetEquippedWeapon(), "Player didn't start without a weapon!");
    }

    [Test]
    public void Test_PlayerCanEquipWeapon()
    {
        Weapon weapon = (Weapon)ScriptableObject.CreateInstance("Weapon");
        PlayerInventoryClass inventory = new PlayerInventoryClass();

        inventory.EquipWeapon(weapon);

        Assert.IsNotNull(inventory.GetEquippedWeapon(), "Player didn't equip a weapon!");
    }

    [Test]
    public void Test_PlayerCanCollectItems()
    {
        PlayerInventoryClass inventory = new PlayerInventoryClass();
        Item item = (Item)ScriptableObject.CreateInstance("Item");

        inventory.CollectItem(item);

        Assert.IsNotEmpty(inventory.GetCollectedItems(), "Player wasn't able to collect an item into the inventory!");
        Assert.IsTrue(inventory.PlayerHasItem(item), "Player doesn't have the collected item in the inventory!");
    }

    [Test]
    public void Test_PlayerCanLoseItem()
    {
        PlayerInventoryClass inventory = new PlayerInventoryClass();
        Item item = (Item)ScriptableObject.CreateInstance("Item");

        inventory.CollectItem(item);

        Assert.IsNotEmpty(inventory.GetCollectedItems(), "Player wasn't able to collect an item into the inventory!");

        inventory.RemoveItem(item);

        Assert.IsEmpty(inventory.GetCollectedItems(), "Player wasn't able to remove an item from the inventory!");
    }
}
