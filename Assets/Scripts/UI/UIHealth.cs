using UnityEngine;

public class UIHealth : MonoBehaviour
{
    public static UIHealth Instance { get; private set; }

    [SerializeField] private GameObject[] hearts;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void UpdateHearts(int lives)
    {
        if (hearts == null)
            return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].SetActive(i < lives);
        }
    }
}
