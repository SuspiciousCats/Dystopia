using Godot;

namespace Dystopia.Entities.Door
{
	public class Door : Sprite
	{
		[Export()] public bool IsOpen = false;

		[Export()] public float DoorOpenHeight = 65;
		
		[Signal]
		public delegate void OnDoorOpened();

		[Signal]
		public delegate void OnDoorClosed();

		public override void _Ready()
		{
			base._Ready();
			if (IsOpen)
			{
				Position = new Vector2(Position.x, Position.y - DoorOpenHeight);
			}
		}

		public virtual void Toggle()
		{
			IsOpen = !IsOpen;
			if (IsOpen)
			{
				Position = new Vector2(Position.x, Position.y - DoorOpenHeight);
				EmitSignal(nameof(OnDoorOpened));
			}
			else
			{
				Position = new Vector2(Position.x, Position.y + DoorOpenHeight);
				EmitSignal(nameof(OnDoorClosed));
			}
		}
	}
}
