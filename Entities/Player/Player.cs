using Dystopia.Entities.Weapons;
using Godot;
using Godot.Collections;

namespace Dystopia.Entities.Player
{
    public class Player : Character
    {
        //Ammo that player has
        public Array<WeaponBase.Ammo> Ammo = new Array<WeaponBase.Ammo>();
        
        //weapons that player has
        public Array<WeaponBase> Weapons = new Array<WeaponBase>();

        [Export()] public int CurrentWeaponId = -1;

        public override void _Ready()
        {
            base._Ready();
        }

        public override WeaponBase CurrentWeapon => (Weapons.Count > 0) ? Weapons[CurrentWeaponId] : null;

        public bool AddWeapon(WeaponBase.WeaponType type,string scene)
        {
            if (Weapons.Count > 0)
            {
                foreach (var weapon in Weapons)
                {
                    if (weapon.Type == type)
                    {
                        return false;
                    }
                }
            }

            _animation.PlayMontage("Pickup_Floor");
            WeaponScene = scene;
            SpawnWeapon();
          
            
            return true;
        }

        protected override bool SpawnWeapon()
        {
            var scene = GD.Load<PackedScene>(WeaponScene);
            if (scene != null)
            {
                var newWeapon = GD.Load<PackedScene>(WeaponScene).Instance() as Weapons.WeaponBase;
                if (newWeapon != null)
                {
                    newWeapon.OwningCharacter = this;
                    AddChild(newWeapon);
                    Weapons.Add(newWeapon);
                    if (CurrentWeaponId == -1)
                    {
                        CurrentWeaponId = 0;
                    }
                    return true;
                }
            }

            return false;
        }
    }
}
