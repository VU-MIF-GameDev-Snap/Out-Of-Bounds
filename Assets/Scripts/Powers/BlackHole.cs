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

	private float _flyTimeStamp;
	private Vector2 _flyDirection = new Vector2 (1, 0);


	// Use this for initialization
	void Start () {
		_flyTimeStamp = Time.time + FlightTime;
	}

	void Initialize(Vector2 direction) {
		_flyDirection = direction.normalized;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time < _flyTimeStamp)
		{
			Fly();
		}
		else
		{
			Wobble();
		}
	}

	void Fly() {
		var direction = new Vector3(_flyDirection.x, _flyDirection.y, 0);
		transform.position = transform.position + (direction * FlightSpeed * Time.deltaTime);
	}

	void Wobble() {
		var scaleX = Mathf.Abs(Mathf.Pow(Mathf.Sin(Time.time * SpinXSpeed), 2) * (SpinXLength - SpinXMinLength)) + SpinXMinLength;
		var scaleY = Mathf.Abs(Mathf.Pow(Mathf.Sin(Time.time * SpinYSpeed), 2) * (SpinYLength - SpinYMinLength)) + SpinYMinLength;
		transform.localScale = new Vector3(scaleX, scaleY, transform.localScale.z);
	}
}
