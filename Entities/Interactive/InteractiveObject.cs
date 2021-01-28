using Godot;
using Godot.Collections;

namespace Dystopia.Entities.Interactive
{
	public class InteractiveObject : Area2D
	{
		[Export()] public Array<int> AcceptableKeys;

		[Export()] public bool NeedsKey = false;

		//function actually responsible for doing stuff(like moving elevator)
		public void DoAction()
		{
		}

		public virtual bool NeedsToBeUnlocked => NeedsKey && AcceptableKeys.Count > 0;
		
		//This function checks if all conditions are met and performs action
		public bool Interact(int key = -1)
		{
			if (!NeedsToBeUnlocked)
			{
				DoAction();
			}
			else if (key == -1 || AcceptableKeys.Contains(key) || AcceptableKeys.Count == 0)
			{
				DoAction();
			}
			return false;
		}
	}
}
