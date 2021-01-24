using Godot;
using Godot.Collections;

namespace Dystopia.Entities.AI
{
	public class AIBase : Character
	{
		public enum State
		{
			
			Idle,
			//Ai is in combat(for example: shooting at player)
			InCombat,
			//If player manages to kick the weapon out of ai's hands than ai starts to fight
			InCloseCombat
		}

		[Export(PropertyHint.Enum)] private Array<Team> EnemyTeams;

		protected Area2D SenseArea;
			
		protected State _currentState = State.Idle;

		public State CurrentState => _currentState;

		private Character Target;

		public override void _Ready()
		{
			base._Ready();
			SenseArea = GetNode<Area2D>("Animation/Sense");
		}

		protected void GetTarget(Array sensed)
		{
			foreach (PhysicsBody2D body in sensed)
			{
				if (body is Character character && body != this)
				{
					if (EnemyTeams.Contains(character.CharacterTeam))
					{
						Target = character;
						TargetFound();
						return;
					}
				}
			}

			if (Target != null)
			{
				TargetLost();
			}
			
			Target = null;
		}
		
		public virtual void UpdateAI()
		{

			Array senced = SenseArea.GetOverlappingBodies();
			
			if (!senced.Contains(Target))
			{
				GetTarget(senced);
			}
			
			if (Target != null)
			{
				if (_weapon != null)
				{
					_weapon.Shoot(
						new Vector2
						(
							(_animation.BulletSpawnPosition.Position.x * (_isLookingLeft ? 1 : -1)) + Position.x,
							_animation.BulletSpawnPosition.Position.y + Position.y
						),
						0,
						_isLookingLeft);
				}
			}
		}

		public virtual void TargetFound()
		{
			GD.Print("Found!");
		}

		public virtual void TargetLost()
		{
			GD.Print("Lost");
		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);
			if (SenseArea != null)
			{
				UpdateAI();
			}
		}
	}
}
