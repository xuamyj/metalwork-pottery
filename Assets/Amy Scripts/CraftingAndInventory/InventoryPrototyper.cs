using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPrototyper : MonoBehaviour
{
    // consts
    public const int NUM_INVENTORY_SLOTS = 24;
    public const string BLANK_IMG_PATH = "blank";

    /* DRAGGABLE */
    public List<GameObject> slots;
    public GameObject iSelectSquare;
    public TextMeshProUGUI selectVisibleNameText;

    // data
    public List<Pot> inventoryData; // TODO: I guess eventually this could contain other fun things like gemstones or special materials? but stick to pots for now
    public int approxNextEmptySlot;
    // private data
    private int iSelectedIndex;

    void Start()
    {
        inventoryData = new List<Pot>(NUM_INVENTORY_SLOTS);
        for (int i = 0; i < NUM_INVENTORY_SLOTS; i++)
        {
            inventoryData.Add(null);
        }
        approxNextEmptySlot = 0;

        iSelectedIndex = -1;
    }

    public bool AddPotToInventory(Pot pot)
    {
        // look from approxNextEmptySlot to end
        for (int i = approxNextEmptySlot; i < NUM_INVENTORY_SLOTS; i++)
        {
            if (inventoryData[i] == null)
            {
                inventoryData[i] = pot;
                SetSlotImage(i, pot.imgPath);

                IncrementApproxNextEmptySlot(i);
                return true;
            }
        }
        // if reached end, look from beginning to approxNextEmptySlot
        for (int i = 0; i < approxNextEmptySlot; i++)
        {
            if (inventoryData[i] == null)
            {
                inventoryData[i] = pot;
                SetSlotImage(i, pot.imgPath);

                IncrementApproxNextEmptySlot(i);
                return true;
            }
        }
        // if reached here, inventory is full! 
        Debug.Log("InventoryPrototyper problem: Inventory is full!");
        return false;
    }

    public bool SwapPotLocInInventory(int startLoc, int endLoc)
    {
        // Check indices are in valid range
        if (startLoc < 0 || startLoc >= NUM_INVENTORY_SLOTS ||
            endLoc < 0 || endLoc >= NUM_INVENTORY_SLOTS)
        {
            Debug.LogWarning("InventoryPrototyper: Invalid inventory indices for swap operation");
            return false;
        }

        if (inventoryData[startLoc] == null)
        {
            return false;
        }
        // pot in startLoc, check endLoc
        if (inventoryData[endLoc] != null)
        {
            Pot temp = inventoryData[endLoc];
            inventoryData[endLoc] = inventoryData[startLoc];
            inventoryData[startLoc] = temp;

            SwapSlotImages(startLoc, endLoc);
        }
        else
        {
            inventoryData[endLoc] = inventoryData[startLoc];
            inventoryData[startLoc] = null;

            SwapSlotImages(startLoc, endLoc);
        }
        return true;
    }

    public bool RemovePotFromInventoryLoc(int loc)
    {
        // Check indices are in valid range
        if (loc < 0 || loc >= NUM_INVENTORY_SLOTS)
        {
            Debug.LogWarning("InventoryPrototyper: Invalid inventory index for remove operation");
            return false;
        }

        if (inventoryData[loc] == null)
        {
            return false;
        }
        inventoryData[loc] = null;
        SetSlotImage(loc, BLANK_IMG_PATH);

        // Optimization: Update approxNextEmptySlot if this slot is "earlier" in the inventory
        // This reduces future search time when adding items
        if (IsEarlierSlot(loc, approxNextEmptySlot))
        {
            approxNextEmptySlot = loc;
        }

        return true;
    }

    // ========

    private void IncrementApproxNextEmptySlot(int i)
    {
        approxNextEmptySlot = (i + 1);
        if (approxNextEmptySlot >= NUM_INVENTORY_SLOTS)
        {
            approxNextEmptySlot = 0;
        }
    }

    private bool IsEarlierSlot(int slot1, int slot2)
    {
        // Handle the wraparound case
        if (slot2 < approxNextEmptySlot && slot1 >= approxNextEmptySlot)
            return false;  // slot1 is actually "later"
        if (slot1 < approxNextEmptySlot && slot2 >= approxNextEmptySlot)
            return true;   // slot1 is "earlier"

        // Normal case
        return slot1 < slot2;
    }

    public bool SetSlotImage(int slotIndex, string imagePath)
    {
        // Make sure the slot index is valid
        if (slotIndex < 0 || slotIndex >= slots.Count)
        {
            Debug.LogError("Slot index out of range");
            return false;
        }

        // Load the sprite from Resources folder
        // Note: Don't include the file extension (.png) in the path
        Sprite newSprite = Resources.Load<Sprite>(imagePath);
        if (newSprite == null)
        {
            Debug.LogError("Failed to load sprite at path: " + imagePath);
            return false;
        }

        Image slotImage = slots[slotIndex].GetComponent<Image>();
        slotImage.sprite = newSprite;
        return true;
    }

    public bool SwapSlotImages(int startLoc, int endLoc)
    {
        // Make sure both indices are valid
        if (startLoc < 0 || startLoc >= slots.Count || endLoc < 0 || endLoc >= slots.Count)
        {
            Debug.LogError("Slot index out of range");
            return false;
        }

        // Get Image components from both slots
        Image startImage = slots[startLoc].GetComponent<Image>();
        Image endImage = slots[endLoc].GetComponent<Image>();

        Sprite tempSprite = startImage.sprite;
        startImage.sprite = endImage.sprite;
        endImage.sprite = tempSprite;
        return true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
