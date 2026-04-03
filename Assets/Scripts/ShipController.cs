using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 75f;
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
    }

    void Update()
    {
        var kb = Keyboard.current;
        float pitch = 0f, yaw = 0f;
        if (kb.upArrowKey.isPressed)    pitch = -1f;
        if (kb.downArrowKey.isPressed)  pitch =  1f;
        if (kb.leftArrowKey.isPressed)  yaw   = -1f;
        if (kb.rightArrowKey.isPressed) yaw   =  1f;

        transform.Rotate(
            pitch * rotationSpeed * Time.deltaTime,
            yaw   * rotationSpeed * Time.deltaTime,
            0f,
            Space.Self
        );
    }

    void FixedUpdate()
    {
        if (Keyboard.current.spaceKey.isPressed)
            rb.AddForce(transform.forward * thrustForce, ForceMode.Acceleration);
    }
}
