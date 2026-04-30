using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialDoorChoice : MonoBehaviour
{
    [Header("Door Rule")]
    [SerializeField] private bool isCorrectDoor;

    [Header("Correct Door Action")]
    [SerializeField] private bool loadSceneOnSuccess;
    [SerializeField] private string nextSceneName = "Level_02";
    [SerializeField] private Transform successTeleportTarget;

    [Header("Wrong Door Action")]
    [SerializeField] private Transform wrongDoorReturnPoint;

    [Header("Feedback")]
    [SerializeField] private string successMessage = "Porta correta! Voce avancou.";
    [SerializeField] private string failMessage = "Porta errada! Tente outra.";
    [SerializeField] private Color successColor = new Color(0.4f, 1f, 0.5f);
    [SerializeField] private Color failColor = new Color(1f, 0.45f, 0.45f);
    [SerializeField] private float blockSecondsAfterTrigger = 0.7f;

    private bool blocked;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (blocked || !other.CompareTag("Player"))
            return;

        StartCoroutine(HandleDoor(other.transform));
    }

    private IEnumerator HandleDoor(Transform player)
    {
        blocked = true;

        if (isCorrectDoor)
        {
            if (GameplayFeedbackUI.Instance != null)
            {
                GameplayFeedbackUI.Instance.ShowMessage(successMessage, successColor);
            }

            if (loadSceneOnSuccess && !string.IsNullOrWhiteSpace(nextSceneName))
            {
                yield return new WaitForSeconds(0.35f);
                Time.timeScale = 1f;
                SceneManager.LoadScene(nextSceneName);
                yield break;
            }

            if (successTeleportTarget != null)
            {
                player.position = successTeleportTarget.position;
            }
        }
        else
        {
            if (GameplayFeedbackUI.Instance != null)
            {
                GameplayFeedbackUI.Instance.ShowMessage(failMessage, failColor);
            }

            if (wrongDoorReturnPoint != null)
            {
                player.position = wrongDoorReturnPoint.position;
            }
        }

        yield return new WaitForSeconds(blockSecondsAfterTrigger);
        blocked = false;
    }
}
