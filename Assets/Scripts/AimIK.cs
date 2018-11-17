using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimIK : MonoBehaviour
{
	private Vector3 _ikLookPos;
	private Animator _animator;
	private Quaternion _rightHandRotationTarget;
	private Vector3 _actualLeftHandTargetPos;

	// For configuration.
	[SerializeField]
	float LookAtLerp = 7f;
	[SerializeField]
	float LookAtWeight = 1f;
	[SerializeField]
	float LookAtBodyWeight = 1f;
	[SerializeField]
	float LookAtHeadWeight = 1f;
	[SerializeField]
	float LookAtEyesWeight = 1f;
	[SerializeField]
	float LookAtClampWeight = 0f;

	// Accessed from other scripts.
	public Vector3 TargetDirection;
	public bool RifleHoldingMode = false;
	public Transform TransformTargetForLeftHand;

	void Start ()
	{
		_animator = GetComponent<Animator>();
	}

	void OnAnimatorIK ()
	{
		if(TargetDirection == Vector3.zero)
		{
			TargetDirection = transform.forward;
		}

		if(RifleHoldingMode)
		{
			_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
			_animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(TargetDirection, transform.right));
			_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
			_animator.SetIKPosition(AvatarIKGoal.LeftHand, _actualLeftHandTargetPos);
		}

		_ikLookPos = Vector3.Lerp(_ikLookPos, transform.position + TargetDirection*20, Time.deltaTime * LookAtLerp);
		_animator.SetLookAtWeight(LookAtWeight, LookAtBodyWeight, LookAtHeadWeight, LookAtEyesWeight, LookAtClampWeight);
		_animator.SetLookAtPosition(_ikLookPos);


	}

	void LateUpdate ()
	{
		// We have to cache target positions here,
		// Only in LateUpdate all bones positions are correct after animation is applied.
		if(TransformTargetForLeftHand != null)
		{
			_actualLeftHandTargetPos = TransformTargetForLeftHand.position;
		}
		else if (RifleHoldingMode)
		{
			RifleHoldingMode = false;
		}
	}
}
