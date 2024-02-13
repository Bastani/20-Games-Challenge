using Godot;
using System;

public partial class Player : CharacterBody3D
{
	private Camera3D Camera { get; set; }
	private Node3D Head { get; set; }
	
	private float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	[Export] 
	private float JumpVelocity { get; set; } = 4.5f;
	
	[Export]
	private float Speed { get; set; } = 5.0f;
	
	[Export]
	private float Acceleration { get; set; } = 0.2f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Head = GetNode<Node3D>("Head");
		Camera = GetNode<Camera3D>("Head/Camera3D");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity.Y -= Gravity * (float)delta;
		}
		
		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}
		
		var inputDir = Input.GetVector("Move_Left", "Move_Right", "Move_Forward", "Move_Back");
		inputDir = inputDir.Normalized();
		
		var direction = (Head.GlobalTransform.Basis * new Vector3(inputDir.X , 0 , inputDir.Y)).Normalized();
		
		Console.WriteLine(direction);
		
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Acceleration);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Acceleration);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
		{
			Head.RotateY(-motion.Relative.X * 0.003f);
			Camera.RotateX(-motion.Relative.Y * 0.003f);

			var cameraRot = Camera.Rotation;

			cameraRot.X = Mathf.Clamp(cameraRot.X,Mathf.DegToRad(-80f),Mathf.DegToRad(80f));

			Camera.Rotation = cameraRot;
		}
	}
}
