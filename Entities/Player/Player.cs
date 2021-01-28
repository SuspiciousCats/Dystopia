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

        [Export()] public Array<int> Keys = new Array<int>();

        private bool _isInteractKeyDown = false;

        private Area2D _interationArea;
        public override void _Ready()
        {
            base._Ready();
            _interationArea = GetNode<Area2D>("InteractionArea2D");
            
        }

        protected void Interact()
        {
            if (_interationArea != null)
            {
               
                if (_interationArea.GetOverlappingAreas().Count > 0)
                {
                    
                    //try everyone of them in-case they are interactive areas
                    foreach (var area in _interationArea.GetOverlappingAreas())
                    {
                        if (area is Interactive.InteractiveObject interactiveObject)
                        {
                            if (interactiveObject.NeedsToBeUnlocked)
                            {
                                //try every key we have
                                foreach (int key in Keys)
                                {
                                    //stop trying if key is good
                                    if (interactiveObject.Interact(key))
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                interactiveObject.Interact();
                            }
                        }
                    }
                    
                }
            }
        }

        protected override void GetInput()
        {
            base.GetInput();
            if (Input.IsActionPressed("interact") && !_isInteractKeyDown)
            {
                Interact();
                //to avoid spam
                _isInteractKeyDown = true;
            }
            else if(!Input.IsActionPressed("interact"))
            {
                _isInteractKeyDown = false;
            }
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
