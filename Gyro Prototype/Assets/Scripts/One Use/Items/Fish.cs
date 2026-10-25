using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private float time, distance, height;
    [SerializeField] private AnimationCurve distanceCurve, heightCurve;
    
    private RoadObject _roadObject;

    private void Awake()
    {
        _roadObject = GetComponent<RoadObject>();
        if (!_roadObject)
            Debug.LogWarning("Item doesn't have a road object attached");
        else
            _roadObject.onExecute.AddListener(Execute);
    }

    private void Execute()
    {
        PlayerMovement player = _roadObject.player;
        float mul = player.moveAmount / GameManager.Instance.maxSpeed;
        
        player.SuperJump(Mathf.Max(time * mul, 2f), distance * mul, height * mul, distanceCurve, heightCurve);
    }
}
