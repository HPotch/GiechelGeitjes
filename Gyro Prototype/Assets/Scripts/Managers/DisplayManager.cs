using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Displays connected:" + Display.displays.Length);
        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }
}
