using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingPrototyper : MonoBehaviour
{
    Dictionary<Material, HashSet<Shape>> allowedPots;

    /* DRAGGABLE */
    public ScoreController scoreController;
    public GameObject craftScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupAllowedPotsMap();
    }

    // EFF: maybe only add instead of re-init and repopulate? 
    public void SetupAllowedPotsMap()
    {
        int lvl = scoreController.lvl;

        allowedPots = new Dictionary<Material, HashSet<Shape>>();
        for (int i = 0; i <= lvl; i++)
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

    public void SelectPotTemplateButton(Button clickedButton)
    {
        // Move your selection square to the button's position
        // selectionSquare.transform.position = clickedButton.transform.position;
        Debug.Log("Pressed: " + clickedButton + clickedButton.transform.position);
        PotTemplateId id = clickedButton.GetComponent<PotTemplateId>();
        Debug.Log("Check: " + id.PTMaterial + ", " + id.PTShape);
    }

    public void SelectColorAccentButton(Button clickedButton)
    {
        // Move your selection square to the button's position
        // selectionSquare.transform.position = clickedButton.transform.position;
        Debug.Log("Pressed: " + clickedButton + clickedButton.transform.position);
        ColorAccentId id = clickedButton.GetComponent<ColorAccentId>();
        Debug.Log("Check: " + id.PTColor);
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
