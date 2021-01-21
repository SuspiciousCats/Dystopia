using Godot;

namespace Dystopia.Entities.Weapons
{
    public class WeaponBase : AnimatedSprite
    {
        public bool CanShoot()
        {
            return true;
        }
        
        public void Shoot()
        {
        }
    }
}