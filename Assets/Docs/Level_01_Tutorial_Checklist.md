# Level_01 Tutorial Checklist

## Flow
- [ ] Cena `MainMenu` com botao `StartGame` chamando `MainMenuUI.StartGame()`.
- [ ] Cena `LoginFake` criada e adicionada no Build Settings antes de `Level_01`.
- [ ] Cena `Level_01` abre com overlay preto `LEVEL 1` e fade out.
- [ ] Dialogo inicial aparece logo apos o fade e pode ser avancado com `ESPACO` ou clique.

## LoginFake Scene
- [ ] Canvas com `Image` principal para slideshow.
- [ ] Canvas com `Slider` (0 a 1) para barra fake de login.
- [ ] GameObject `LoginFlowManager` com script `FakeLoginFlow`.
- [ ] Campo `Slides` preenchido com 5 sprites.
- [ ] Campo `Next Scene Name` = `Level_01`.

## Level_01 Intro + Tutorial UI
- [ ] Canvas overlay preto com `CanvasGroup`.
- [ ] Texto central `LEVEL 1`.
- [ ] GameObject `LevelIntroManager` com script `LevelIntroOverlay`.
- [ ] Campo `Disable During Intro` contendo ao menos `PlayerController`.
- [ ] Painel de dialogo `DialogRoot` com texto principal e hint.
- [ ] GameObject com script `TutorialDialogController` ligado ao `DialogRoot`.

## Door Choice System (3 portas)
- [ ] Criar/usar 3 portas com trigger collider (`Is Trigger` = true).
- [ ] Adicionar `TutorialDoorChoice` nas 3 portas.
- [ ] Apenas 1 porta com `Is Correct Door` = true.
- [ ] Porta correta:
- [ ] `Load Scene On Success` habilitado e `Next Scene Name` configurado (ex: `Level_02`), ou `Success Teleport Target`.
- [ ] Portas erradas:
- [ ] `Is Correct Door` = false.
- [ ] `Wrong Door Return Point` preenchido (ponto seguro de retorno).
- [ ] Na UI da cena: `GameplayFeedbackUI` configurado com `CanvasGroup + Text`.

## Spawn Automatico de Vampiros
- [ ] GameObject `EnemySpawner` com script `LevelEnemyAutoSpawner`.
- [ ] Config `Level Configs` preenchida:
- [ ] `Level_01` -> prefab vampiro -> `Enemy Count = 1`.
- [ ] `Level_02` -> prefab vampiro -> `Enemy Count` maior.
- [ ] `Level_03` -> prefab vampiro -> `Enemy Count` ainda maior.
- [ ] `Spawn Points` preenchidos com pontos do mapa (ou usar fallback area).
- [ ] `Destroy Existing Enemies On Start` marcado se quiser limpeza automatica.

## Build Settings
- [ ] Ordem recomendada:
- [ ] `MainMenu`
- [ ] `LoginFake`
- [ ] `Level_01`
- [ ] `Level_02`
- [ ] `Level_03`
- [ ] `PassGame`
- [ ] `EndGame`
- [ ] `GameOver`
