using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaleanPowerController : MonoBehaviour, ICharacterPowerController
{
    [Header("Power2")]
    public GameObject BlackHole;
    public Transform BlackHoleSpawn;
    public float Power2Cooldown;

	private float _power2TimeStamp = 0;

    public void StartPower1()
    {
		throw new System.NotImplementedException();
    }

    public void StartPower2()
    {
        if (_power2TimeStamp <= Time.time)
        {
            _power2TimeStamp = Time.time + Power2Cooldown;

            var blackHole = Instantiate (BlackHole, BlackHoleSpawn.position, BlackHoleSpawn.rotation);

            blackHole.GetComponent<BlackHole>().Initialize(transform.root.gameObject, GetComponent<PlayerInputManager>().GetAimDirection());
        }
    }

    void Start ()
	{

	}

	void Update ()
	{

	}
}
