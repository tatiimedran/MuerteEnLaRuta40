using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Scriptable Objects/Weapons")]
public class Weapon : ScriptableObject
{
    [Header("Informaci�n del arma")]
    public string weaponName;        // Nombre del arma
    public int weaponSlot;           // N�mero asignado al arma
    public int attackDamage;         // Da�o que inflige el arma
    public float attackRange;        // Rango de ataque (solo cuerpo a cuerpo)

    [Header("Configuraci�n del ataque")]
    public bool isRanged;            // Indica si es un arma de ataque a distancia
    public GameObject projectilePrefab; // Prefab del proyectil (solo para armas a distancia)
    public float projectileSpeed;      // Velocidad del proyectil

    [Header("Animaciones y efectos")]
    public string attackAnimation;   // Trigger o tag del Blend Tree
    public GameObject attackEffect;  // Efecto visual
    public AudioClip attackSound;    // Sonido al atacar
}
