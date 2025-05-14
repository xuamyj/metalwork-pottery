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
    Red,
    Orange,
    Yellow,
    Green,
    Blue,
    Purple,
    Pink,
    Brown,
    Black,
    Gray,
    White,
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
} // better than tuple because the contents are named 

public static class LevelPotsList
{
    // this runs as soon as you start the game
    public static List<HashSet<PotTemplate>> list = LevelPotsList.getList();

    public static List<HashSet<PotTemplate>> getList()
    {
        // TODO: Later have this read from a file, and use De's "if list is defined, return list" structure

        list = new List<HashSet<PotTemplate>>{
            new HashSet<PotTemplate>{}, // empty for 0
            new HashSet<PotTemplate>{new PotTemplate(Material.Terracotta, Shape.Flowerpot)}, // 1
            new HashSet<PotTemplate>{new PotTemplate(Material.Terracotta, Shape.Bowl), new PotTemplate(Material.Terracotta, Shape.Plate)}, // 2
            new HashSet<PotTemplate>{new PotTemplate(Material.Stoneware, Shape.Mug)}, // 3
            new HashSet<PotTemplate>{new PotTemplate(Material.Stoneware, Shape.Bowl), new PotTemplate(Material.Stoneware, Shape.Plate)}, // 4
            new HashSet<PotTemplate>{new PotTemplate(Material.Stoneware, Shape.Teapot)}, // 5
            new HashSet<PotTemplate>{new PotTemplate(Material.Kaolin, Shape.Bowl), new PotTemplate(Material.Kaolin, Shape.Plate)}, // 6
            new HashSet<PotTemplate>{new PotTemplate(Material.Kaolin, Shape.Teacup)}, // 7
            new HashSet<PotTemplate>{new PotTemplate(Material.Terracotta, Shape.RomanTransportAmphora)}, // 8
            new HashSet<PotTemplate>{}, // 9, haven't decided
        };
        return list;
    }
}

// then make a CraftingController + InventoryController. CraftingController determines what combos are allowed based on lvl + LevelPotsList, makes pots, puts them in InventoryController. InventoryController allows you to inspect pots (and later move, delete, sell, etc)
public class Pot
{
    public Material material;
    public Shape shape;
    public ColorAccent colorAccent;
    public string visibleName;
    public string img;

    public Pot(Material m, Shape sh, ColorAccent ca) // later add img
    {
        this.material = m;
        this.shape = sh;
        this.colorAccent = ca;
        this.visibleName = this.colorAccent.ToString() + this.material.ToString() + this.shape.ToString();
        this.img = ""; // TODO: Add img and make it work
    }
}
