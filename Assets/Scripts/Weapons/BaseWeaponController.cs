using UnityEngine;

public abstract class BaseWeaponController : MonoBehaviour
{
	public abstract void Fire ();
	public abstract void ReduceAmmo ();
	public abstract WeaponType GetWeaponType ();

	[Tooltip("Seconds after which the gun will disappear")]
	public int SelfDestructTimer = 5;
	private float _timeToDie = 0;

	protected AudioSource _audio;
	protected Transform _originalParent;

	void Start ()
	{
		_audio = GetComponent<AudioSource>();
		_originalParent = gameObject.transform.root;
	}

	void Update ()
	{
		if (ShouldSelfDestruct() && !IsBeingHeld())
			Destroy(gameObject);
	}

	private bool ShouldSelfDestruct ()
	{
		// Death timer not set yet
		if (_timeToDie == 0)
			_timeToDie = Time.time + SelfDestructTimer;

		return _timeToDie <= Time.time;
	}

	private bool IsBeingHeld ()
	{
		return _originalParent != gameObject.transform.root;
	}
}

public enum WeaponType
{
	BaseballBat,
	Rifle,
}
