using UnityEngine;

public class ItemSway : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerManager manager;
    [Header("Sway Settings")]
    public float swayAmount = 0.05f;
    public float maxSwayAmount = 0.1f;
    public float swaySmooth = 8f;

    private Vector3 initialPosition;

    void Start()
    {
        movement = GetComponentInParent<PlayerMovement>();
        manager = GetComponentInParent<PlayerManager>();
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (!movement.isGrounded || !manager.mouseLocked) return;

        Vector3 swayOffset = CalculateSway();

        // Combine bob + sway + base position
        Vector3 targetPosition = initialPosition + swayOffset;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * swaySmooth);
    }

    Vector3 CalculateSway()
    {
        float mouseX = -Input.GetAxis("Mouse X") * swayAmount;
        float mouseY = -Input.GetAxis("Mouse Y") * swayAmount;

        mouseX = Mathf.Clamp(mouseX, -maxSwayAmount, maxSwayAmount);
        mouseY = Mathf.Clamp(mouseY, -maxSwayAmount, maxSwayAmount);

        return new Vector3(mouseX, mouseY, 0f);
    }
}
