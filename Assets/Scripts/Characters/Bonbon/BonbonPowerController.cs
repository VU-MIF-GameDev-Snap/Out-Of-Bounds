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
        PlayerController.PlayerAbility.Dash,
    };
    private float _stoneFormStartTime;
    private float _stoneFormCooldownStartTime;
    private int _stoneFormAlreadyHealedAmount;
    private bool _stoneFormActive = false;
    private PlayerController.PlayerAbility[] _stoneFormAbilitiesToTurnOff = {
        PlayerController.PlayerAbility.Jump,
        PlayerController.PlayerAbility.Power1,
        PlayerController.PlayerAbility.Punch,
        PlayerController.PlayerAbility.Shoot,
        PlayerController.PlayerAbility.Aim,
        PlayerController.PlayerAbility.Dash,
        PlayerController.PlayerAbility.Walk,
    };
    private PlayerController _playerController;
    private PlayerInputManager _inputManager;
    private Animator _animator;

    [SerializeField]
    Renderer RendererForMaterial;
    [Header("ChargePunch")]
    [SerializeField]
    ParticleSystem ChargePunchChargingParticleSystem;
    [SerializeField]
    ParticleSystem ChargePunchFinishParticleSystem;
    [SerializeField]
    float ChargePunchCooldown = 10f;
    [SerializeField]
    float ChargePuchDamagePerSecond = 10f;
    [SerializeField]
    float ChargePuchKnockbackPerSecond = 10f;
    [SerializeField]
    float ChargePunchMaxChargeDuration = 15f;

    [Header("StoneForm")]
    [SerializeField]
    ParticleSystem StoneFormFormingParticleSystem;
    [SerializeField]
    ParticleSystem StoneFormBreakParticleSystem;
    [SerializeField]
    ParticleSystem StoneFormHealingParticleSystem;
    [SerializeField]
    float StoneFormCooldown = 7f;
    [SerializeField]
    float StoneFormHealingPerSecond = 20f;
    [SerializeField]
    float StoneFormMaxDuration = 6f;
    [SerializeField]
    float StoneFormMinDuration = 1.2f;
    [SerializeField]
    float StoneFormActivationDelay = 1f;
    [SerializeField]
    Material OriginalMaterial;
    [SerializeField]
    Material StoneFormMaterial;

    public void StartPower1 ()
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
    }

    private void HandleChargePunch ()
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
        }
    }

    public void OnChargePunchLoop ()
    {
        var currentDuration = Time.time - _chargePunchStartTime;
        if(currentDuration > ChargePunchMaxChargeDuration)
        {
            currentDuration = ChargePunchMaxChargeDuration;
        }

        var sphericalShape = ChargePunchChargingParticleSystem.shape;
        var minRadius = 0.1f;
        var maxRadius = 1.5f;
        var newRadius = Mathf.Clamp((currentDuration / ChargePunchMaxChargeDuration) * (maxRadius - minRadius) + minRadius, minRadius, maxRadius);
        sphericalShape.radius = newRadius;

        var particleMainOptions = ChargePunchChargingParticleSystem.main;
        particleMainOptions.startSpeed = -newRadius;

        ChargePunchChargingParticleSystem.Play();
    }

    public void OnChargePunchFinish ()
    {
        var currentDuration = Time.time - _chargePunchStartTime;
        if(currentDuration > ChargePunchMaxChargeDuration)
        {
            currentDuration = ChargePunchMaxChargeDuration;
        }

        var coneShape = ChargePunchFinishParticleSystem.shape;
        var minAngle = 10f;
        var maxAngle = 70f;
        coneShape.angle = Mathf.Clamp((currentDuration / ChargePunchMaxChargeDuration) * (maxAngle - minAngle) + minAngle, minAngle, maxAngle);
        var particleEmissionOptions = ChargePunchFinishParticleSystem.emission;
        var minEmissionRate = 50f;
        var maxEmissionRate = 400f;
        particleEmissionOptions.rateOverTime = Mathf.Clamp((currentDuration / ChargePunchMaxChargeDuration) * (maxEmissionRate - minEmissionRate) + minEmissionRate, minEmissionRate, maxEmissionRate);
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
                Damage = (int)(currentDuration * ChargePuchDamagePerSecond),
                HitType = HitType.None,
                KnockbackDirection = ChargePunchFinishParticleSystem.transform.forward,
                KnockbackValue = (int)(currentDuration * ChargePuchKnockbackPerSecond),
            };

            player.OnHit(hitMessage);
        }

        // After animation is done reset abilities.
        foreach (var ability in _chargePunchAbilitiesToTurnOff)
        {
            _playerController.AbilityToggle(ability, true);
        }
    }

    public void StartPower2 ()
    {
        if(_stoneFormCooldownStartTime + StoneFormCooldown > Time.time || _stoneFormActive)
        {
            // Can't activate yet.
            return;
        }
        _stoneFormActive = true;
        var formingParticlesMain = StoneFormFormingParticleSystem.main;
        formingParticlesMain.duration = StoneFormActivationDelay;
        StoneFormFormingParticleSystem.Play();

        _stoneFormAlreadyHealedAmount = 0;
        _stoneFormStartTime = Time.time;
    }

    public void HandleStoneForm ()
    {
        if(!_stoneFormActive || (_stoneFormStartTime + StoneFormActivationDelay > Time.time))
        {
            return;
        }
        if(RendererForMaterial.material != StoneFormMaterial)
        {
            // * Power just activated!
            RendererForMaterial.material = StoneFormMaterial;
            foreach (var ability in _stoneFormAbilitiesToTurnOff)
            {
                _playerController.AbilityToggle(ability, false);
            }
            // Make player invincible
            _playerController.DamageResistance = 1f;
            _playerController.KnockbackResistance = 1f;
            _animator.StartPlayback();
        }

        var duration = Time.time - _stoneFormStartTime - StoneFormActivationDelay;
        int rightNowHealAmount = (int)Math.Ceiling(duration * StoneFormHealingPerSecond);
        if(rightNowHealAmount - _stoneFormAlreadyHealedAmount > 0)
        {
            StoneFormHealingParticleSystem.Play();
            _playerController.ReduceHitpoints(rightNowHealAmount - _stoneFormAlreadyHealedAmount);
            _stoneFormAlreadyHealedAmount = rightNowHealAmount;
        }


        if(duration > StoneFormMinDuration && (duration > StoneFormMaxDuration || !_inputManager.IsButtonDown(PlayerInputManager.Key.Power2)))
        {
            _animator.StopPlayback();
            RendererForMaterial.material = OriginalMaterial;
            _stoneFormActive = false;
            _stoneFormCooldownStartTime = Time.time;
            StoneFormBreakParticleSystem.Play();
            // Player can get hurt again
            _playerController.DamageResistance = 0f;
            _playerController.KnockbackResistance = 0f;

            foreach (var ability in _stoneFormAbilitiesToTurnOff)
            {
                _playerController.AbilityToggle(ability, true);
            }
        }
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
        HandleStoneForm();
	}
}
