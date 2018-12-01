using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour {

	[Header("Look")]
	public float SpinXSpeed;
	public float SpinYSpeed;
	public float SpinXLength;
	public float SpinYLength;
	public float SpinXMinLength;
	public float SpinYMinLength;

	[Header("Fly")]
	public float FlightTime;
	public float FlightSpeed;

	[Header ("Suck")]
	public float SuckStrength;

	private float _flyTimeStamp;
	private Vector2 _flyDirection = new Vector2 (1, 0);

	private GameObject _owner;
	private List<GameObject> _characters = new List<GameObject>();



	// Use this for initialization
	void Start () {
		_flyTimeStamp = Time.time + FlightTime;
		transform.eulerAngles = new Vector3(0, 0, 0);
	}

	public void Initialize(GameObject owner, Vector2 direction) {
		_owner = owner;
		_flyDirection = direction.normalized;

		// Get all other characters to follow
		var characters = GameObject.FindGameObjectsWithTag("Player");

		foreach(GameObject c in characters)
		{
			if(c != _owner)
			{
				_characters.Add(c);
			}
		}
	}
	
	void FixedUpdate () {
		if(Time.time < _flyTimeStamp)
		{
			Fly();
		}
		else
		{
			Wobble();
			Suck();
		}
	}

	void Fly() {
		var direction = new Vector3(_flyDirection.x, _flyDirection.y, 0);
		transform.position = transform.position + (direction * FlightSpeed * 0.01f);
	}

	void Wobble() {
		var scaleX = Mathf.Abs(Mathf.Pow(Mathf.Sin(Time.time * SpinXSpeed), 2) * (SpinXLength - SpinXMinLength)) + SpinXMinLength;
		var scaleY = Mathf.Abs(Mathf.Pow(Mathf.Sin(Time.time * SpinYSpeed), 2) * (SpinYLength - SpinYMinLength)) + SpinYMinLength;
		transform.localScale = new Vector3(scaleX, scaleY, transform.localScale.z);
	}

	void Suck() {
		foreach(var c in _characters) {
			if(c == null)
				continue;

			var target = c.GetComponent<Collider>().bounds.center;
			var distance = Vector3.Distance(c.transform.position, target);

			var hitMessage = new HitMessage();

			hitMessage.Damage = 0;
			hitMessage.KnockbackValue = (int)(SuckStrength * 0.01f / Mathf.Clamp(Mathf.Pow(distance, 2), 1f, 1000f));
			hitMessage.KnockbackDirection = (transform.position - target).normalized;

			c.GetComponent<PlayerController>().OnHit(hitMessage);
		}
	}
}
