using Godot;
using Godot.Collections;

namespace Dystopia.Entities.Weapons
{
	public class WeaponBase : AnimatedSprite
	{
		[Export(PropertyHint.File)] public Array<AudioStreamSample> FireSounds;

		[Export()] public float CooldownTime = 1;

		protected Timer CooldownTimer;

		protected AudioStreamPlayer2D FireSoundPlayer;

		private bool _isCoolingDown = false;

		private RandomNumberGenerator _randomNumberGenerator = new RandomNumberGenerator();
		
		public override void _Ready()
		{
			base._Ready();
			FireSoundPlayer = GetNode<AudioStreamPlayer2D>("FireSound");
		}

		public virtual bool CanShoot()
		{
			return !_isCoolingDown;
		}
		
		public virtual void Shoot()
		{
			if (CanShoot())
			{
				if (FireSounds.Count > 0)
				{
					_randomNumberGenerator.Randomize();

					FireSoundPlayer.Stream = FireSounds[_randomNumberGenerator.RandiRange(0, FireSounds.Count - 1)];

					FireSoundPlayer.Play();
				}

				_isCoolingDown = true;
				CooldownTimer = new Timer();
				CooldownTimer.Connect("timeout", this, "ResetCooldownTimer");
				CooldownTimer.WaitTime = CooldownTime;
				AddChild(CooldownTimer);
				CooldownTimer.Start();
			}
		}

		protected virtual void ResetCooldownTimer()
		{
			_isCoolingDown = false;
		}
	}
}
