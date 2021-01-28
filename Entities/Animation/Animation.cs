using Godot;

namespace Dystopia.Entities.Animation
{
	public class Animation : AnimatedSprite
	{
		[Signal]
		public delegate void OnMontageInterrupted(string montageName, int currentFrame);

		[Signal]
		public delegate void OnMontageFinished(string montageName);
		
		private Position2D _bulletSpawnPosition;

		public Position2D BulletSpawnPosition => _bulletSpawnPosition;

		protected bool IsPlayingMontage = false;

		public override void _Ready()
		{
			base._Ready();
			_bulletSpawnPosition = GetNode<Position2D>("BulletSpawnPosition");
		}

		public virtual bool SetAnimation(string animationName)
		{
			if (Frames.HasAnimation(animationName) && !IsPlayingMontage)
			{
				Animation = animationName;
				if (!Playing)
				{
					Play();
				}

				return true;
			}

			return false;
		}

		/* Plays montage
		 * Montage is same as usual animation,but can not be overriden by default animation process
		* Name montage is taken from Unreal's animation system
		*/
		public virtual void PlayMontage(string montageName)
		{
			if (IsPlayingMontage)
			{
				EmitSignal(nameof(OnMontageInterrupted), Animation, Frame);
			}

			IsPlayingMontage = true;

			if (Frames.HasAnimation(montageName))
			{
				Animation = montageName;
			}
		}

		private void _on_Animation_animation_finished()
		{
			//GD.Print("_on_Animation_animation_finished");
			if (IsPlayingMontage)
			{
				GD.Print("Ended Montage");
				IsPlayingMontage = false;

				EmitSignal(nameof(OnMontageFinished), Animation);
			}
		}
	}
}



