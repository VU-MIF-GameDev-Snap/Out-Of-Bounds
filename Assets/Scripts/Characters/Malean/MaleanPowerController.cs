using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaleanPowerController : MonoBehaviour, ICharacterPowerController
{
	private PlayerInputManager _playerInputManager;
	private CharacterController _characterController;
    private PlayerController _playerController;
    private List<GameObject> _characters = new List<GameObject>();
    private bool _strikeHappened = true;

	[Header("Power 1")]
	public float Power1Cooldown;
    public float StrikeVelocity;
    public float StrikeDelay;
    [SerializeField]
    ParticleSystem CyborgStrikeParticleSystem;

    public float StrikeRadius;
    public float Damage;
    public float KnockbackStrength;

	private float _power1TimeStamp = 0;

	[Header("Power2")]
	public GameObject BlackHole;
	public Transform BlackHoleSpawn;
	public float Power2Cooldown;
    

	private float _power2TimeStamp = 0;

	public void StartPower1 ()
	{
		if (_power1TimeStamp > Time.time)
			return;

        if (_characterController.isGrounded)
            return;

        _power1TimeStamp = Time.time + Power1Cooldown;
        _strikeHappened = false;

        var v =_playerController.Velocity;
        if(v.y > 0)
        {
            _playerController.Velocity = new Vector3(v.x, v.y + StrikeVelocity, v.z);
        }
        else
        {
            _playerController.Velocity = new Vector3(v.x, v.y - StrikeVelocity, v.z);
        }
	}

	public void StartPower2 ()
	{
		if (_power2TimeStamp <= Time.time)
		{
			_power2TimeStamp = Time.time + Power2Cooldown;

			var blackHole = Instantiate(BlackHole, BlackHoleSpawn.position, BlackHoleSpawn.rotation);

			blackHole.GetComponent<BlackHole>().Initialize(transform.root.gameObject, _playerInputManager.GetAimDirection());
		}
	}

	void Start ()
	{
		_playerInputManager = GetComponent<PlayerInputManager>();
		_characterController = GetComponent<CharacterController>();
        _playerController = GetComponent<PlayerController>();

        // Get all other characters to follow
		var characters = GameObject.FindGameObjectsWithTag("Player");

		foreach(GameObject c in characters)
		{
			if(c != gameObject)
			{
				_characters.Add(c);
			}
		}
	}

	void Update ()
	{
        // If strike didn't happen in some time dont let it happen
        if(Time.time > _power1TimeStamp - Power1Cooldown + StrikeDelay)
            _strikeHappened = true;
	}

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // If Cyborg Strike not happening
		if(_strikeHappened == true)
			return;

        _strikeHappened = true;

        if(!hit.collider.transform.root.gameObject.CompareTag("Platform"))
            return;

        var myPosition = transform.position;

		// Hit players with explosion if necessary
		foreach(GameObject c in _characters)
		{
			if(c == null || !c.GetComponent<CharacterController>().isGrounded)
				continue;

			var target = c.GetComponent<Collider>().bounds.center;
			var distance = Vector3.Distance(myPosition, target);

			if(distance < StrikeRadius)
			{
				var hitMessage = new HitMessage();

				hitMessage.Damage = (int)(Damage);
				hitMessage.KnockbackValue = (int)(KnockbackStrength);
				hitMessage.KnockbackDirection = (target - myPosition).normalized;

				c.gameObject.GetComponent<PlayerController>().OnHit(hitMessage);
			}
		}

        CyborgStrikeParticleSystem.transform.position = hit.point;
        CyborgStrikeParticleSystem.Play();
    }
}
