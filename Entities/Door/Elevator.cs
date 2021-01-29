using Godot;

namespace Dystopia.Entities.Door
{
	public class Elevator : Node2D
	{
		[Export()] public Vector2 EndLocationRelative;

		[Export()] public float Speed = 0.1f;

		[Export()] public bool IsAtTheEnd = false;

		[Export(PropertyHint.File)] public AudioStreamSample MovementStartSound;

		[Export(PropertyHint.File)] public AudioStreamSample MovementEndSound;

		[Export(PropertyHint.File)] public AudioStreamSample MovementLoopSound;

		public bool Moving = false;

		protected Vector2 StartLocation;

		protected AudioStreamPlayer2D SoundPlayer;

		protected AudioStreamPlayer2D SoundLoopPlayer;

		public override void _Ready()
		{
			base._Ready();

			StartLocation = Position;

			SoundPlayer = new AudioStreamPlayer2D();
			AddChild(SoundPlayer);

			SoundLoopPlayer = new AudioStreamPlayer2D {Stream = MovementLoopSound};
			
			AddChild(SoundLoopPlayer);
		}

		public virtual void Move()
		{
			if(!Moving)
			{
				Moving = true;
				
				SoundPlayer.Stream = MovementStartSound;
				SoundPlayer.Play();

				SoundLoopPlayer.Play();
			}
		}

		protected virtual void OnMovementEnd()
		{
			Moving = false;
			IsAtTheEnd = !IsAtTheEnd;
			SoundPlayer.Stream = MovementEndSound;
			SoundPlayer.Play();

			SoundLoopPlayer.Stop();
		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);
			if (Moving)
			{
				if (!IsAtTheEnd)
				{
					Position = new Vector2
					(
						MathHelpers.Math.FInterpConstantTo(Position.x, StartLocation.x + EndLocationRelative.x, delta, Speed),
						MathHelpers.Math.FInterpConstantTo(Position.y, StartLocation.y + EndLocationRelative.y, delta, Speed)
					);
					if (Position.IsEqualApprox(StartLocation + EndLocationRelative))
					{
						OnMovementEnd();
					}
				}
				else
				{
					Position = new Vector2
					(
						MathHelpers.Math.FInterpConstantTo(Position.x, StartLocation.x, delta, Speed),
						MathHelpers.Math.FInterpConstantTo(Position.y, StartLocation.y, delta, Speed)
					);
					if (Position.IsEqualApprox(StartLocation))
					{
						OnMovementEnd();
					}
				}
			}
		}
	}
}
