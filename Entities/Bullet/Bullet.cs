using Godot;

namespace Dystopia.Entities.Bullet
{
	public class Bullet : Area2D
	{
		[Export(PropertyHint.Range, "0")] public float Speed = 300;
		
		
		//who shot the bullet(to avoid self damage)
		public Character OwningCharacter;

		//Weapon that spawned the bullet(for getting info)
		public Weapons.WeaponBase OwningWeapon;

		public bool IsGoingLeft = false;

		protected Vector2 _spawnPosition;
		
		protected float _spawnRotation;

		public Vector2 SpawnPosition
		{
			get => _spawnPosition;
			set { _spawnPosition = value;Position = value;}
		}

		public float SpawnRotation
		{
			get => _spawnRotation;
			set
			{
				_spawnRotation = value;
				Rotation = (IsGoingLeft ? 0 : 180) + value;
			} 
		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);

			Position += new Vector2(Mathf.Cos(_spawnRotation) * (IsGoingLeft ? 1 : -1), Mathf.Sin(_spawnRotation)) *
						delta *
						Speed;
		}

		private void _on_Bullet_body_entered(object body)
		{
			if (body is Character character && body != OwningCharacter)
			{
				character.DealDamage(OwningWeapon.Damage);
				QueueFree();
			}
		}
	}
}



