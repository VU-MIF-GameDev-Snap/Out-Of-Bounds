using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boundary {

	public float xMin, xMax, zMin, zMax;
}

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class WeaponController : MonoBehaviour
{
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;

	private Rigidbody _rb;
	private AudioSource _audio;
	private float _nextFire;
	private KeyCode _keyCode;

	void Start() {

		_rb = GetComponent<Rigidbody> ();
		_audio = GetComponent<AudioSource> ();

		if (gameObject.tag.Equals("Weapon")) {
            
			_keyCode = KeyCode.KeypadEnter;
		}
        
	}

	void Update() {

		if (Input.GetKeyDown (_keyCode) && Time.time > _nextFire) {
			
			_nextFire = Time.time + fireRate;
            
			Instantiate (shot, shotSpawn.position, shotSpawn.rotation);

            _audio.Play ();
		}
	}
}
