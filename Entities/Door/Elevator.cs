using Godot;

namespace Dystopia.Entities.Door
{
	public class Elevator : Node2D
	{
		[Export()] public Vector2 EndLocationRelative;

		[Export()] public float Speed = 0.1f;

		[Export()] public bool IsAtTheEnd = false;

		public bool Moving = false;

		protected Vector2 StartLocation;

		public override void _Ready()
		{
			base._Ready();

			StartLocation = Position;
		}

		public virtual void Move()
		{
			if(!Moving)
			{
				Moving = true;
			}
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
						Moving = false;
						IsAtTheEnd = !IsAtTheEnd;
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
						Moving = false;
						IsAtTheEnd = !IsAtTheEnd;
					}
				}
			}
		}
	}
}
