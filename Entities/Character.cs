using Dystopia.Entities.Weapons;
using Godot;

namespace Dystopia.Entities
{
	public class Character : KinematicBody2D
	{

		public enum AnimationOverlayType
		{
			None,
			Pistol,
			Rifle
		}

		[Export] public bool IsControlledByPlayer = true;
		
		[Export] public float Speed = 100;

		[Export] public float GravityForce = 980;

		[Export] public float JumpForce = -450;

		[Export(PropertyHint.File)] public string WeaponScene = "";

		[Export(PropertyHint.Enum)] public AnimationOverlayType CurrentOverlay;

		private Vector2 _aimLocation;

		private bool isRunning = false;

		private bool _isLookingLeft = false;
	
		private Vector2 _velocity = Vector2.Zero;

		private BodyPart _skeletalMesh;

		protected Node2D _skeletalMesh_Torso;

		private WeaponBase _weapon;

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

		public virtual float GetCurrentSpeed()
		{
			return Speed * (isRunning ? 2 : 1);
		}

		public virtual string GetCurrentAnimation()
		{
			string animType = "";
			if (Mathf.Abs(_velocity.x) > 0.01)
			{
				animType = (isRunning ? "Run" : "Walk");
			}
			else
			{
				animType =  "Idle";
			}

			if (CurrentOverlay == AnimationOverlayType.None)
			{
				if (Mathf.Abs(_velocity.x) > 0.01)
				{
					return (isRunning ? "Run" : "Walk");
				}
				else
				{
					return "Idle";
				}
			}

			if (CurrentOverlay == AnimationOverlayType.Pistol)
			{
				return "Pistol_" + animType;
			}

			if (CurrentOverlay == AnimationOverlayType.Rifle)
			{
				return "Rifle_" + animType;
			}

			return "None";

		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_skeletalMesh = GetNode<BodyPart>("SkeletalMesh");

			_skeletalMesh_Torso = _skeletalMesh.FindNode("Manny_Torso") as Node2D;

			var scene = GD.Load<PackedScene>(WeaponScene);
			if (scene != null)
			{
				_weapon = GD.Load<PackedScene>(WeaponScene).Instance() as WeaponBase;
				if (_weapon != null)
				{
					_weapon.OwningCharacter = this;
					_skeletalMesh.FindNode("Manny_Wrist_Right").AddChild(_weapon);
				}
			}
			
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

			if (Input.IsActionPressed("move_jump"))
			{
				_velocity.y = JumpForce;
			}

			if (Input.IsActionPressed("shoot"))
			{
				if (_weapon != null)
				{
					_weapon.Shoot(_weapon.Position + Position, _skeletalMesh_Torso.Rotation,_isLookingLeft);
				}
			}
			
			
			isRunning = Input.IsActionPressed("move_run");
		}

		public override void _Input(InputEvent @event)
		{
			base._Input(@event);
			if (@event is InputEventMouseMotion eventMouse)
			{
				_aimLocation = eventMouse.Position;
			}
		}

		protected virtual void UpdateAnimation()
		{
			//set current direction
			_isLookingLeft = (_aimLocation.x - GetViewport().Size.x / 2) > 0;
			
			if (_skeletalMesh != null)
			{
				_skeletalMesh.SetAnimation(GetCurrentAnimation());
				
				_skeletalMesh.Scale = new Vector2(Mathf.Abs(_skeletalMesh.Scale.x) * (_isLookingLeft ? 1 : -1),
						_skeletalMesh.Scale.y);
				

				if (!_skeletalMesh.Animation.IsPlaying())
				{
					_skeletalMesh.Animation.Play();
				}

				if (_skeletalMesh_Torso != null)
				{
					_skeletalMesh_Torso.LookAt(_aimLocation - GetViewport().Size/2);
					
					_skeletalMesh_Torso.RotationDegrees += 15;
				}
			}
			
		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);
			
			if (IsControlledByPlayer)
			{
				GetInput();
			}

			_velocity.y += GravityForce * delta;
			
			if (IsOnFloor())
			{
				_velocity.y = 0;
			}

			_velocity = MoveAndSlide(_velocity);

			UpdateAnimation();
			
		}
	}
}
