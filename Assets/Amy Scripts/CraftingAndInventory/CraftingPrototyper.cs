using System.Collections.Generic;
using UnityEngine;

public class CraftingPrototyper : MonoBehaviour
{
    Dictionary<Material, HashSet<Shape>> allowedPots;

    /* DRAGGABLE */
    public ScoreController scoreController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
        foreach (Material material in allowedPots.Keys)
        {
            // Get the material name or some identifier
            string materialName = material.ToString();

            // Get the shapes for this material
            HashSet<Shape> shapes = allowedPots[material];

            // Format the shapes into a comma-separated string
            string shapesString = "";
            foreach (Shape s in shapes)
            {
                shapesString += s + ", ";
            }

            // Print the material and its associated shapes
            Debug.Log($"Material: {materialName} → Shapes: [{shapesString}]");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
