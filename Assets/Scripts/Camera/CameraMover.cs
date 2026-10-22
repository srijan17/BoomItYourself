using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private InputActionReference scrollAction;

    [SerializeField] private float scrollSensitivity = 0.002f;
    [SerializeField] private float transitionSpeed = 2f;

    public Vector3 tvLocation = new(-1.6f, 7.7f, -18.2f);
    public Vector3 tableLocation = new(-1.6f, 17.29f, -13.19f);

    public Quaternion tvRotation = Quaternion.Euler(10.586f, -0.688f, 0f);
    public Quaternion tableRotation = Quaternion.Euler(44.722f, -0.688f, 0f);

    private float targetT = 0f;
    private float currentT = 0f;

    private void Awake()
    {
        // Initialize based on the camera's current position
        Vector3 direction = tableLocation - tvLocation;

        currentT = Mathf.Clamp01(
            Vector3.Dot(transform.position - tvLocation, direction)
            / direction.sqrMagnitude
        );

        targetT = currentT;
    }

    private void OnEnable()
    {
        scrollAction.action.Enable();
        scrollAction.action.performed += OnScroll;
    }

    private void OnDisable()
    {
        scrollAction.action.performed -= OnScroll;
        scrollAction.action.Disable();
    }

    private void OnScroll(InputAction.CallbackContext ctx)
    {
        float scroll = ctx.ReadValue<Vector2>().y;

        targetT = Mathf.Clamp01(
            targetT + scroll * scrollSensitivity
        );
    }

    private void Update()
    {
        // Smoothly move toward target
        currentT = Mathf.MoveTowards(
            currentT,
            targetT,
            transitionSpeed * Time.deltaTime
        );

        transform.position = Vector3.Lerp(
            tvLocation,
            tableLocation,
            currentT
        );

        transform.rotation = Quaternion.Slerp(
            tvRotation,
            tableRotation,
            currentT
        );
    }
}