using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseballBatWeaponController : BaseWeaponController
{
	[SerializeField]
	int HitDamage = 50;
	[SerializeField]
	int HitKnockback = 250;
	[SerializeField]
	int HitCount = 2;

	private bool _onCollideDoDamage;
	private PlayerController _ownerPlayerController;

	public override void Fire ()
	{
		// Initialize damage and on collision do damage once.
		_onCollideDoDamage = true;
		_ownerPlayerController = transform.root.gameObject.GetComponent<PlayerController>();
	}

	public override void ReduceAmmo ()
	{
		if (--HitCount < 1)
		{
			_ownerPlayerController.DropWeapon();
			Destroy(gameObject);
		}
	}

	public override WeaponType GetWeaponType ()
	{
		return WeaponType.BaseballBat;
	}

	private void OnTriggerEnter (Collider collision)
	{
		if (!_onCollideDoDamage)
		{
			return;
		}
		// do damage
		var playerController = collision.gameObject.GetComponent<PlayerController>();
		if (playerController == null || (_ownerPlayerController != null && _ownerPlayerController.gameObject == collision.gameObject))
		{
			return;
		}

		HitMessage hitMessage = new HitMessage();
		hitMessage.Damage = HitDamage;
		hitMessage.HitType = HitType.Punch;
		hitMessage.KnockbackValue = HitKnockback;
		hitMessage.KnockbackDirection = _ownerPlayerController.GetComponent<PlayerInputManager>().GetAimDirection();
		playerController.OnHit(hitMessage);
		_onCollideDoDamage = false;
	}
}
