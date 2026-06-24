using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float parallaxEffect = 0.5f;

    private float startX;

    private void Awake()
    {
        startX = transform.position.x;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if (cameraTransform == null)
            return;

        float distance = cameraTransform.position.x * parallaxEffect;
        transform.position = new Vector3(startX + distance, transform.position.y, transform.position.z);
    }
}
