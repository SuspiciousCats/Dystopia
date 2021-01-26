using Dystopia.Entities.Weapons;
using Godot;
using Godot.Collections;

namespace Dystopia.Entities
{
	public class Character : KinematicBody2D
	{
		public enum Team
		{
			Player,
			Solders
		}
		
		[Export] public bool IsControlledByPlayer = true;
		
		[Export] public float Speed = 100;

		[Export] public float GravityForce = 980;

		[Export] public float JumpForce = -450;

		[Export(PropertyHint.Layers2dPhysics)] public uint DeadCollisionLayer;

		[Export(PropertyHint.Layers2dPhysics)] public uint DeadCollisionMask;

		[Export(PropertyHint.Range, "0")] public float Health = 100;

		[Export(PropertyHint.File)] public string WeaponScene = "";

		[Export(PropertyHint.Enum)] public Team CharacterTeam = Team.Player;

		protected bool Reloading = false;

		private bool _dead = false;

		private Vector2 _aimLocation;

		private bool isRunning = false;

		protected bool _isLookingLeft = false;
	
		protected Vector2 _velocity = Vector2.Zero;

		protected Animation.Animation _animation;

		protected WeaponBase _weapon;

		public virtual WeaponBase CurrentWeapon
		{
			get => _weapon;
		}

		private bool _jumpButtonDown = false;

		public bool IsPlayingAnyMontage()
		{
			return Reloading;
		}

		public bool IsRunning
		{
			get => isRunning;
			set => isRunning = value;
		}

		public Vector2 AimLocation
		{
			get => _aimLocation;
			set => _aimLocation = value;
		}

		public bool Dead
		{
			get => _dead;
			set => _dead = value;
		}

		public virtual float GetCurrentSpeed()
		{
			return Speed * (isRunning ? 2 : 1);
		}

		public virtual string GetCurrentAnimation()
		{
			string animType = "";
			if (Mathf.Abs(_velocity.x) > 0.01 && IsOnFloor())
			{
				animType = "Walk";
			}
			else if(IsOnFloor())
			{
				animType = "Idle";
			}

			if (!IsOnFloor() && _velocity.y < -0.01)
			{
				animType = "Jump";
			}
			else if (!IsOnFloor())
			{
				animType = "Fall";
			}

			return animType +"_"+ (CurrentWeapon != null ?CurrentWeapon.Type.ToString() : "None");
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			base._Ready();
			
			_animation = GetNode<Animation.Animation>("Animation");

			if (IsControlledByPlayer)
			{
				GetNode<Camera2D>("Camera2D").Current = true;
			}
			
		}

		protected virtual bool SpawnWeapon()
		{
			var scene = GD.Load<PackedScene>(WeaponScene);
			if (scene != null)
			{
				_weapon = GD.Load<PackedScene>(WeaponScene).Instance() as Weapons.WeaponBase;
				if (_weapon != null)
				{
					_weapon.OwningCharacter = this;
					AddChild(CurrentWeapon);
					return true;
				}
			}
			return false;
		}

		protected virtual void GetInput()
		{
			if (Input.IsActionPressed("move_right"))
			{
				_velocity.x = Speed;
			}

			if (Input.IsActionPressed("move_left"))
			{
				_velocity.x = -1* GetCurrentSpeed();
			}
		
			else if (!Input.IsActionPressed("move_left") && !Input.IsActionPressed("move_right"))
			{
				_velocity.x = 0;
			}

			if (Input.IsActionPressed("move_jump") )
			{
				_jumpButtonDown = true;
			}
			else
			{
				_jumpButtonDown = false;
			}

			if (Input.IsActionPressed("shoot"))
			{
				CurrentWeapon?.Shoot(
					new Vector2
					(
						(_animation.BulletSpawnPosition.Position.x*(_isLookingLeft ? 1 : -1)) + Position.x,
						_animation.BulletSpawnPosition.Position.y + Position.y
					),
					0,
					_isLookingLeft);
			}

			if (Input.IsActionPressed("reload") && !Reloading && CurrentWeapon != null)
			{
				Reloading = true;
				_animation.PlayMontage("Reload_"+CurrentWeapon.Type.ToString());
			}
			else if(Input.IsActionPressed("reload"))
			{
				GD.Print("Reloading: " + Reloading.ToString());
			}
			
			
			isRunning = Input.IsActionPressed("move_run");
		}

		public override void _Input(InputEvent @event)
		{
			if (IsControlledByPlayer && !Dead)
			{
				base._Input(@event);
				if (@event is InputEventMouseMotion eventMouse)
				{
					_aimLocation = eventMouse.Position;
				}
			}
		}

		public void Die()
		{
			if (!Dead)
			{
				Dead = true;
				
				_animation.PlayMontage("Death");

				CollisionLayer = DeadCollisionLayer;
				CollisionMask = DeadCollisionMask;
			}
		}

		public void DealDamage(float damage)
		{
			Health -= damage;
			if (Health <= 0)
			{
				Die();
			}
		}

		protected virtual void UpdateAnimation()
		{
			if (_velocity.x > 0.01)
			{
				_isLookingLeft =  true;
			}
			else if (_velocity.x < -0.01)
			{
				_isLookingLeft = false;
			}
			
			//set current direction
			//_isLookingLeft = (_aimLocation.x - GetViewport().Size.x / 2) > 0;
			
			if (_animation != null)
			{
				_animation.SetAnimation(GetCurrentAnimation());

				_animation.Scale = new Vector2(Mathf.Abs(_animation.Scale.x) * (_isLookingLeft ? 1 : -1),
					_animation.Scale.y);
				

				if (!_animation.IsPlaying())
				{
					_animation.Play();
				}
			}
			
		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);

			if (IsControlledByPlayer && !Dead)
			{
				GetInput();
			}

			_velocity.y += GravityForce * delta;


			_velocity = MoveAndSlide(_velocity,Vector2.Up);

			if (IsOnFloor() && _jumpButtonDown)
			{
				_velocity.y = JumpForce;
			}

			if (!Dead)
			{
				UpdateAnimation();
			}
		}

		private void _on_Animation_OnMontageFinished(string montageName)
		{
			if (montageName == ("Reload_" + CurrentWeapon.Type.ToString()) && Reloading)
			{
				Reloading = false;
				CurrentWeapon.Reload();
			}
		}

		private void _on_Animation_OnMontageInterrupted(string montageName, int currentFrame)
		{
			if (montageName == ("Reload_" + CurrentWeapon.Type.ToString()) && Reloading)
			{
				Reloading = false;
			}
		}

	}
}
