using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class WeaponController : MonoBehaviour
{
    [Tooltip("Seconds after which the gun will disappear")]
    public int SelfDestructTimer = 5;
    private float _timeToDie = 0;

    [Header("Weapon attributes")]
	public float FireRate = 0.1f;
    public int Ammo = 5;
    public float Damage;    

    [Header("External objects")]
    public GameObject Shot;
	public Transform shotSpawn;


    private AudioSource _audio;
    private float _nextFire;
    private KeyCode _keyCode;
    
    private Transform _originalParent;

	void Start() 
    {
		_audio = GetComponent<AudioSource> ();
        _originalParent = gameObject.transform.root;

		if (gameObject.tag.Equals("Weapon")) 
			_keyCode = KeyCode.KeypadEnter;
	}

	void Update() 
    {
        if (ShouldSelfDestruct() && !IsBeingHeld())
            Destroy(gameObject);

        if (Input.GetKey(_keyCode))
            Fire();
	}

    public void Fire ()
    {
        // Cooldown still active
        if (Time.time < _nextFire)
            return;

        _nextFire = Time.time + FireRate;
        Instantiate(Shot, shotSpawn.position, shotSpawn.rotation);
        _audio.Play();
        Ammo--;
        if (Ammo <= 0)
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