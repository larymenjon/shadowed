using UnityEngine;

public class StepPlatform : MonoBehaviour
{
    [Header("Regras")]
    public int stepsToAppear = 6;
    public int jumpsToDisappear = 5;

    private SpriteRenderer sprite;
    private Collider2D col;

    // Alterado para true para começar aparecendo
    private bool appeared = true;

    private int stepCheckpoint;
    private int jumpCheckpoint;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Se começa aparecendo, o checkpoint que importa agora é o de pulos
        if (PlayerStepCounter.instance != null)
        {
            jumpCheckpoint = PlayerStepCounter.instance.jumps;
        }
        
        ShowPlatform(); // Garante que comece visível
    }

    void Update()
    {
        if (PlayerStepCounter.instance == null) return;

        // QUANTOS PASSOS DESDE QUE A PLATAFORMA SUMIU
        int stepsSinceHide = Mathf.Max(0, PlayerStepCounter.instance.steps - stepCheckpoint);

        // QUANTOS PULOS DESDE QUE A PLATAFORMA APARECEU
        int jumpsSinceShow = Mathf.Max(0, PlayerStepCounter.instance.jumps - jumpCheckpoint);

        // APARECER
        if (!appeared && stepsSinceHide >= stepsToAppear)
        {
            appeared = true;
            jumpCheckpoint = PlayerStepCounter.instance.jumps;

            Debug.Log("Plataforma APARECEU | passos desde sumir: " + stepsSinceHide);

            ShowPlatform();
        }

        // DESAPARECER
        if (appeared && jumpsSinceShow >= jumpsToDisappear)
        {
            Debug.Log("Plataforma SUMIU | pulos desde aparecer: " + jumpsSinceShow);

            appeared = false;
            stepCheckpoint = PlayerStepCounter.instance.steps;

            HidePlatform();
        }
    }

    void ShowPlatform()
    {
        sprite.enabled = true;
        col.enabled = true;
    }

    void HidePlatform()
    {
        sprite.enabled = false;
        col.enabled = false;
    }
}