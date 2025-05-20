using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public enum Material
{
    Terracotta,
    Stoneware,
    Kaolin,
}

public enum Shape
{
    Flowerpot,
    Bowl,
    Plate,
    Mug,
    Teapot,
    Teacup,
    RomanTransportAmphora,
}

public enum ColorAccent
{
    Plain,
    Blue,
    Green,
    Yellow,
    Orange,
    Red,
    Pink,
    Purple,
    Gray,
    Black,
}

public class PotTemplate
{
    public Material material;
    public Shape shape;

    public PotTemplate(Material material, Shape shape)
    {
        this.material = material;
        this.shape = shape;
    }

    // Override the ToString method for better string representation
    public override string ToString()
    {
        return "{" + material + ", " + shape + "}";
    }

    // Override Equals to compare by material and shape rather than reference
    public override bool Equals(object obj)
    {
        // If the passed object is null or not a PotTemplate, return false
        if (obj == null || !(obj is PotTemplate))
            return false;

        // Cast the object to PotTemplate
        PotTemplate other = (PotTemplate)obj;

        // Compare the properties
        return material.Equals(other.material) && shape.Equals(other.shape);
    }

    // Override GetHashCode to ensure objects with the same properties have the same hash code
    public override int GetHashCode()
    {
        // Combine the hash codes of the properties
        // This is a simple implementation that works for most cases
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17; // Prime number to start with
            hash = hash * 23 + (material != null ? material.GetHashCode() : 0);
            hash = hash * 23 + (shape != null ? shape.GetHashCode() : 0);
            return hash;
        }
    }
} // better than tuple because the contents are named 

public static class LevelPotsList
{
    // this runs as soon as you start the game
    public static List<HashSet<PotTemplate>> list = LevelPotsList.getList();
    public static Dictionary<PotTemplate, string> potTemplateToImgPath = LevelPotsList.setupTemplateToImage();

    public static List<HashSet<PotTemplate>> getList()
    {
        // TODO: Later have this read from a file, and use De's "if list is defined, return list" structure

        list = new List<HashSet<PotTemplate>>{
            new HashSet<PotTemplate>{}, // empty for 0
            new HashSet<PotTemplate>{new PotTemplate(Material.Terracotta, Shape.Flowerpot)}, // 1
            new HashSet<PotTemplate>{new PotTemplate(Material.Terracotta, Shape.Plate), new PotTemplate(Material.Terracotta, Shape.Bowl)}, // 2
            new HashSet<PotTemplate>{new PotTemplate(Material.Stoneware, Shape.Mug)}, // 3
            new HashSet<PotTemplate>{new PotTemplate(Material.Stoneware, Shape.Plate), new PotTemplate(Material.Stoneware, Shape.Bowl)}, // 4
            new HashSet<PotTemplate>{new PotTemplate(Material.Stoneware, Shape.Teapot)}, // 5
            new HashSet<PotTemplate>{new PotTemplate(Material.Kaolin, Shape.Teacup)}, // 6
            new HashSet<PotTemplate>{new PotTemplate(Material.Kaolin, Shape.Plate), new PotTemplate(Material.Kaolin, Shape.Bowl)}, // 7
            new HashSet<PotTemplate>{new PotTemplate(Material.Terracotta, Shape.RomanTransportAmphora)}, // 8
            new HashSet<PotTemplate>{}, // 9, haven't decided
        };
        return list;
    }

    public static Dictionary<PotTemplate, string> setupTemplateToImage()
    {
        potTemplateToImgPath = new Dictionary<PotTemplate, string>{
            { new PotTemplate(Material.Terracotta, Shape.Flowerpot), "Pots/terracotta-flowerpot" },
            { new PotTemplate(Material.Terracotta, Shape.Plate), "Pots/terracotta-plate" },
            { new PotTemplate(Material.Terracotta, Shape.Bowl), "Pots/terracotta-bowl" },
            { new PotTemplate(Material.Stoneware, Shape.Mug), "Pots/stoneware-mug" },
            { new PotTemplate(Material.Stoneware, Shape.Plate), "Pots/stoneware-plate" },
            { new PotTemplate(Material.Stoneware, Shape.Bowl), "Pots/stoneware-bowl" },
            { new PotTemplate(Material.Stoneware, Shape.Teapot), "Pots/stoneware-teapot" },
            { new PotTemplate(Material.Kaolin, Shape.Teacup), "Pots/kaolin-teacup-2" },
            { new PotTemplate(Material.Kaolin, Shape.Plate), "Pots/kaolin-plate" },
            { new PotTemplate(Material.Kaolin, Shape.Bowl), "Pots/kaolin-bowl" },
            { new PotTemplate(Material.Terracotta, Shape.RomanTransportAmphora), "Pots/terracotta-romanamphora-red" },
        };
        return potTemplateToImgPath;
    }
}

// then make a CraftingController + InventoryController. CraftingController determines what combos are allowed based on lvl + LevelPotsList, makes pots, puts them in InventoryController. InventoryController allows you to inspect pots (and later move, delete, sell, etc)
public class Pot
{
    public Material material;
    public Shape shape;
    public ColorAccent colorAccent;
    public string visibleName;
    public string imgPath;

    public Pot(Material m, Shape sh, ColorAccent ca, string imgPath)
    {
        this.material = m;
        this.shape = sh;
        this.colorAccent = ca;
        this.visibleName = this.colorAccent.ToString() + this.material.ToString() + this.shape.ToString();
        this.imgPath = imgPath;
    }
}
