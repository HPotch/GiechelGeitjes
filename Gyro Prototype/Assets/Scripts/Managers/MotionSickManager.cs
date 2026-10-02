using System;
using System.Collections.Generic;
using UnityEngine;

public class MotionSickManager : MonoBehaviour
{
    [Serializable]
    public class RWV
    {
        public RotateWithVelocity rwv;
        public float aLittleMultiplier;
        public float aLotMultiplier;
    }
    [Serializable]
    public class AT
    {
        public AnimateTransform at;
        public float aLittleMultiplier;
        public float aLotMultiplier;
    }
    
    [SerializeField] private List<RWV> rotateWithVelocities = new List<RWV>();
    [SerializeField] private List<AT> animateWithTransforms = new List<AT>();

    private void Start()
    {
        switch (GameManager.Instance.motionSickMode)
        {
            case GameManager.MotionSickMode.aLittle:
                foreach (RWV i in rotateWithVelocities) i.rwv.multiplier *= i.aLittleMultiplier;
                foreach (AT i in animateWithTransforms) i.at.multiplier *= i.aLittleMultiplier;
                break;
            case GameManager.MotionSickMode.aLot:
                foreach (RWV i in rotateWithVelocities) i.rwv.multiplier *= i.aLotMultiplier;
                foreach (AT i in animateWithTransforms) i.at.multiplier *= i.aLotMultiplier;
                break;
        }
    }
}
