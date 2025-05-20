using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingPrototyper : MonoBehaviour
{
    // consts
    public const float GRAY_ALPHA = 80f;

    // data
    Dictionary<Material, HashSet<Shape>> allowedPots;
    PotTemplate selectedTemplate;
    ColorAccent selectedColor;

    /* DRAGGABLE */
    public ScoreController scoreController;
    public GameObject craftScreen;
    public List<GameObject> potTemplateImageObjs;
    public GameObject potTemplateSelectSquare;
    public GameObject colorSelectSquare;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupAllowedPotsMap();
        ColorAllowedPotsUI();

        selectedTemplate = new PotTemplate(Material.Terracotta, Shape.Flowerpot);
        selectedColor = ColorAccent.Plain;
    }

    // EFF: maybe only add instead of re-init and repopulate? 
    public void SetupAllowedPotsMap()
    {
        int maxLvl = Mathf.Min(scoreController.lvl, LevelPotsList.list.Count - 1);

        allowedPots = new Dictionary<Material, HashSet<Shape>>();
        for (int i = 0; i <= maxLvl; i++)
        {
            HashSet<PotTemplate> potTemplates = LevelPotsList.list[i];
            foreach (PotTemplate template in potTemplates)
            {
                if (!allowedPots.ContainsKey(template.material))
                {
                    allowedPots[template.material] = new HashSet<Shape>();
                }
                allowedPots[template.material].Add(template.shape);
            }
        }
        Debug.Log("SetupAllowedPotsMap: " + allowedPots.ToDebugString());
    }

    public void ColorAllowedPotsUI()
    {
        foreach (GameObject obj in potTemplateImageObjs)
        {
            Button button = obj.GetComponent<Button>();
            Image image = obj.GetComponent<Image>();

            PotTemplateId potTemplateId = obj.GetComponent<PotTemplateId>();
            if (!allowedPots.ContainsKey(potTemplateId.PTMaterial) || !allowedPots[potTemplateId.PTMaterial].Contains(potTemplateId.PTShape)) // not allowed, gray and disable button
            {
                // Disable the Button component
                button.interactable = false;

                // Set the Image transparency to 80 (out of 255)
                Color color = image.color;
                color.a = GRAY_ALPHA / 255f; // Convert from 0-255 to 0-1 range
                image.color = color;
            }
            else
            {
                // Reverse above
                button.interactable = true;
                // Color too
                Color color = image.color;
                color.a = 1.0f;
                image.color = color;
            }
        }
    }

    public void SelectPotTemplateButton(Button clickedButton)
    {
        // Move your selection square to the button's position
        Debug.Log("Pressed: " + clickedButton + clickedButton.transform.position);
        PotTemplateId id = clickedButton.GetComponent<PotTemplateId>();
        Debug.Log("Check: " + id.PTMaterial + ", " + id.PTShape);

        potTemplateSelectSquare.transform.position = clickedButton.transform.position;
        selectedTemplate = new PotTemplate(id.PTMaterial, id.PTShape);
    }

    public void SelectColorAccentButton(Button clickedButton)
    {
        // Move your selection square to the button's position
        Debug.Log("Pressed: " + clickedButton + clickedButton.transform.position);
        ColorAccentId id = clickedButton.GetComponent<ColorAccentId>();
        Debug.Log("Check: " + id.PTColor);

        colorSelectSquare.transform.position = clickedButton.transform.position;
        selectedColor = id.PTColor;
    }

    public void OnClickStartCraft()
    {
        craftScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
