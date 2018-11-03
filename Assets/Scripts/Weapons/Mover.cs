using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Mover : MonoBehaviour {

	public float speed;
	private Rigidbody _rb;

	void Start() {

		_rb = GetComponent<Rigidbody> ();
		_rb.velocity = transform.forward * speed;
	}
}
