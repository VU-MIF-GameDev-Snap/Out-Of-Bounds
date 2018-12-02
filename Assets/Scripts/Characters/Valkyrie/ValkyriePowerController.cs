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

	private float _power1TimeStamp = 0;

    [Header("Power2")]
    public List<Material> Materials;
    public float Power2Cooldown;
    public float Power2Duration;
    public float Power2HealingAmount;
    public float Power2FadeTime;

    private float _power2TimeStamp = 0;
    private PlayerController _playerController;
    private int _healingDone = 0;

    public void StartPower1()
    {
		if (_power1TimeStamp <= Time.time)
        {
            _power1TimeStamp = Time.time + Power1Cooldown;

            var rocket1 = Instantiate (Rocket, RocketSpawn1.position, RocketSpawn1.rotation);
            var rocket2 = Instantiate (Rocket, RocketSpawn2.position, RocketSpawn2.rotation);

            rocket1.GetComponent<HomingMissile>().Initialize(transform.root.gameObject);
            rocket2.GetComponent<HomingMissile>().Initialize(transform.root.gameObject);
        }
    }

    public void StartPower2()
    {
        if (_power2TimeStamp > Time.time + Power2Cooldown)
            return;

        _power2TimeStamp = Time.time;
        _healingDone = 0;
    }

    void Start ()
	{
        _playerController =GetComponent<PlayerController>();
	}

	void Update ()
	{
        // Period of fading
        if(Time.time < _power2TimeStamp + Power2FadeTime && _power2TimeStamp > 0)
        {
            var factor = (_power2TimeStamp + Power2FadeTime - Time.time) / Power2FadeTime;

            foreach(var m in Materials)
            {
                var currentColor = m.GetColor("_Color");
                currentColor.a = factor;
                m.SetColor("_Color", currentColor);
            }
        }
        // Period of reappearing
        else if(Time.time > _power2TimeStamp + Power2FadeTime + Power2Duration && Time.time < _power2TimeStamp + Power2Duration + 2 * Power2FadeTime && _power2TimeStamp > 0)
        {
            var factor = 1 - ((_power2TimeStamp + Power2Duration + 2 * Power2FadeTime - Time.time) / Power2FadeTime);

            foreach(var m in Materials)
            {
                var currentColor = m.GetColor("_Color");
                currentColor.a = factor;
                m.SetColor("_Color", currentColor);
            }
        }
        // Period of healing
        else if(Time.time > _power2TimeStamp + Power2FadeTime && Time.time < _power2TimeStamp + Power2FadeTime + Power2Duration && _power2TimeStamp > 0)
        {
            var factor = 1 - ((_power2TimeStamp + Power2Duration + Power2FadeTime - Time.time) / Power2Duration);

            if(Power2HealingAmount * factor - _healingDone >= 1)
            {
                _playerController.ReduceHitpoints(1);
                ++_healingDone;
            }

            foreach(var m in Materials)
            {
                var currentColor = m.GetColor("_Color");
                currentColor.a = 0;
                m.SetColor("_Color", currentColor);
            }
        }
        else
        {
            foreach(var m in Materials)
            {
                var currentColor = m.GetColor("_Color");
                currentColor.a = 1;
                m.SetColor("_Color", currentColor);
            }
        }
	}
}
