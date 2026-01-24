using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PassGameController : MonoBehaviour
{
    public float waitTime = 2.5f;

    private void Start()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadNext());
    }

    IEnumerator LoadNext()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(GameManager.Instance.nextLevel);
    }
}
