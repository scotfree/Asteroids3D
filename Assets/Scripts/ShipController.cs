using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 75f;
    [SerializeField] float mouseSensitivity = 0.15f;
    [SerializeField] float thrustForce = 8f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        // Physics won't rotate the ship — we handle rotation manually
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Lock and hide cursor for FPS-style mouse look
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        var kb = Keyboard.current;
        float pitch = 0f, yaw = 0f, roll = 0f;
        if (kb.wKey.isPressed) pitch = -1f;
        if (kb.sKey.isPressed) pitch =  1f;
        if (kb.aKey.isPressed) yaw   = -1f;
        if (kb.dKey.isPressed) yaw   =  1f;
        if (kb.qKey.isPressed) roll  =  1f;
        if (kb.eKey.isPressed) roll  = -1f;

        // Mouse adds to pitch/yaw on top of keyboard
        var mouseDelta = Mouse.current.delta.ReadValue();
        pitch += -mouseDelta.y * mouseSensitivity;
        yaw   +=  mouseDelta.x * mouseSensitivity;

        transform.Rotate(
            pitch * rotationSpeed * Time.deltaTime,
            yaw   * rotationSpeed * Time.deltaTime,
            roll  * rotationSpeed * Time.deltaTime,
            Space.Self
        );
    }

    void FixedUpdate()
    {
        if (Keyboard.current.spaceKey.isPressed)
            rb.AddForce(transform.forward * thrustForce, ForceMode.Acceleration);
    }
}
