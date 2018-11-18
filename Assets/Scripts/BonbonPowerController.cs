using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerInputManager))]
public class BonbonPowerController : MonoBehaviour, ICharacterPowerController
{
    private float _chargePunchStartTime;
    private float _chargePunchCooldownStartTime;
    private bool _chargePunchActive = false;
    private PlayerController.PlayerAbility[] _chargePunchAbilitiesToTurnOff = {
        PlayerController.PlayerAbility.Jump,
        PlayerController.PlayerAbility.Power2,
        PlayerController.PlayerAbility.Punch,
        PlayerController.PlayerAbility.Shoot,
        };
    private PlayerController _playerController;
    private PlayerInputManager _inputManager;
    private Animator _animator;

    [SerializeField]
    ParticleSystem ChargePunchChargingParticleSystem;
    [SerializeField]
    ParticleSystem ChargePunchFinishParticleSystem;
    [SerializeField]
    float ChargePunchCooldown = 10f;
    [SerializeField]
    float ChargePuchDamagePerSecond = 10f;
    float ChargePuchKnockbackPerSecond = 10f;

    public void StartPower1()
    {
        if(_chargePunchCooldownStartTime + ChargePunchCooldown > Time.time || _chargePunchActive)
        {
            // Can't activate yet.
            return;
        }
        _chargePunchActive = true;

        _chargePunchStartTime = Time.time;

        foreach (var ability in _chargePunchAbilitiesToTurnOff)
        {
            _playerController.AbilityToggle(ability, false);
        }
        _animator.SetTrigger("ChargePunchTrigger");
        return;
        throw new System.NotImplementedException();
        // Charge punch
        // * Idea:
        // * Press down button
        // * While holding "charges"
        // * Released start attack animation with damage calculated from charging time.

        // ! While charging disable regular punch, power2
    }

    private void HandleChargePunch()
    {
        _animator.SetBool("ChargePunch", _chargePunchActive);
        if(!_chargePunchActive)
        {
            return;
        }

        if(!_inputManager.IsButtonDown(PlayerInputManager.Key.Power1))
        {
            _chargePunchActive = false;
            _chargePunchCooldownStartTime = Time.time;

            // * Consider reseting abilities a little later, after animation is done.
            foreach (var ability in _chargePunchAbilitiesToTurnOff)
            {
                _playerController.AbilityToggle(ability, true);
            }
        }
    }

    public void OnChargePunchLoop()
    {
        var sphericalShape = ChargePunchChargingParticleSystem.shape;
        var newRadius = Mathf.Clamp((Time.time - _chargePunchStartTime) / 10f, 0.1f, 1.5f);
        var particleMainOptions = ChargePunchChargingParticleSystem.main;
        particleMainOptions.startSpeed = -newRadius;
        particleMainOptions.startLifetime = newRadius;
        sphericalShape.radius = newRadius;

        ChargePunchChargingParticleSystem.Play();
    }

    public void OnChargePunchFinish()
    {
        var coneShape = ChargePunchFinishParticleSystem.shape;
        coneShape.angle = Mathf.Clamp((Time.time - _chargePunchStartTime) * 4.6f, 10f, 70f);
        var particleEmissionOptions = ChargePunchFinishParticleSystem.emission;
        particleEmissionOptions.rateOverTime = Mathf.Clamp((Time.time - _chargePunchStartTime) * 26f, 50f, 400f);
        ChargePunchFinishParticleSystem.Play();

        var playerCollisions = Physics.OverlapBox(ChargePunchFinishParticleSystem.transform.position + ChargePunchFinishParticleSystem.transform.forward * ChargePunchFinishParticleSystem.shape.scale.z/2, ChargePunchFinishParticleSystem.shape.scale, ChargePunchFinishParticleSystem.transform.rotation)
            .Select((collider) => {
            return collider.GetComponent<PlayerController>();
        }).Where((playerController) => {
            return playerController != null && playerController.gameObject != gameObject;
        });

        foreach (var player in playerCollisions)
        {
            var hitMessage = new HitMessage
            {
                Damage = (int)((Time.time - _chargePunchStartTime) * ChargePuchDamagePerSecond),
                HitType = HitType.None,
                KnockbackDirection = ChargePunchFinishParticleSystem.transform.forward,
                KnockbackValue = (int)((Time.time - _chargePunchStartTime) * ChargePuchKnockbackPerSecond),
            };

            player.OnHit(hitMessage);
        }
    }

    public void StartPower2()
    {
        throw new System.NotImplementedException();
        // Stone form
    }

    void Start ()
	{
        _playerController = gameObject.GetComponent<PlayerController>();
        _inputManager = GetComponent<PlayerInputManager>();
        _animator = GetComponent<Animator>();
	}

	void Update ()
	{
        HandleChargePunch();
	}
}
