
using UnityEngine;

public class RaceFinishManager : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    [Header("Player 1 Screen")]
    [SerializeField] private GameObject player1WinScreen;
    [SerializeField] private GameObject player1LoseScreen;

    [Header("Player 2 Screen")]
    [SerializeField] private GameObject player2WinScreen;
    [SerializeField] private GameObject player2LoseScreen;

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

            if (winner == null)
                winner = player1;
        }

        // Check whether the collider belongs to Player 2.
        else if (hit == player2 || hit.IsChildOf(player2))
        {
            if (player2Finished) return;

            player2Finished = true;

            if (winner == null)
                winner = player2;
        }
        else
        {
            return;
        }

        // Show results only after both players finish.
        if (player1Finished && player2Finished)
        {
            ShowResults();
        }
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
