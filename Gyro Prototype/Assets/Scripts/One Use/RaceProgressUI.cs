using UnityEngine;
using UnityEngine.UI;

public class RaceProgressUI : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    [Header("Race Settings")]
    [SerializeField] private float startZ = 0f;
    [SerializeField] private float finishZ = 500f;

    [Header("UI Icons")]
    [SerializeField] private RectTransform player1Icon;
    [SerializeField] private RectTransform player2Icon;
    [SerializeField] private RectTransform progressBar;

    private void Update()
    {
        if (player1 == null || player2 == null ||
            player1Icon == null || player2Icon == null ||
            progressBar == null)
            return;

        float player1Progress = GetProgress(player1);
        float player2Progress = GetProgress(player2);

        UpdateIcon(player1Icon, player1Progress);
        UpdateIcon(player2Icon, player2Progress);
    }

    private float GetProgress(Transform player)
    {
        return Mathf.Clamp01(
            (player.position.z - startZ) /
            (finishZ - startZ)
        );
    }

    private void UpdateIcon(RectTransform icon, float progress)
    {
        float halfWidth = progressBar.rect.width / 2f;

        Vector2 position = icon.anchoredPosition;
        position.x = Mathf.Lerp(
            -halfWidth,
            halfWidth,
            progress
        );

        icon.anchoredPosition = position;
    }
}
