using Godot;
using Godot.Collections;

namespace Dystopia.Entities.Interactive
{
	public class DoorButton : InteractiveObject
	{
		[Export()] public Array<string> DoorNames;

		public override void DoAction()
		{
			base.DoAction();
			foreach (var doorName in DoorNames)
			{
				(GetParent().FindNode(doorName)as Door.Door)?.Toggle();
			}
		}
	}
}
