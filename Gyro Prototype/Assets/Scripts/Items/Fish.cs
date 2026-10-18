using UnityEngine;

public class Fish : MonoBehaviour
{
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
        print("Fish");
    }
}
