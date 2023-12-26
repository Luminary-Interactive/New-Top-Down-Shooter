using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    private IWeapon _equipped;
    private IWeapon _primary;
    private IWeapon _secondary;
    private IWeapon _throwable;
    private PlayerControls _inputManager;

    private void Awake()
    {
        _inputManager = new();
        _inputManager.Player.Drop.performed += (_) => DropEquipped();
        _inputManager.Player.EquipPrimary.performed += (_) => EquipPrimary();
        _inputManager.Player.EquipSecondary.performed += (_) => EquipSecondary();
        _inputManager.Player.EquipThrowable.performed += (_) => EquipThrowable();
        
        void DropEquipped()
        {
            _equipped.OnWeaponDrop();
            switch (_equipped.Type)
            {
                case WeaponType.Primary:
                    _primary = null;
                    break;
                case WeaponType.Secondary:
                    _secondary = null;
                    break;
                case WeaponType.Throwable:
                    _throwable = null;
                    break;
            }
            _equipped = null;
        }
        void EquipPrimary()
        {
            throw new System.NotImplementedException();
        }
        void EquipSecondary()
        {
            throw new System.NotImplementedException();
        }
        void EquipThrowable()
        {
            throw new System.NotImplementedException();
        }
    }
    private void OnEnable()
    {
        _inputManager.Enable();
    }
    private void OnDisable()
    {
        _inputManager.Disable();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<IWeapon>(out var weapon)) { return; }
        if (HasEquippedOfType(weapon)) { return; }

        Equip(weapon);

        bool HasEquippedOfType(IWeapon toCompare)
        {
            switch (toCompare.Type)
            {
                case WeaponType.Primary:
                    return _primary != null;
                case WeaponType.Secondary:
                    return _secondary != null;
                case WeaponType.Throwable:
                    return _throwable != null;
            }
            return true; // I see no reason this would ever happen but if it does, return true to be safe.
        }
        void Equip(IWeapon toEquip)
        {
            switch (toEquip.Type)
            {
                case WeaponType.Primary:
                    _primary = toEquip;
                    break;
                case WeaponType.Secondary:
                    _secondary = toEquip;
                    break;
                case WeaponType.Throwable:
                    _throwable = toEquip;
                    break;
            }
            toEquip.OnWeaponPickup();
        }
    }
}