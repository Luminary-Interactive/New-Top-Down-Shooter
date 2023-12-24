public interface IWeapon
{
    WeaponType Type { get; }

    void OnWeaponPickup();
    void OnWeaponDrop();
}