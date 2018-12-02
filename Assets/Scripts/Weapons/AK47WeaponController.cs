using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class AK47WeaponController : BaseWeaponController
{
	[Header("Weapon attributes")]
	public float FireRate = 0.1f;
	public int Ammo = 5;
	public int Damage = 5;
	public int KnockbackValue = 5;

	[Header("External objects")]
	public GameObject Shot;
	public Transform shotSpawn;
	public Transform LeftHandPosition;

	private float _nextFire;

	public override void Fire ()
	{
		// Cooldown still active
		if (Time.time < _nextFire)
			return;

		_nextFire = Time.time + FireRate;

		var bullet = Instantiate(Shot, shotSpawn.position, shotSpawn.rotation);
		var message = new HitMessage()
		{ HitType = HitType.Rifle, Damage = Damage, KnockbackValue = KnockbackValue, KnockbackDirection = this.transform.forward };
		bullet.GetComponent<HitEvent>().Initialise(message);

		_audio.Play();
		ReduceAmmo();
	}

	public Transform GetLeftHandPosition ()
	{
		return LeftHandPosition;
	}

	public override WeaponType GetWeaponType ()
	{
		return WeaponType.Rifle;
	}

	public override void ReduceAmmo ()
	{
		if (--Ammo <= 0)
			Destroy(gameObject);
	}
}
