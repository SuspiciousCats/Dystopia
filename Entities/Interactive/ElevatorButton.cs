using Dystopia.Entities.Door;

namespace Dystopia.Entities.Interactive
{
	//this class MUST always be a child of elevator node
	public class ElevatorButton : InteractiveObject
	{
		private Elevator _elevator;

		public override void _Ready()
		{
			base._Ready();
			_elevator = GetParent<Elevator>();
		}

		public override void DoAction()
		{
			base.DoAction();
			_elevator.Move();
		}
	}
}
