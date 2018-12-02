using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour {

	[Header("Visuals")]
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
	public int Damage;
	public float SuckDuration;

	[Header ("Begone")]
	public float BegoneDuration;

	private float _flyTimeStamp;
	private Vector2 _flyDirection = new Vector2 (1, 0);

	private GameObject _owner;
	private List<GameObject> _characters = new List<GameObject>();
	// Characters which have been hit by black hole (to not hit second time)
	private List<GameObject> _charactersHit = new List<GameObject>();

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
		else if(Time.time < _flyTimeStamp + SuckDuration)
		{
			transform.localScale = Wobble(Time.time);
			Suck();
		}
		else if(Time.time < _flyTimeStamp + SuckDuration + BegoneDuration)
		{
			Begone();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Fly() {
		var direction = new Vector3(_flyDirection.x, _flyDirection.y, 0);
		var factor = Mathf.Pow(1 - (_flyTimeStamp - Time.time) / FlightTime, 2);
		transform.position = transform.position + (direction * FlightSpeed * 0.01f * (1 - factor));
		var goal = Wobble(_flyTimeStamp);
		var start = new Vector3(0.2f, 0.2f, 0.2f);
		transform.localScale = (goal - start) * factor + start;
	}

	Vector3 Wobble(float time) {
		var scaleX = Mathf.Abs(Mathf.Pow(Mathf.Sin(time * SpinXSpeed), 2) * (SpinXLength - SpinXMinLength)) + SpinXMinLength;
		var scaleY = Mathf.Abs(Mathf.Pow(Mathf.Sin(time * SpinYSpeed), 2) * (SpinYLength - SpinYMinLength)) + SpinYMinLength;
		return new Vector3(scaleX, scaleY, transform.localScale.z);
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

	void Begone() {
		var factor = Mathf.Pow((_flyTimeStamp + SuckDuration + BegoneDuration - Time.time) / BegoneDuration, 2);
		var start = Wobble(_flyTimeStamp + SuckDuration);
		transform.localScale = start * factor;
	}

	void OnTriggerEnter(Collider other)
	{	
		// If it's still flying, then no damage should be done
		if(Time.time < _flyTimeStamp)
			return;

		if(!other.CompareTag("Player") || other.transform.root.gameObject == _owner)
			return;

		var player = other.transform.root.gameObject;

		if(_charactersHit.Contains(player))
			return;

		var hitMessage = new HitMessage();
		hitMessage.Damage = Damage;
		hitMessage.KnockbackValue = 0;
		hitMessage.KnockbackDirection = new Vector3(0, 0, 0);

		player.GetComponent<PlayerController>().OnHit(hitMessage);

		_charactersHit.Add(player);
	}
}
