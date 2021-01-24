using Godot;

namespace Dystopia.Entities
{
	public class BodyPart : Node2D
	{
		private AnimationPlayer _animation;

		public AnimationPlayer Animation
		{
			get => _animation;
		}

		public Character OwningCharacter; 
		/*Safe way of changing the animation
		 *Returns false if animation is not found
		*/
		public bool SetAnimation(string animationName)
		{
			if (_animation.HasAnimation(animationName))
			{
				_animation.CurrentAnimation = animationName;
				return true;
			}

			return false;
		}
		
		public override void _Ready()
		{
			base._Ready();
			
			_animation = GetNode<AnimationPlayer>("AnimationPlayer");
		}

		private void _on_AnimationPlayer_animation_finished(string anim_name)
		{
			if (OwningCharacter != null)
			{
				OwningCharacter.OnAnimationEnd(anim_name);
			}
		}

		private void _on_AnimationPlayer_animation_changed(string old_name, string new_name)
		{
			if (OwningCharacter != null)
			{
				OwningCharacter.OnAnimationInterrupt(old_name);
			}
		}
	}
}






