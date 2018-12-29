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
	}

    public void UpdateInventoryUI()
    {
        int index = 0;
        foreach (Button slot in slots)
        {
            PlayerInventoryClass inventory = GameCtr.player.GetPlayerInventory();
            if (slot != null 
                && inventory != null)  // todo: what does MaxItemCounts control?
            {
                if (slot.transform.GetChild(0) != null)
                {
                    Image img = slot.transform.GetChild(0).GetComponent<Image>();
                    if (img != null)
                    {
                        if (index < inventory.items.Count)
                        {
                            Item item = inventory.items[index];
                            img.sprite = item.GetIcon();
                            img.enabled = true;
                        }
                        else
                        {
                            img.sprite = null;
                            img.enabled = false;
                        }
                    }
                }

                if (slot.transform.GetChild(1) != null)
                {
                    TextMeshProUGUI uses = slot.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    if (uses != null)
                    {
                        if (index < inventory.items.Count)
                        {
                            Item item = inventory.items[index];
                            uses.text = item.GetUsesLeft().ToString();
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


    public void InventoryUIButtonClick(int index)
    {
        PlayerInventoryClass inventory = GameCtr.player.GetPlayerInventory();
        if (index < inventory.items.Count)
        {
            inventory.UseItem(index);
        }
        UpdateInventoryUI();
    }
}
