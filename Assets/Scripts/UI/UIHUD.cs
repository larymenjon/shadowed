using UnityEngine;
using UnityEngine.UI;

public class UIHUD : MonoBehaviour
{
    public Text coinsText;

    private void Update()
    {
        if (GameManager.Instance == null) return;

        coinsText.text = GameManager.Instance.coins.ToString();
    }
}

