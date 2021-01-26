using Godot;
using Godot.Collections;

namespace Dystopia.Entities.Weapons
{
	public class WeaponBase : AnimatedSprite
	{
		public class Ammo
		{
			public WeaponType Type = WeaponType.Pistol;

			public int Amount = 0;
		}
		
		public enum WeaponType
		{
			None,
			Pistol
		}
		
		[Export(PropertyHint.File)] public Array<AudioStreamSample> FireSounds;

		[Export(PropertyHint.File)] public AudioStreamSample EmptyMagazineSound;

		[Export(PropertyHint.File)] public string BulletPath;

		[Export()] public float CooldownTime = 1;

		[Export()] public float Damage = 20;

		[Export()] public int AmmoPerClip = 15;

		[Export(PropertyHint.Enum)] public WeaponType Type;

		protected int CurrentAmmoInTheClip = 5;

		public Character OwningCharacter;

		protected Timer CooldownTimer;

		protected AudioStreamPlayer2D FireSoundPlayer;

		private bool _isCoolingDown = false;

		private RandomNumberGenerator _randomNumberGenerator = new RandomNumberGenerator();

		private PackedScene _bulletScene;
		
		public override void _Ready()
		{
			base._Ready();
			FireSoundPlayer = GetNode<AudioStreamPlayer2D>("FireSound");
			_bulletScene = GD.Load<PackedScene>(BulletPath);
		}

		public virtual bool CanShoot()
		{
			return !_isCoolingDown && _bulletScene != null && OwningCharacter != null &&
				   (CooldownTimer == null || CooldownTimer.IsStopped()) && CurrentAmmoInTheClip > 0;
		}
		
		public virtual void Shoot(Vector2 location,float rotation,bool isGoingLeft)
		{
			if (CanShoot())
			{
				_isCoolingDown = true;
				
				if (FireSounds.Count > 0)
				{
					_randomNumberGenerator.Randomize();

					FireSoundPlayer.Stream = FireSounds[_randomNumberGenerator.RandiRange(0, FireSounds.Count - 1)];

					FireSoundPlayer.Play();
				}

				
				CooldownTimer = new Timer();
				CooldownTimer.Connect("timeout", this, "ResetCooldownTimer");
				CooldownTimer.WaitTime = CooldownTime;
				AddChild(CooldownTimer);
				CooldownTimer.Start();

				if (_bulletScene.Instance() is Bullet.Bullet bullet)
				{
					bullet.OwningWeapon = this;

					bullet.IsGoingLeft = isGoingLeft;
					
					bullet.OwningCharacter = OwningCharacter;
					
					bullet.SpawnPosition = location;

					//bullet.SpawnRotation = rotation;

					OwningCharacter.GetParent().AddChild(bullet);
				}

				CurrentAmmoInTheClip--;
				
				if (CurrentAmmoInTheClip < 0)
				{
					CurrentAmmoInTheClip = 0;
				}
			}
			else if (CurrentAmmoInTheClip == 0)
			{
				FireSoundPlayer.Stream = EmptyMagazineSound;
				FireSoundPlayer.Play();
			}
		}

		public virtual void Reload()
		{
			CurrentAmmoInTheClip = AmmoPerClip;
		}
		
		protected virtual void ResetCooldownTimer()
		{
			_isCoolingDown = false;
			CooldownTimer.Stop();
		}
	}
}
