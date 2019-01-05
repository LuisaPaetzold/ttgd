using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using TMPro;
using NSubstitute;

public class Test_PMInventoryUI
{
    [TearDown]
    public void TearDown()
    {
        Time.timeScale = 1;
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
            GameObject.Destroy(o);
        }
    }

    [UnityTest]
    public IEnumerator SlotImagesShowItemIcon()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        InventoryUI inventoryUI = CreateInventoryUI();
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForEndOfFrame();

        inventoryUI.GameCtr = gameCtr;
        Item item = ScriptableObject.CreateInstance<Item>();
        item.Icon = CreateExampleSprite();

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.IsNotNull(player.inventory, "Player did not have an inventory!");
        player.inventory.CollectItem(item);

        yield return new WaitForEndOfFrame();

        Image imageToTest = inventoryUI.GetImageOfSlot(inventoryUI.Slot1);

        yield return new WaitForEndOfFrame();

        Assert.AreEqual(item.GetIcon(), imageToTest.sprite, "Item icon was not displayed correctly!");
        Assert.IsNotNull(imageToTest.sprite, "Displayed icon was null even though there was an item!");
        Assert.IsTrue(imageToTest.enabled, "Item icon was not enabled even though there was an item!");
    }

    [UnityTest]
    public IEnumerator SlotTextShowsItemUsesLeft()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        InventoryUI inventoryUI = CreateInventoryUI();
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForEndOfFrame();

        inventoryUI.GameCtr = gameCtr;
        Item item = ScriptableObject.CreateInstance<Item>();
        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.IsNotNull(player.inventory, "Player did not have an inventory!");
        player.inventory.CollectItem(item);

        yield return new WaitForEndOfFrame();

        TextMeshProUGUI textToTest = inventoryUI.GetUsesTextOfSlot(inventoryUI.Slot1);

        yield return new WaitForEndOfFrame();

        Assert.AreEqual(item.GetUsesLeft().ToString(), textToTest.text, "Item uses were not displayed correctly!");
    }

    [UnityTest]
    public IEnumerator SlotImagesShowNothingWhenSlotIsEmpty()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        InventoryUI inventoryUI = CreateInventoryUI();
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForEndOfFrame();

        inventoryUI.GameCtr = gameCtr;
        Item item = ScriptableObject.CreateInstance<Item>();
        item.Icon = CreateExampleSprite();
        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.IsNotNull(player.inventory, "Player did not have an inventory!");

        yield return new WaitForEndOfFrame();

        Image imageToTest = inventoryUI.GetImageOfSlot(inventoryUI.Slot1);

        Assert.AreNotEqual(item.GetIcon(), imageToTest.sprite, "Item icon was displayed even though there was no item!");
        Assert.IsFalse(imageToTest.enabled, "Item icon was enabled even though there was no item!");
    }

    [UnityTest]
    public IEnumerator SlotTextShowsNothingWhenNoItem()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        InventoryUI inventoryUI = CreateInventoryUI();
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForEndOfFrame();

        inventoryUI.GameCtr = gameCtr;
        Item item = ScriptableObject.CreateInstance<Item>();
        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.IsNotNull(player.inventory, "Player did not have an inventory!");

        yield return new WaitForEndOfFrame();

        TextMeshProUGUI textToTest = inventoryUI.GetUsesTextOfSlot(inventoryUI.Slot1);

        Assert.AreNotEqual(item.GetUsesLeft().ToString(), textToTest.text, "Item uses were displayed even though there was no item!");
        Assert.AreEqual("", textToTest.text, "Item uses were not displayed as empty even though there was no item!");
    }

    [UnityTest]
    public IEnumerator InventoryUIOnlyEnabledDuringBattle()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        InventoryUI inventoryUI = CreateInventoryUI();
        GameController gameCtr = CreateGameController(player);
        inventoryUI.GameCtr = gameCtr;

        yield return new WaitForEndOfFrame();

        Assert.IsFalse(inventoryUI.gameObject.activeSelf, "Inventory UI was enabled on game start!");

        gameCtr.StartBattle(enemy);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(inventoryUI.gameObject.activeSelf, "Inventory UI was not enabled during battle!");

        enemy.stats.ReceiveDamage(enemy.stats.GetMaxHealth());

        yield return new WaitForEndOfFrame();

        Assert.IsFalse(inventoryUI.gameObject.activeSelf, "Inventory UI was not enabled after battle!");
    }

    [UnityTest]
    public IEnumerator SlotsAreOnlyInteractableIfPlayerCanAct()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        InventoryUI inventoryUI = CreateInventoryUI();
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForEndOfFrame();

        inventoryUI.GameCtr = gameCtr;
        Item item = ScriptableObject.CreateInstance<Item>();

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);
        player.inventory.CollectItem(item);

        yield return new WaitForEndOfFrame();

        Button slotButton = inventoryUI.transform.GetChild(0).gameObject.GetComponent<Button>();

        Assert.IsFalse(slotButton.IsInteractable(), "Slot button was interactable before the player waited their turn time!");

        yield return new WaitForSeconds(player.stats.TurnTime);

        Assert.IsTrue(slotButton.IsInteractable(), "Slot button wasn't interactable after the player waited their turn time!");

        player.inventory.UseItem(0);
        yield return new WaitForEndOfFrame(); yield return new WaitForEndOfFrame();

        Assert.IsFalse(slotButton.IsInteractable(), "Slot button wasn't reset to not interactable after the player attacked!");
    }

    [UnityTest]
    public IEnumerator SlotsAreOnlyInteractableIfContainsItem()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        InventoryUI inventoryUI = CreateInventoryUI();
        GameController gameCtr = CreateGameController(player);

        yield return new WaitForEndOfFrame();

        inventoryUI.GameCtr = gameCtr;
        Item item = ScriptableObject.CreateInstance<Item>();

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);

        yield return new WaitForSeconds(player.stats.TurnTime);
        yield return new WaitForEndOfFrame();

        Button slotButton = inventoryUI.transform.GetChild(0).gameObject.GetComponent<Button>();

        Assert.IsFalse(slotButton.IsInteractable(), "Slot button was interactable even though there was no item!");
        
        player.inventory.CollectItem(item);

        yield return new WaitForEndOfFrame();

        Assert.IsTrue(slotButton.IsInteractable(), "Slot button wasn't interactable even though there was an item!");

        player.inventory.RemoveItem(item);
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(slotButton.IsInteractable(), "Slot button wasn't reset to not interactable after the item was removed!");
    }

    [UnityTest]
    public IEnumerator AttackBoosterCannotBeUsedIfBoostAlreadyActive()
    {
        Player player = CreatePlayer();
        Enemy enemy = CreateEnemy(false);
        InventoryUI inventoryUI = CreateInventoryUI();
        GameController gameCtr = CreateGameController(player);
        IUnityStaticService staticService = CreateUnityService(player.stats.TurnTime, 0, 0);
        player.staticService = staticService;

        yield return new WaitForEndOfFrame();

        inventoryUI.GameCtr = gameCtr;
        Item item = ScriptableObject.CreateInstance<Item>();
        item.type = ItemType.AttackBoost;
        item.AddUses(1);

        yield return new WaitForEndOfFrame();

        gameCtr.StartBattle(enemy);
        player.inventory.CollectItem(item);
        
        yield return new WaitForEndOfFrame();

        Button slotButton = inventoryUI.transform.GetChild(0).gameObject.GetComponent<Button>();
        Assert.IsTrue(slotButton.IsInteractable(), "Slot button for boost item was not interactable even though there was no boost active!");

        player.inventory.UseItem(0);
        
        yield return new WaitForEndOfFrame();

        Assert.IsFalse(slotButton.IsInteractable(), "Slot button for boost item was interactable even though there was already a boost active!");
    }


    // --------------------- helper methods ----------------------------------------

    public Player CreatePlayer()
    {
        Player p = new GameObject().AddComponent<Player>();
        p.stats = new PlayerStatsClass();
        p.stats.TurnTime = 0.1f;
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

        Button slot = new GameObject("Slot").AddComponent<Button>();
        Image slotImage = new GameObject("Image").AddComponent<Image>();
        TextMeshProUGUI uses = new GameObject("Text").AddComponent<TextMeshProUGUI>();

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

    IUnityStaticService CreateUnityService(float deltaTimeReturn, float horizontalReturn, float verticalReturn)
    {
        IUnityStaticService s = Substitute.For<IUnityStaticService>();
        s.GetDeltaTime().Returns(deltaTimeReturn);
        s.GetInputAxis("Horizontal").Returns(horizontalReturn);
        s.GetInputAxis("Vertical").Returns(verticalReturn);

        return s;
    }
}
