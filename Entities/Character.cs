using Godot;

namespace Dystopia.Entities
{
	public class Character : KinematicBody2D
	{
		[Export] public float Speed = 100;

		[Export] public float GravityForce = 980;

		[Export] public float JumpForce = -450;

		private bool isRunning = false;

		private bool _isLookingLeft = false;
	
		private Vector2 _velocity = Vector2.Zero;

		private BodyPart _lowerBodyNode;

		private BodyPart _upperBodyNode;

		public bool IsRunning
		{
			get => isRunning;
			set => isRunning = value;
		}
		
		public virtual float GetCurrentSpeed()
		{
			return Speed * (isRunning ? 2 : 1);
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_lowerBodyNode = GetNode<BodyPart>("LowerBody");

			_upperBodyNode = GetNode<BodyPart>("UpperBody");
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

			isRunning = Input.IsActionPressed("move_run");
		}

		protected virtual void UpdateAnimation()
		{
			if (_lowerBodyNode != null && _upperBodyNode != null)
			{
				if (Mathf.Abs(_velocity.x) >= 0.01)
				{
					//update current animation
					_lowerBodyNode.SetAnimation(isRunning ? "Run" : "Walk");
					_lowerBodyNode.Scale = new Vector2(_lowerBodyNode.Scale.x * (_isLookingLeft ? -1 : 1),
					_lowerBodyNode.Scale.y);_upperBodyNode.SetAnimation(isRunning ? "Run" : "Walk");

					//set current direction
					_isLookingLeft = _velocity.x > 0;
					
				}
				else
				{
					_lowerBodyNode.SetAnimation("None");
					_upperBodyNode.SetAnimation("None");
				}

				if (!_lowerBodyNode.Animation.IsPlaying())
				{
					_lowerBodyNode.Animation.Play();
				}

				_lowerBodyNode.Scale = new Vector2(Mathf.Abs(_lowerBodyNode.Scale.x) * (_isLookingLeft ? 1 : -1),
					_lowerBodyNode.Scale.y);

				_upperBodyNode.Scale = new Vector2(Mathf.Abs(_upperBodyNode.Scale.x) * (_isLookingLeft ? 1 : -1),
					_upperBodyNode.Scale.y);
				
			}
			
		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);
			
			GetInput();
			
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
