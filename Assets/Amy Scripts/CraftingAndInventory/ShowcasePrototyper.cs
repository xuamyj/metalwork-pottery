using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowcasePrototyper : MonoBehaviour
{
    // consts
    public const int NUM_SHOWCASE_SLOTS = 12;
    public const string BLANK_IMG_PATH = "blank";

    /* DRAGGABLE */
    public List<GameObject> bigSlots;

    // data
    public List<Pot> showcaseData;
    public int approxNextEmptySlot;

    void Start()
    {
        showcaseData = new List<Pot>(NUM_SHOWCASE_SLOTS);
        for (int i = 0; i < NUM_SHOWCASE_SLOTS; i++)
        {
            showcaseData.Add(null);
        }
        approxNextEmptySlot = 0;
    }

    private void IncrementApproxNextEmptySlot(int i)
    {
        approxNextEmptySlot = (i + 1);
        if (approxNextEmptySlot >= NUM_SHOWCASE_SLOTS)
        {
            approxNextEmptySlot = 0;
        }
    }

    public bool AddPotToShowcase(Pot pot)
    {
        // look from approxNextEmptySlot to end
        for (int i = approxNextEmptySlot; i < NUM_SHOWCASE_SLOTS; i++)
        {
            if (showcaseData[i] == null)
            {
                showcaseData[i] = pot;
                SetSlotImage(i, pot.imgPath);

                IncrementApproxNextEmptySlot(i);
                return true;
            }
        }
        // if reached end, look from beginning to approxNextEmptySlot
        for (int i = 0; i < approxNextEmptySlot; i++)
        {
            if (showcaseData[i] == null)
            {
                showcaseData[i] = pot;
                SetSlotImage(i, pot.imgPath);

                IncrementApproxNextEmptySlot(i);
                return true;
            }
        }
        // if reached here, showcase is full! 
        Debug.Log("ShowcasePrototyper problem: Showcase is full!");
        return false;
    }

    public bool SwapPotLocInShowcase(int startLoc, int endLoc)
    {
        // Check indices are in valid range
        if (startLoc < 0 || startLoc >= NUM_SHOWCASE_SLOTS ||
            endLoc < 0 || endLoc >= NUM_SHOWCASE_SLOTS)
        {
            Debug.LogWarning("ShowcasePrototyper: Invalid showcase indices for swap operation");
            return false;
        }

        if (showcaseData[startLoc] == null)
        {
            return false;
        }
        // pot in startLoc, check endLoc
        if (showcaseData[endLoc] != null)
        {
            Pot temp = showcaseData[endLoc];
            showcaseData[endLoc] = showcaseData[startLoc];
            showcaseData[startLoc] = temp;

            SwapSlotImages(startLoc, endLoc);
        }
        else
        {
            showcaseData[endLoc] = showcaseData[startLoc];
            showcaseData[startLoc] = null;

            SwapSlotImages(startLoc, endLoc);
        }
        return true;
    }

    public bool RemovePotFromShowcaseLoc(int loc)
    {
        // Check indices are in valid range
        if (loc < 0 || loc >= NUM_SHOWCASE_SLOTS)
        {
            Debug.LogWarning("ShowcasePrototyper: Invalid showcase index for remove operation");
            return false;
        }

        if (showcaseData[loc] == null)
        {
            return false;
        }
        showcaseData[loc] = null;
        SetSlotImage(loc, BLANK_IMG_PATH);

        // Optimization: Update approxNextEmptySlot if this slot is "earlier" in the showcase
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

    public bool SetSlotImage(int slotIndex, string imagePath)
    {
        // Make sure the slot index is valid
        if (slotIndex < 0 || slotIndex >= bigSlots.Count)
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

        Image slotImage = bigSlots[slotIndex].GetComponent<Image>();
        slotImage.sprite = newSprite;
        return true;
    }


    public bool SwapSlotImages(int startLoc, int endLoc)
    {
        // Make sure both indices are valid
        if (startLoc < 0 || startLoc >= bigSlots.Count || endLoc < 0 || endLoc >= bigSlots.Count)
        {
            Debug.LogError("Slot index out of range");
            return false;
        }

        // Get Image components from both bigSlots
        Image startImage = bigSlots[startLoc].GetComponent<Image>();
        Image endImage = bigSlots[endLoc].GetComponent<Image>();

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
