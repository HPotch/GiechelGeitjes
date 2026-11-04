using System.Collections;
using UnityEngine;
using AudioSettings = AudioManager.AudioSettings;

public class RaceFinishManager : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;
    [SerializeField] private Transform cam1;
    [SerializeField] private Transform cam2;

    [Header("Player 1 Screen")]
    [SerializeField] private GameObject player1WinScreen;
    [SerializeField] private GameObject player1LoseScreen;

    [Header("Player 2 Screen")]
    [SerializeField] private GameObject player2WinScreen;
    [SerializeField] private GameObject player2LoseScreen;

    [Header("Cam Animation")]
    [SerializeField] private Transform camParent;
    [SerializeField] private AnimationCurve camCurve;
    [SerializeField] private float camTime;
    
    [Header("Settings")]
    [SerializeField] private AudioSettings finishAudio;
    [SerializeField] private float showResultsWait = 3f;

    private bool player1Finished;
    private bool player2Finished;

    private Transform winner;

    private void Start()
    {
        // Hide all result screens at the beginning.
        player1WinScreen.SetActive(false);
        player1LoseScreen.SetActive(false);
        player2WinScreen.SetActive(false);
        player2LoseScreen.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform hit = other.transform;

        // Check whether the collider belongs to Player 1.
        if (hit == player1 || hit.IsChildOf(player1))
        {
            if (player1Finished) return;

            player1Finished = true;
            player1.GetComponent<PlayerMovement>().finished = true;
            StartCoroutine(AnimateCamera(cam1));

            if (winner == null)
            {
                AudioManager.Instance.PlayAudioSettings(finishAudio);
                winner = player1;
            }
        }

        // Check whether the collider belongs to Player 2.
        else if (hit == player2 || hit.IsChildOf(player2))
        {
            if (player2Finished) return;

            player2Finished = true;
            player2.GetComponent<PlayerMovement>().finished = true;
            StartCoroutine(AnimateCamera(cam2));

            if (winner == null)
            {
                AudioManager.Instance.PlayAudioSettings(finishAudio);
                winner = player2;
            }
        }
        else
        {
            return;
        }

        // Show results only after both players finish.
        if (player1Finished && player2Finished)
            StartCoroutine(ShowResultsRoutine());
    }

    private IEnumerator AnimateCamera(Transform cam)
    {
        float timer = 0f;
        Vector3 startPos = cam.position;
        Vector3 startRot = cam.eulerAngles;

        cam.parent = camParent;
        cam.GetComponent<Camera>().cullingMask = ~0;

        while (timer < camTime)
        {
            timer += Time.deltaTime;
            float t = camCurve.Evaluate(timer / camTime);
            cam.position = Vector3.Lerp(startPos, camParent.position, t);
            cam.eulerAngles = new Vector3(
                Mathf.LerpAngle(startRot.x, camParent.eulerAngles.x, t),
                Mathf.LerpAngle(startRot.y, camParent.eulerAngles.y, t),
                Mathf.LerpAngle(startRot.z, camParent.eulerAngles.z, t));
            
            yield return null;
        }

        cam.position = camParent.position;
        cam.eulerAngles = camParent.eulerAngles;
    }

    private IEnumerator ShowResultsRoutine()
    {
        yield return new WaitForSeconds(showResultsWait);
        ShowResults();
    }
    
    private void ShowResults()
    {
        if (winner == player1)
        {
            player1WinScreen.SetActive(true);
            player2LoseScreen.SetActive(true);
        }
        else
        {
            player2WinScreen.SetActive(true);
            player1LoseScreen.SetActive(true);
        }

        // Stop both players from continuing to race.
        Time.timeScale = 0f;
    }
}
