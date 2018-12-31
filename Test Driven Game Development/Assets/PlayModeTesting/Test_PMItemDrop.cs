using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NSubstitute;
using TMPro;

public class Test_PMItemDrop
{
    [UnityTest]
    public IEnumerator Test_ItemDropGetsRemovedAfterBeingCollected()
    {
        Player player = CreatePlayer();
        ItemDrop drop = CreateItemDrop();

        yield return new WaitForEndOfFrame();

        drop.PlayerCollectItem();

        yield return new WaitForEndOfFrame();

        ItemDrop[] foundDrops = GameObject.FindObjectsOfType<ItemDrop>();

        Assert.Zero(foundDrops.Length, "Item drop was not deleted after collecting it!");
    }

    [UnityTest]
    public IEnumerator Test_ItemDropTextShownIfPlayerCloseEnough()
    {
        Player player = CreatePlayer();
        ItemDrop drop = CreateItemDrop();
        drop.transform.position = new Vector3(drop.maxDisplayDistance - 1, 0, 0);
        drop.player = player;
        Canvas canvas = drop.GetComponentInChildren<Canvas>();

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(canvas.gameObject.activeSelf, "Item drop text was not shown even though player was close enough!");
    }

    [UnityTest]
    public IEnumerator Test_ItemDropTextNotShownIfPlayerTooFar()
    {
        Player player = CreatePlayer();
        ItemDrop drop = CreateItemDrop();
        drop.transform.position = new Vector3(drop.maxDisplayDistance + 1, 0, 0);
        drop.player = player;
        Canvas canvas = drop.GetComponentInChildren<Canvas>();

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(canvas.gameObject.activeSelf, "Item drop text was shown even though player was too far away!");
    }

    [UnityTest]
    public IEnumerator Test_ItemDropTextIndicatesFullInventory()
    {
        Player player = CreatePlayer();
        player.inventory.MaxItemSlots = 0;
        ItemDrop drop = CreateItemDrop();
        drop.player = player;
        TextMeshProUGUI text = drop.GetComponentInChildren<TextMeshProUGUI>();

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Assert.IsTrue(text.text == "Inventory full!", "Item drop text did not inform the player about full inventory!");

        player.inventory.MaxItemSlots = 1;
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(text.text == "Inventory full!", "Item drop text did not get set back to the normal text!");
    }

    [UnityTest]
    public IEnumerator Test_ItemDropIncreasesUsesIfItemAlreadyInInventory()
    {
        Player player = CreatePlayer();
        ItemDrop drop = CreateItemDrop();
        player.inventory.CollectItem(drop.droppedItem);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(player.inventory.PlayerHasItem(drop.droppedItem), "Player did not collect the item the first time!");
        Assert.AreEqual(drop.droppedItem.GetMaxUses(), player.inventory.items[0].GetUsesLeft(), "Item uses were not correct even though nothing happened!");

        drop.PlayerCollectItem();

        yield return new WaitForEndOfFrame();

        Assert.AreEqual(1, player.inventory.items.Count, "Item was collected a second time instead of increasing uses of existing item!");
        Assert.AreEqual(drop.droppedItem.GetMaxUses() * 2, player.inventory.items[0].GetUsesLeft(), "Item uses of existing item were increased by the right amount!");
    }

    [UnityTest]
    public IEnumerator Test_ItemDropCannotBeCollectedIfNotYetInInventoryAndNoFreeSlot()
    {
        Player player = CreatePlayer();
        ItemDrop drop = CreateItemDrop();
        drop.droppedItem.type = ItemType.AttackBoost;
        ItemDrop drop1 = CreateItemDrop();
        drop1.droppedItem.type = ItemType.Healing;
        player.inventory.MaxItemSlots = 1;

        yield return new WaitForEndOfFrame();

        drop.PlayerCollectItem();

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(player.inventory.PlayerHasItem(drop.droppedItem), "Player did not collect the first item!");

        drop1.PlayerCollectItem();

        yield return new WaitForEndOfFrame();

        Assert.IsFalse(player.inventory.PlayerHasItem(drop1.droppedItem), "Player was able to collect the second item even though there was no space!");
    }





    // --------------------- helper methods ----------------------------------------

    public Player CreatePlayer(bool setUpComponentsInTest = true)
    {
        Player p = new GameObject().AddComponent<Player>();
        if (setUpComponentsInTest)
        {
            p.stats = new PlayerStatsClass();
            p.inventory = new PlayerInventoryClass();
        }
        return p;
    }

    public ItemDrop CreateItemDrop()
    {
        ItemDrop drop = new GameObject("drop").AddComponent<ItemDrop>();
        drop.droppedItem = ScriptableObject.CreateInstance<Item>();

        Canvas canvas = new GameObject("canvas").AddComponent<Canvas>();
        canvas.gameObject.transform.SetParent(drop.transform);

        TextMeshProUGUI text = new GameObject("text").AddComponent<TextMeshProUGUI>();
        text.gameObject.transform.SetParent(canvas.transform);

        return drop;
    }
}
