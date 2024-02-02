using Godot;
using System;
using System.Diagnostics;

public partial class Player : CharacterBody3D
{
	[Export]
	public int Speed { get; set; } = 14;
	// The downward acceleration when in the air, in meters per second squared.
	[Export]
	public int FallAcceleration { get; set; } = 75;

	private Vector3 _targetVelocity = Vector3.Zero;

	[Signal]
	public delegate void HitEventHandler();
	
	// Vertical impulse applied to the character upon jumping in meters per second.
	[Export]
	public int JumpImpulse { get; set; } = 20;
	
	[Export]
	public int BounceImpulse { get; set; } = 16;
	
	public override void _PhysicsProcess(double delta)
	{
		// We create a local variable to store the input direction.
		var direction = Vector3.Zero;

		// We check for each move input and update the direction accordingly.
		if (Input.IsActionPressed("move_right"))
		{
			direction.X += 1.0f;
		}
		if (Input.IsActionPressed("move_left"))
		{
			direction.X -= 1.0f;
		}
		if (Input.IsActionPressed("move_back"))
		{
			// Notice how we are working with the vector's X and Z axes.
			// In 3D, the XZ plane is the ground plane.
			direction.Z += 1.0f;
		}
		if (Input.IsActionPressed("move_forward"))
		{
			direction.Z -= 1.0f;
		}
		
		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();
			GetNode<Node3D>("Pivot").LookAt(Position + direction, Vector3.Up);
			GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 4;
		}
		else
		{
			GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 1;
		}
		
		// Ground velocity
		_targetVelocity.X = direction.X * Speed;
		_targetVelocity.Z = direction.Z * Speed;

		// Vertical velocity
		if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		}

		// Moving the character
		Velocity = _targetVelocity;
		MoveAndSlide();
		
		// Jumping.
		if (IsOnFloor() && Input.IsActionJustPressed("jump"))
		{
			_targetVelocity.Y = JumpImpulse;
		}
		
		// Iterate through all collisions that occurred this frame.
		for (int index = 0; index < GetSlideCollisionCount(); index++)
		{
			// We get one of the collisions with the player.
			KinematicCollision3D collision = GetSlideCollision(index);
		
			// If the collision is with a mob.
			// With C# we leverage typing and pattern-matching
			// instead of checking for the group we created.
			if (collision.GetCollider() is Mob mob)
			{
				// We check that we are hitting it from above.
				if (Vector3.Up.Dot(collision.GetNormal()) > 0.1f)
				{
					// If so, we squash it and bounce.
					mob.Squash();
					_targetVelocity.Y = BounceImpulse;
					// Prevent further duplicate calls.
					break;
				}
			}
		}
		
		var pivot = GetNode<Node3D>("Pivot");
		pivot.Rotation = new Vector3(Mathf.Pi / 6.0f * Velocity.Y / JumpImpulse, pivot.Rotation.Y, pivot.Rotation.Z);
	}

	private void Die()
	{
		EmitSignal(SignalName.Hit);
		QueueFree();
	}

	private void OnMobDetectorBodyEntered(Node3D body)
	{
		Die();
	}
}
