using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	private enum MoveMode { viking, goat }

	private enum GoatPhoneMode { vertical, horizontal }

	[SerializeField] [Tooltip("Units per second")] private float speed = 2f;
    [SerializeField] [Tooltip("This curve decides tilt speed, based on the GameManagers max speed")] private AnimationCurve tiltCurve;
    [SerializeField] [Tooltip("The player gets pushed back if they are this close to the bounds")] private float pushBackDistance = 1f;
    [SerializeField] [Tooltip("The layermask used for detecting if the player can move")] private LayerMask layerMask;

	[Header("Viking")]
	[SerializeField] private float vikingMotionRequired = 10f; // In degrees
	[Tooltip("The viking's gyro must go from top to bottom in this time for a move to occur")][SerializeField] private float vikingSpeedRequired = 0.7f; // In seconds
	[SerializeField] private float vikingForce = 0.1f;
	[SerializeField] private float vikingTimeInfluence = 5f;
	[SerializeField] private float velocityDrag = 5f;

	[Header("Goat")]
	[SerializeField] private float goatRotateSensitivity = 3f;
	[SerializeField] private GoatPhoneMode goatPhoneMode;

	[Header("Phone Functions")]
	[SerializeField] private MoveMode phone1Mode;
	[SerializeField] private MoveMode phone2Mode;
	[SerializeField] private int group = 1;

	public UnityEvent afterMove;

    private float _disableTime = 0f;
	private float _steeringDirection;
	private readonly Dictionary<float, float> _lastVikingXRotations = new Dictionary<float, float>();

	public void SetDisableTime(float time)
    {
        _disableTime = time;
    }
    
    private void Update()
    { 
        _disableTime -= Time.deltaTime;
        if (_disableTime > 0f) return;

		ReadGyroscope();

		LeftRight();
        Forward();
        MoveForward();
    }

	private void ReadGyroscope()
	{
		GyroReceiver gr = GyroReceiver.Instance;
		if (gr == null)
			return;

		// Select the correct phone's gyro data.
		Vector3 gyro = phone1Mode == MoveMode.viking ? gr.gyro1Values[group] : gr.gyro2Values[group];

		if (phone1Mode == MoveMode.viking)
			VikingMove(gyro);
		else
			GoatMove(gyro);
	}

	private void VikingMove(Vector3 dir)
	{
		_lastVikingXRotations.Add(Time.time, dir.x);

		float highestValue = -Mathf.Infinity;
		float highestValueTime = 0f;
		for (int i = 0; i < _lastVikingXRotations.Count; i++)
		{
			// Remove pairs that have expired
			float time = _lastVikingXRotations.Keys.ToArray()[i];
			if (time < Time.time - vikingSpeedRequired)
			{
				_lastVikingXRotations.Remove(time);
				continue;
			}
			// Get highest value
			float value = _lastVikingXRotations[time];
			if (value <= highestValue) continue;
			highestValue = value;
			highestValueTime = time;
		}

		if (!(dir.x < highestValue - vikingMotionRequired)) return;
		// The gyroscope has moved down the sufficient amount to initiate a move
		float force = vikingForce * Mathf.Lerp(vikingTimeInfluence, 1f, (Time.time - highestValueTime) / vikingSpeedRequired);
		print("Delta: " + (Time.time - highestValueTime));
		print("T: " + ((Time.time - highestValueTime) / vikingSpeedRequired));
		print("SpeedInfluence: " + Mathf.Lerp(vikingTimeInfluence, 1f, (Time.time - highestValueTime) / vikingSpeedRequired));
		// A successful gyro movement increases the game speed.
		GameManager.Instance.Move(force);
		_lastVikingXRotations.Clear(); // Clear the list so we don't initiate another move on te same values
	}

	private void GoatMove(Vector3 dir)
	{
		_steeringDirection = (dir.y - (goatPhoneMode == GoatPhoneMode.horizontal ? 45f : 0f)) * goatRotateSensitivity;
	}

	private void LeftRight()
    {
        float dir = 0f;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            dir = -1;
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            dir = 1;

        float distToLeftBound = GameManager.Instance.bounds - transform.position.x * -1f;
        dir += Mathf.Clamp01(pushBackDistance - distToLeftBound);
        float distToRightBound = GameManager.Instance.bounds - transform.position.x;
        dir -= Mathf.Clamp01(pushBackDistance - distToRightBound);

        float tiltSpeed = tiltCurve.Evaluate(Mathf.Clamp01(GameManager.Instance.moveAmount / GameManager.Instance.maxSpeed));
        
        float velocity = dir * Time.deltaTime * speed * tiltSpeed;

        transform.position += new Vector3(velocity, 0f, 0f);
        afterMove.Invoke();
    }

    private void Forward()
    {
        if (!(Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)) return;
        if (CastRay()) return;

		GameManager.Instance.Move(1f);
	}

    private void MoveForward()
    {
		float currentSpeed = GameManager.Instance.moveAmount;

		transform.position +=
			transform.forward * currentSpeed * Time.deltaTime;
	}

    private bool CastRay()
    {
        return Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), 1f, layerMask);
    }
}
