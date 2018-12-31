using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public Button Slot1;
    public Button Slot2;
    public Button Slot3;

    public GameController GameCtr;

    private List<Button> slots;
    private bool firstFrame = true;

    void Start ()
    {
		if (GameCtr == null)
        {
            GameCtr = FindObjectOfType<GameController>();
        }

        slots = new List<Button>();
        slots.Add(Slot1);
        slots.Add(Slot2);
        slots.Add(Slot3);

    }

	void Update ()
    {
        if (firstFrame)
        {
            UpdateInventoryUI();
            firstFrame = false;
        }

        UpdateSlotInteractability();
    }

    public void UpdateInventoryUI()
    {
        int index = 0;
        if (slots == null)
        {
            return;
        }

        foreach (Button slot in slots)
        {
            PlayerInventoryClass inventory = GameCtr.player.GetPlayerInventory();
            if (slot != null 
                && inventory != null)  // todo: what does MaxItemCounts control?
            {
                if (slot.transform.GetChild(0) != null)
                {
                    Image img = GetImageOfSlot(slot);
                    if (img != null)
                    {
                        if (index < inventory.items.Count)
                        {
                            Item item = inventory.items[index];
                            if (item != null)
                            {
                                img.sprite = item.GetIcon();
                                img.enabled = true;
                            }
                            else
                            {
                                img.enabled = false;
                            }
                        }
                        else
                        {
                            img.enabled = false;
                        }
                    }
                }

                if (slot.transform.GetChild(1) != null)
                {
                    TextMeshProUGUI uses = GetUsesTextOfSlot(slot);
                    if (uses != null)
                    {
                        if (index < inventory.items.Count)
                        {
                            Item item = inventory.items[index];
                            if (item != null)
                            {
                                uses.text = item.GetUsesLeft().ToString();
                            }
                            else
                            {
                                uses.text = "";
                            }
                        }
                        else
                        {
                            uses.text = "";
                        }
                    }
                }
            }
            index++;
        }
    }

    public void UpdateSlotInteractability()
    {
        int index = 0;
        if (slots == null)
        {
            return;
        }

        foreach (Button slot in slots)
        {
            PlayerInventoryClass inventory = GameCtr.player.GetPlayerInventory();
            if (slot != null
                && inventory != null
                && GameCtr != null)
            {
                if (GameCtr.player.stats.CanAct()
                    && index < inventory.items.Count)
                {
                    slot.interactable = true;
                }
                else
                {
                    slot.interactable = false;
                }
            }
            index++;
        }
    }


    public void InventoryUIButtonClick(int index)
    {
        PlayerInventoryClass inventory = GameCtr.player.GetPlayerInventory();
        if (index < inventory.items.Count)
        {
            inventory.UseItem(index);
        }
        UpdateInventoryUI();
    }

    public Image GetImageOfSlot(Button slot)
    {
        return slot.transform.GetChild(0).GetComponent<Image>();
    }
    public TextMeshProUGUI GetUsesTextOfSlot(Button slot)
    {
        Transform child = slot.transform.GetChild(1);
        TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
        return text;
    }
}

