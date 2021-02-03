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

		[Export(PropertyHint.Enum)] public Array<Team> EnemyTeams;

		[Export()] public float PatrolWaitTime = 3;

		[Export()] public bool IsGoingLeft = false;

		[Export()] public bool DrawDebugLines = false;

		[Export()] public bool DoDebugOutput = false;

		protected bool IsWaiting = false;
		
		/*used for primitive path finding
		* If this trigger returns true then there is a wall in front of the ai->ai should turn around and move
		*/
		protected Area2D FloorDetection;

		/*Used for primitive path finding
		 *If this trigger returns false there is no floor to walk on->->ai should turn around and move
		 * */
		protected Area2D WallDetection;

		protected Area2D SenseArea;
			
		protected State _currentState = State.Idle;

		public State CurrentState => _currentState;

		private Character Target;

		private Timer WaitTimer;
		
		protected int FloorInterceptionCount = 0;

		protected int WallInterceptionCount = 0;

		private Line2D _debugLine;

		private RichTextLabel _debugOutput;

		public override void _Ready()
		{

			_isLookingLeft = IsGoingLeft;

			base._Ready();
			SenseArea = GetNode<Area2D>("Animation/Sense");
			
			FloorDetection = GetNode<Area2D>("PathDetection_Floor");
			WallDetection = GetNode<Area2D>("PathDetection_Wall");

			FloorDetection.Position = new Vector2(IsGoingLeft ? 0 : -40, 0);
			WallDetection.Position = new Vector2(IsGoingLeft ? 0 : -40, 0);

			_debugLine = GetNode<Line2D>("PerceptionDebugLine2D");

			SpawnWeapon();

			if (DoDebugOutput)
			{
				_debugOutput = GetNode<RichTextLabel>("DebugAiDisplay");
			}
			else
			{
				GetNode<RichTextLabel>("DebugAiDisplay")?.QueueFree();
				FindNode("WallDetect_ColorRect").QueueFree();
				FindNode("FloorDetect_ColorRect").QueueFree();
			}
		}

		public bool CanWalkForward()
		{
			return FloorInterceptionCount > 0 && WallInterceptionCount == 0;
		}

		protected void GetTarget(Array sensed)
		{
			if (sensed.Count <= 0)
			{
				return;
			}

			foreach (var body in sensed)
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

		protected virtual void UpdateAiMovement()
		{
			if (DoDebugOutput)
			{
				_debugOutput.Text = "FloorInterceptionCount " + FloorInterceptionCount.ToString() + "\n" +
									"WallInterceptionCount " + WallInterceptionCount.ToString();

				if (WallInterceptionCount > 0)
				{
					_debugOutput.Text += "\n" + WallDetection.GetOverlappingBodies()[0].ToString();
				}
			}

			if (Target == null && !IsWaiting && IsOnFloor())
			{
				if (CanWalkForward())
				{
					_velocity.x = Speed * (IsGoingLeft ? 1 : -1);
				}
				else if (WaitTimer == null || WaitTimer.IsStopped())
				{
					_velocity.x = 0;
					IsWaiting = true;
					WaitTimer = new Timer();
					WaitTimer.WaitTime = PatrolWaitTime;
					WaitTimer.Connect("timeout", this, "OnStoppedWaiting");
					AddChild(WaitTimer);
					WaitTimer.Start();

					/*The detection positions are update here because ai is still in movement at this frame
					 and as such overlaps will be properly updated
					 Setting it afterwards will mean that they are update while ai is standing
					*/
					FloorDetection.Position = new Vector2(!IsGoingLeft ? 0 : -50, 0);
					WallDetection.Position = new Vector2(!IsGoingLeft ? 0 : -50, 0);
				}
			}
			else
			{
				_velocity.x = 0;
			}
		}

		//this checks if there is an obstacle between target and ai using ray cast
		protected bool CanActuallySeeTarget()
		{
			if (Target != null)
			{
				var spaceState = GetWorld2d().DirectSpaceState;
				var result = spaceState.IntersectRay(GlobalPosition, Target.GlobalPosition, new Array() {this});

				if (DrawDebugLines && Target != null)
				{
					_debugLine.ClearPoints();
					_debugLine.AddPoint(GlobalPosition);
					_debugLine.AddPoint(Target.GlobalPosition);
				}
				
				if (result != null && result["collider"] != null)
				{
					if (result["collider"] is Character character)
					{
						return !character.Crouching;
					}
				}
			}
			return false;
		}


		public virtual void UpdateAI()
		{

			Array senced = SenseArea.GetOverlappingBodies();
			
			if (!senced.Contains(Target))
			{
				GetTarget(senced);
			}

			
			
			if (Target != null && !Target.Dead && CanActuallySeeTarget())
			{
				if (_weapon != null)
				{
					if (_weapon.CanShoot())
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
					else if (_weapon.CurrentAmmoInTheClip == 0 && !Reloading)
					{
						Reloading = true;
						_animation.PlayMontage("Reload_" + CurrentWeapon.Type.ToString());
					}
				}
			}

			UpdateAiMovement();
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
			
			if (SenseArea != null && !Dead)
			{
				UpdateAI();
			}
		}

		protected virtual void OnStoppedWaiting()
		{
			WaitTimer?.Stop();
			IsWaiting = false;
			IsGoingLeft = !IsGoingLeft;
			_isLookingLeft = IsGoingLeft;
			UpdateAnimation();
		}


		private void _on_PathDetection_Floor_body_entered(object body)
		{
			FloorInterceptionCount++;
		}


		private void _on_PathDetection_Floor_body_exited(object body)
		{
			FloorInterceptionCount--;
		}


		private void _on_PathDetection_Wall_body_entered(object body)
		{
			WallInterceptionCount++;
		}


		private void _on_PathDetection_Wall_body_exited(object body)
		{
			WallInterceptionCount--;
		}

		private void _on_Animation_OnMontageFinished(string montageName)
		{
			
			if (CurrentWeapon != null)
			{
				if (montageName == ("Reload_" + CurrentWeapon.Type.ToString()) && Reloading)
				{
					Reloading = false;
					CurrentWeapon.Reload();
				}
			}
		}

		private void _on_Animation_OnMontageInterrupted(string montageName, int currentFrame)
		{
			if (CurrentWeapon != null)
			{
				if (montageName == ("Reload_" + CurrentWeapon.Type.ToString()) && Reloading)
				{
					Reloading = false;
				}
			}
		}
	}
}





