# Coding Standards

## Principles
- One class per file.
- Avoid duplicate folders and duplicate script copies.
- Prefer private fields with `[SerializeField]` over public mutable state.
- Keep methods small and focused on one responsibility.
- Use clear names that describe intent.
- Add comments only when they explain why, not what.

## Unity Rules
- Do not create `Folder 1` or `Asset (1)` duplicates in source control.
- Remove orphan `.meta` files only when the asset is deleted on purpose.
- Keep runtime singletons defensive and reset static state for Play Mode.
- Prefer events and small adapters instead of tight coupling between systems.

## Workflow
- Before adding new gameplay systems, search for existing scripts that can be reused.
- Avoid duplicated logic across player, enemy, and UI code.
- Run a build after cleanup or refactor work.
