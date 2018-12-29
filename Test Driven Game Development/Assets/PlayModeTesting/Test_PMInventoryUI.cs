using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using TMPro;

public class Test_PMInventoryUI
{
    // to test:
    // button images show icons? disabled when no item?
    // button text shows uses left. no text when no item
    // 

    [UnityTest]
    public IEnumerator SlotImagesShowItemIcon()
    {
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        InventoryUI inventoryUI = CreateInventoryUI();
        inventoryUI.GameCtr = gameCtr;
        Item item = ScriptableObject.CreateInstance<Item>();
        item.Icon = CreateExampleSprite();
     
        yield return new WaitForEndOfFrame();

        Assert.IsNotNull(player.inventory, "Player did not have an inventory!");
        player.inventory.CollectItem(item);

        yield return new WaitForEndOfFrame();

        Image imageToTest = inventoryUI.GetImageOfSlot(inventoryUI.Slot1);

        Assert.AreEqual(item.GetIcon(), imageToTest.sprite, "Item icon was not displayed correctly!");
        Assert.IsNotNull(imageToTest.sprite, "Displayed icon was null even though there was an item!");
        Assert.IsTrue(imageToTest.enabled, "Item icon was not enabled even though there was an item!");
    }

    [UnityTest]
    public IEnumerator SlotTextShowsItemUsesLeft()
    {
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        InventoryUI inventoryUI = CreateInventoryUI();
        inventoryUI.GameCtr = gameCtr;
        Item item = ScriptableObject.CreateInstance<Item>();

        yield return new WaitForEndOfFrame();

        Assert.IsNotNull(player.inventory, "Player did not have an inventory!");
        player.inventory.CollectItem(item);

        yield return new WaitForEndOfFrame();
        
        TextMeshProUGUI textToTest = inventoryUI.GetUsesTextOfSlot(inventoryUI.Slot1);
        
        Assert.AreEqual(item.GetUsesLeft().ToString(), textToTest.text, "Item uses were not displayed correctly!");
    }

    [UnityTest]
    public IEnumerator SlotImagesShowNothingWhenSlotIsEmpty()
    {
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        InventoryUI inventoryUI = CreateInventoryUI();
        inventoryUI.GameCtr = gameCtr;
        Item item = ScriptableObject.CreateInstance<Item>();
        item.Icon = CreateExampleSprite();

        yield return new WaitForEndOfFrame();

        Assert.IsNotNull(player.inventory, "Player did not have an inventory!");

        yield return new WaitForEndOfFrame();

        Image imageToTest = inventoryUI.GetImageOfSlot(inventoryUI.Slot1);

        Assert.AreNotEqual(item.GetIcon(), imageToTest.sprite, "Item icon was displayed even though there was no item!");
        Assert.IsNull(imageToTest.sprite, "Displayed icon was not null even though there was no item!");
        Assert.IsFalse(imageToTest.enabled, "Item icon was enabled even though there was no item!");
    }

    [UnityTest]
    public IEnumerator SlotTextShowsNothingWhenNoItem()
    {
        Player player = CreatePlayer();
        GameController gameCtr = CreateGameController(player);
        InventoryUI inventoryUI = CreateInventoryUI();
        inventoryUI.GameCtr = gameCtr;
        Item item = ScriptableObject.CreateInstance<Item>();

        yield return new WaitForEndOfFrame();

        Assert.IsNotNull(player.inventory, "Player did not have an inventory!");

        yield return new WaitForEndOfFrame();

        TextMeshProUGUI textToTest = inventoryUI.GetUsesTextOfSlot(inventoryUI.Slot1);

        Assert.AreNotEqual(item.GetUsesLeft().ToString(), textToTest.text, "Item uses were displayed even though there was no item!");
        Assert.AreEqual("", textToTest.text, "Item uses were not displayed as empty even though there was no item!");
    }



    // --------------------- helper methods ----------------------------------------

    public Player CreatePlayer()
    {
        Player p = new GameObject().AddComponent<Player>();
        p.stats = new PlayerStatsClass();
        p.inventory = new PlayerInventoryClass();
        return p;
    }

    public Enemy CreateEnemy(bool autoAttack)
    {
        Enemy e = new GameObject().AddComponent<Enemy>();
        e.stats = new EnemyStatsClass();
        e.autoAttack = autoAttack;
        return e;
    }

    public GameController CreateGameController(Player p)
    {
        GameController g = new GameObject().AddComponent<GameController>();
        g.player = p;
        return g;
    }

    public InventoryUI CreateInventoryUI()
    {
        InventoryUI inventoryUI = new GameObject("InventoryUI").AddComponent<InventoryUI>();

        Button slot = new GameObject().AddComponent<Button>();
        Image slotImage = new GameObject().AddComponent<Image>();
        TextMeshProUGUI uses = new GameObject().AddComponent<TextMeshProUGUI>();

        slot.transform.SetParent(inventoryUI.transform);
        slotImage.transform.SetParent(slot.transform);
        uses.transform.SetParent(slot.transform);
        slotImage.transform.SetAsFirstSibling();

        inventoryUI.Slot1 = slot;

        return inventoryUI;
    }

    public Sprite CreateExampleSprite()
    {
        return Sprite.Create(new Texture2D(1, 1), new Rect(new Vector2(0, 0), new Vector2(1, 1)), new Vector2(1, 1));
    }
}
