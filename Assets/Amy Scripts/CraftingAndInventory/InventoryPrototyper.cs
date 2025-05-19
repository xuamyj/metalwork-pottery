using System.Collections.Generic;
using UnityEngine;

public class InventoryPrototyper : MonoBehaviour
{
    // consts
    public const int NUM_INVENTORY_SLOTS = 24;

    /* DRAGGABLE */
    public List<GameObject> slots;

    // data
    public List<Pot> inventoryData; // TODO: I guess eventually this could contain other fun things like gemstones or special materials? but stick to pots for now
    public int approxNextEmptySlot;

    // TODO-inv: Later might also need draggable slots for images

    void Start()
    {
        inventoryData = new List<Pot>(NUM_INVENTORY_SLOTS);
        for (int i = 0; i < NUM_INVENTORY_SLOTS; i++)
        {
            inventoryData.Add(null);
        }
        approxNextEmptySlot = 0;
    }

    private void IncrementApproxNextEmptySlot(int i)
    {
        approxNextEmptySlot = (i + 1);
        if (approxNextEmptySlot >= NUM_INVENTORY_SLOTS)
        {
            approxNextEmptySlot = 0;
        }
    }

    public bool AddPotToInventory(Pot pot)
    {
        // look from approxNextEmptySlot to end
        for (int i = approxNextEmptySlot; i < NUM_INVENTORY_SLOTS; i++)
        {
            if (inventoryData[i] == null)
            {
                inventoryData[i] = pot;
                IncrementApproxNextEmptySlot(i);
                // TODO-inv: set image to pot
                return true;
            }
        }
        // if reached end, look from beginning to approxNextEmptySlot
        for (int i = 0; i < approxNextEmptySlot; i++)
        {
            if (inventoryData[i] == null)
            {
                inventoryData[i] = pot;
                IncrementApproxNextEmptySlot(i);
                // TODO-inv: set image to pot
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
            // TODO-inv: update images
        }
        else
        {
            inventoryData[endLoc] = inventoryData[startLoc];
            inventoryData[startLoc] = null;
            // TODO-inv: update images
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
        // TODO-inv: set image to blank
        inventoryData[loc] = null;

        // Optimization: Update approxNextEmptySlot if this slot is "earlier" in the inventory
        // This reduces future search time when adding items
        if (IsEarlierSlot(loc, approxNextEmptySlot))
        {
            approxNextEmptySlot = loc;
        }

        return true;
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

    // Update is called once per frame
    void Update()
    {

    }
}
