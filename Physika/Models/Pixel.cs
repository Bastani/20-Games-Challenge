using System;
using Godot;

namespace Physika.Models;

public class Pixel
{
    public enum Material
    {
        Sand,
        Water
    }
    
    public Color Color { get; set; }
    public Material Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public Pixel(Material material)
    {
        Type = material;
        switch (Type)
        {
            case Material.Sand:
                Color = Colors.Yellow;
                break;
            case Material.Water:
                Color = Colors.Aquamarine;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(material), material, null);
        }
    }
}