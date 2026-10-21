using UnityEngine;

public class Grass : MonoBehaviour
{
    [SerializeField] private float runTime = 5f;
    [SerializeField] private float runMultiplier = 2f; // The speed will get multiplied with this number
    [SerializeField] private float runSmoothing = 1f;
    [SerializeField] private float fovChange = 20f;
    
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
        _roadObject.player.Sprint(runTime, runSmoothing, runMultiplier, fovChange);
    }
}