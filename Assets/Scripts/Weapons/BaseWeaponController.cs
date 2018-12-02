using UnityEngine;

public abstract class BaseWeaponController : MonoBehaviour
{
	public abstract void Fire ();
	public abstract void ReduceAmmo ();
	public abstract WeaponType GetWeaponType ();
}

public enum WeaponType
{
	BaseballBat,
	Rifle,
}
