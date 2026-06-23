using UnityEngine;
using UnityEngine.UI;

public class UIHUD : MonoBehaviour
{
    [SerializeField] private Text coinsText;

    private void Update()
    {
        if (coinsText == null || GameManager.Instance == null)
            return;

        coinsText.text = GameManager.Instance.Coins.ToString();
    }
}
