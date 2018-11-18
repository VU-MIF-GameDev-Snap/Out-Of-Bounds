using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValkyriePowerController : MonoBehaviour, ICharacterPowerController
{
	[Header("Power1")]
    public GameObject Rocket;
    public Transform RocketSpawn1;
    public Transform RocketSpawn2;
    public float Power1Cooldown;

	private float _timeStamp = 0;

    public void StartPower1()
    {
		if (_timeStamp <= Time.time)
        {
            _timeStamp = Time.time + Power1Cooldown;

            var rocket1 = Instantiate (Rocket, RocketSpawn1.position, RocketSpawn1.rotation);
            var rocket2 = Instantiate (Rocket, RocketSpawn2.position, RocketSpawn2.rotation);

            rocket1.SendMessage("Initialize", transform.root.gameObject);
            rocket2.SendMessage("Initialize", transform.root.gameObject);
        }
    }

    public void StartPower2()
    {
        throw new System.NotImplementedException();
    }

    void Start ()
	{

	}

	void Update ()
	{

	}
}
