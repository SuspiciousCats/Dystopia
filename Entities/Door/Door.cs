using Godot;
using Godot.Collections;

namespace Dystopia.Entities.Door
{
	public class Door : Sprite
	{
		[Export()] public bool IsOpen = false;

		[Export()] public float DoorOpenHeight = 65;

		[Export(PropertyHint.File)] public AudioStreamSample OpenSound;

		[Export(PropertyHint.File)] public AudioStreamSample CloseSound;
		
		[Signal]
		public delegate void OnDoorOpened();

		[Signal]
		public delegate void OnDoorClosed();

		protected AudioStreamPlayer2D SoundPlayer;

		public override void _Ready()
		{
			base._Ready();
			if (IsOpen)
			{
				Position = new Vector2(Position.x, Position.y - DoorOpenHeight);
			}

			SoundPlayer = new AudioStreamPlayer2D();
			AddChild(SoundPlayer);
		}

		public virtual void Toggle()
		{
			IsOpen = !IsOpen;
			if (IsOpen)
			{
				Position = new Vector2(Position.x, Position.y - DoorOpenHeight);
				EmitSignal(nameof(OnDoorOpened));
				if (SoundPlayer != null)
				{
					SoundPlayer.Stream = OpenSound;
					SoundPlayer.Play();
				}
			}
			else
			{
				Position = new Vector2(Position.x, Position.y + DoorOpenHeight);
				EmitSignal(nameof(OnDoorClosed));
				if (SoundPlayer != null)
				{
					SoundPlayer.Stream = CloseSound;
					SoundPlayer.Play();
				}
			}
		}
	}
}
