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

		
		public virtual void Move()
		{
			Moving = true;
			StartLocation = Position;
		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);
			if (Moving)
			{
				Position = new Vector2
				(
					MathHelpers.Math.FInterpTo(Position.x, StartLocation.x + EndLocationRelative.x, delta, Speed),
					MathHelpers.Math.FInterpTo(Position.y, StartLocation.y + EndLocationRelative.y, delta, Speed)
				);
				if (Position.IsEqualApprox(StartLocation + EndLocationRelative))
				{
					Moving = false;
					IsAtTheEnd = !IsAtTheEnd;
				}
			}
		}
	}
}
