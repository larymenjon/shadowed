using UnityEngine;

public class UIHealth : MonoBehaviour
{
    public static UIHealth Instance;

    public GameObject[] hearts;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateHearts(int lives)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < lives);
        }
    }
}


