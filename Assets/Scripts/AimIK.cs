using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimIK : MonoBehaviour
{
	[SerializeField]
	public Vector3 TargetDirection;
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
	Vector3 ik_lookPos;
	Animator animator;
	void Start ()
	{
		animator = GetComponent<Animator>();
	}

	void OnAnimatorIK ()
	{
		if(TargetDirection == Vector3.zero)
			return;
		ik_lookPos = Vector3.Lerp(ik_lookPos, transform.position + TargetDirection*20, Time.deltaTime * LookAtLerp);
		animator.SetLookAtWeight(LookAtWeight, LookAtBodyWeight, LookAtHeadWeight, LookAtEyesWeight, LookAtClampWeight);
		animator.SetLookAtPosition(ik_lookPos);
	}
}
