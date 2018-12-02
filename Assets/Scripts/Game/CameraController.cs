using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public float DampTime = 0.2f;
	public float ScreenEdgeBuffer = 4f;
	public float MinSize = 6.5f;

	private ICollection<Transform> _targets;
	private Camera _camera;
	private float _zoomSpeed;
	private Vector3 _moveVelocity;
	private Vector3 _desiredPosition;

	private void Awake ()
	{
		_camera = GetComponent<Camera>();
	}

	private void FixedUpdate ()
	{
		// Could possibly update players at rarer intervals..
		SetTargets();

		Move();
		Zoom();
	}

	private void SetTargets ()
	{
		_targets = GameObject.FindGameObjectsWithTag("Player").Select((p) =>
		{
			return p.transform;
		}).ToList();
	}

	private void Move ()
	{
		FindAveragePosition();

		transform.position = Vector3.SmoothDamp(transform.position, _desiredPosition, ref _moveVelocity, DampTime);
	}


	private void FindAveragePosition ()
	{
		Vector3 averagePos = new Vector3();
		int numTargets = 0;

		foreach (var target in _targets)
		{
			if (!target.gameObject.activeSelf)
				continue;

			averagePos += new Vector3(target.transform.position.x, target.transform.position.y + target.GetComponent<Collider>().bounds.size.y, target.transform.position.z );
			numTargets++;
		}

		if (numTargets > 0)
			averagePos /= numTargets;

		// Move camera up relative to players avearage osition.
		averagePos -= Vector3.Distance(averagePos, transform.position) * transform.forward;

		averagePos.z = transform.position.z;

		_desiredPosition = averagePos;
	}


	private void Zoom ()
	{
		float requiredSize = FindRequiredSize();
		_camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, requiredSize, ref _zoomSpeed, DampTime);
	}


	private float FindRequiredSize ()
	{
		Vector3 desiredLocalPos = transform.InverseTransformPoint(_desiredPosition);

		float size = 0f;

		foreach (var target in _targets)
		{
			if (!target.gameObject.activeSelf)
				continue;

			Vector3 targetLocalPos = transform.InverseTransformPoint(target.position);

			Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

			size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
			size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / _camera.aspect);
		}

		size += ScreenEdgeBuffer;

		size = Mathf.Max(size, MinSize);

		return size;
	}


	public void SetStartPositionAndSize ()
	{
		FindAveragePosition();

		transform.position = _desiredPosition;

		_camera.orthographicSize = FindRequiredSize();
	}
}
