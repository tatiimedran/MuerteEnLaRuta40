using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Scriptable Objects/Weapons")]
public class Weapon : ScriptableObject
{
    [Header("Weapon Information")]
    public string weaponName;            // Name of the weapon
    public int weaponSlot;               // Slot number assigned to the weapon
    public int attackDamage;             // Damage dealt by the weapon
    public float attackRange;            // Attack range (melee only)

    [Header("Attack Configuration")]
    public bool isRanged;                // Indicates if the weapon is ranged
    public GameObject projectilePrefab;  // Projectile prefab (for ranged weapons only)
    public float projectileSpeed;        // Projectile speed

    [Header("Animations and Effects")]
    public string attackAnimation;       // Trigger or Blend Tree tag
    public GameObject attackEffect;      // Visual effect
    public AudioClip attackSound;        // Attack sound effect
}
