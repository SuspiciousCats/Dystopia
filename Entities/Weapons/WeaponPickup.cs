using Godot;

namespace Dystopia.Entities.Weapons
{
	public class WeaponPickup : Sprite
	{
		[Export(PropertyHint.File, "*.tscn")] public string Weapon;

		private WeaponBase _weapon;

		public override void _Ready()
		{
			base._Ready();

			var scene = GD.Load<PackedScene>(Weapon);
			if (scene != null)
			{
				var inst = scene.Instance();
				if (inst is WeaponBase)
				{
					_weapon = inst as WeaponBase;
				}

			}
		}

		
		public virtual void PickUp(Player.Player pickuper)
		{
			if (pickuper != null)
			{
				if (pickuper.AddWeapon(_weapon.Type,Weapon))
				{
					QueueFree();
				}
			}
		}

		private void _on_Area2D_body_entered(object body)
		{
			PickUp(body as Player.Player);
		}
	}
}



