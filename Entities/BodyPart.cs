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
	}
}
