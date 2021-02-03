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
				GD.Print(GetTree().Root.GetChild(0).FindNode(doorName, true, false) is Door.Door);
				(GetTree().Root.GetChild(0).FindNode(doorName, true, false)as Door.Door)?.Toggle();
			}
		}
	}
}
