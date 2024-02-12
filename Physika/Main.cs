using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using Physika.Models;

public partial class Main : Node2D
{
	public Pixel[,] PixelList { get; set; }
	public PackedScene PixelScene { get; set; }

	public int Height { get; set; }
	public int Width { get; set; }
	
	public Vector2 ViewportArea { get; set; }

	public Pixel.Material SelectedMaterial = Pixel.Material.Sand;
	public ImageTexture Tex { get; set; }
	public Image Img { get; set; }
	
	public Main()
	{
		PixelScene = GD.Load<PackedScene>("./Pixel.tscn");
	}


	public override void _Draw()
	{
		Tex.Update(Img);
		DrawTexture(Tex, Vector2.Zero);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var screenSize = GetViewport().GetVisibleRect().Size;
		ViewportArea = screenSize - Vector2.One;
		Width = (int)screenSize.X;
		Height = (int)screenSize.Y;
		PixelList = new Pixel[Width, Height];
		Img = Image.Create(Width, Height, false, Image.Format.Rgba8);
		Tex = ImageTexture.CreateFromImage(Img);
	}


	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.Keycode == Key.Key1)
			{
				SelectedMaterial = Pixel.Material.Sand;
			}
			else if (keyEvent.Keycode == Key.Key2)
			{
				SelectedMaterial = Pixel.Material.Water;
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		if (Input.IsMouseButtonPressed(MouseButton.Left))
		{
			var coordinates = GetViewport().GetMousePosition().Clamp(Vector2.Zero, ViewportArea);
			var x = (int)coordinates.X;
			var y = (int)coordinates.Y;
			var pixel = PixelList[x,y];
			if (pixel != null)
			{
				//pixel.QueueFree();
			}
			else
			{
				pixel = new Pixel(SelectedMaterial)
				{
					X = x,
					Y = y
				};
				PixelList[x,y] = pixel;
			}
		}
		
		for(var y = Height - 1; y >= 0; y--)
		{
			for (var x = 0; x < Width; x++)
			{
				if(processPixel(x,y))
					QueueRedraw();
			}
		}
	}

	private bool processPixel(int x, int y)
	{
		var pixel = PixelList[x, y];
		if (pixel is null) 
			return false;
		int yMove;
		int xMove;
		switch (pixel.Type)
		{
			case Pixel.Material.Sand:
				yMove = y + 1;
				if (yMove < Height)
				{
					if (PixelList[x, yMove] is null)
					{
						xMove = x;
					}
					else if (x - 1 >= 0 && PixelList[x - 1, yMove] is null)
					{
						xMove = x - 1;
					}
					else if (x + 1 < Width && PixelList[x + 1, yMove] is null)
					{
						xMove = x + 1;
					}
					else
						return false;

					pixel.X = xMove;
					pixel.Y = yMove;
					PixelList[xMove, yMove] = pixel;
					PixelList[x, y] = null;
					Img.SetPixel(xMove, yMove, pixel.Color);
					Img.SetPixel(x, y, Colors.Transparent);
				}
				break;
			case Pixel.Material.Water:
				if (y + 1 < Height && PixelList[x, y + 1] is null)
				{
					xMove = x;
					yMove = y + 1;
				}
				else if (y + 1 < Height && x - 1 >= 0 && PixelList[x - 1, y + 1] is null)
				{
					xMove = x - 1;
					yMove = y + 1;
				}
				else if (y + 1 < Height && x + 1 < Width && PixelList[x + 1, y + 1] is null)
				{
					xMove = x + 1;
					yMove = y + 1;
				}
				else if (x + 1 < Width && PixelList[x + 1, y] is null)
				{
					xMove = x + 1;
					yMove = y;
				}
				else if (x - 1 >= 0 && PixelList[x - 1, y] is null)
				{
					xMove = x - 1;
					yMove = y;
				}
				else
					return false;
				pixel.X = xMove;
				pixel.Y = yMove;
				PixelList[xMove, yMove] = pixel;
				PixelList[x, y] = null;
				Img.SetPixel(xMove, yMove, pixel.Color);
				Img.SetPixel(x, y, Colors.Transparent);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		return true;
	}
}
