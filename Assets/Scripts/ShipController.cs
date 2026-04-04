using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    [SerializeField] float rotationSpeed   = 75f;
    [SerializeField] float mouseSensitivity = 0.15f;
    [SerializeField] float thrustForce     = 8f;
    [SerializeField] float shotSpeed       = 40f;
    [SerializeField] float shotCooldown    = 1f;

    Rigidbody rb;
    Camera    cam;
    float     lastFireTime = -999f;

    enum ViewMode { ThirdPerson, FirstPerson }
    ViewMode currentView = ViewMode.ThirdPerson;

    GameObject crosshairCanvas;

    void Awake()
    {
        rb  = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();

        rb.useGravity       = false;
        rb.linearDamping    = 0f;
        rb.angularDamping   = 0f;
        rb.constraints      = RigidbodyConstraints.FreezeRotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        var col = gameObject.AddComponent<SphereCollider>();
        col.radius    = 0.5f;
        col.isTrigger = true;

        BuildShipMesh();
        SetThirdPersonCamera();
        BuildCrosshair();
    }

    // ── View toggle ──────────────────────────────────────────────────────────

    void ToggleView()
    {
        if (currentView == ViewMode.ThirdPerson)
        {
            currentView = ViewMode.FirstPerson;
            // Camera sits just ahead of the nose, looking straight along +Z
            cam.transform.localPosition = new Vector3(0f, 0f, 1.6f);
            cam.transform.localRotation = Quaternion.identity;
            var dbg = GetComponent<ShipDebugVisuals>();
            if (dbg) dbg.enabled = false;
            crosshairCanvas.SetActive(true);
        }
        else
        {
            currentView = ViewMode.ThirdPerson;
            SetThirdPersonCamera();
            var dbg = GetComponent<ShipDebugVisuals>();
            if (dbg) dbg.enabled = true;
            crosshairCanvas.SetActive(false);
        }
    }

    void SetThirdPersonCamera()
    {
        if (cam == null) return;
        cam.transform.localPosition = new Vector3(0f, 6f, -9f);
        cam.transform.localRotation = Quaternion.LookRotation(new Vector3(0f, -6f, 9f).normalized);
    }

    // ── Crosshair (first-person only) ────────────────────────────────────────

    void BuildCrosshair()
    {
        crosshairCanvas = new GameObject("CrosshairCanvas");
        var canvas = crosshairCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        // No CanvasScaler — crosshair pixel size stays constant at any resolution

        MakeCrosshairBar(crosshairCanvas, new Vector2(20f, 2f)); // horizontal
        MakeCrosshairBar(crosshairCanvas, new Vector2(2f, 20f)); // vertical

        crosshairCanvas.SetActive(false); // hidden until first-person mode
    }

    void MakeCrosshairBar(GameObject parent, Vector2 size)
    {
        var go  = new GameObject("Bar");
        go.transform.SetParent(parent.transform, false);
        var img = go.AddComponent<Image>();
        img.color = Color.white;
        var r = go.GetComponent<RectTransform>();
        r.anchorMin        = new Vector2(0.5f, 0.5f);
        r.anchorMax        = new Vector2(0.5f, 0.5f);
        r.pivot            = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = Vector2.zero;
        r.sizeDelta        = size;
    }

    // ── Ship mesh ────────────────────────────────────────────────────────────

    void BuildShipMesh()
    {
        SpawnConeChild("ShipBody", new Vector3(0f, 0f, -1.5f), length: 3f,    baseRadius: 0.8f,  includeCap: true,  color: new Color(0.6f, 0.75f, 1f));
        SpawnConeChild("ShipTip",  new Vector3(0f, 0f,  0.75f), length: 0.75f, baseRadius: 0.21f, includeCap: false, color: Color.red);
    }

    Mesh BuildConeMesh(float length, float baseRadius, int segments, bool includeCap)
    {
        int baseStart = includeCap ? 2 : 1;
        var verts = new Vector3[baseStart + segments];
        var tris  = new int[segments * (includeCap ? 6 : 3)];

        verts[0] = new Vector3(0, 0, length);
        if (includeCap) verts[1] = Vector3.zero;

        for (int i = 0; i < segments; i++)
        {
            float a = i * Mathf.PI * 2f / segments;
            verts[baseStart + i] = new Vector3(Mathf.Cos(a) * baseRadius, Mathf.Sin(a) * baseRadius, 0);
        }

        int t = 0;
        for (int i = 0; i < segments; i++)
        {
            int curr = baseStart + i;
            int next = baseStart + (i + 1) % segments;
            tris[t++] = 0; tris[t++] = curr; tris[t++] = next;
            if (includeCap) { tris[t++] = 1; tris[t++] = next; tris[t++] = curr; }
        }

        var mesh = new Mesh();
        mesh.vertices  = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        return mesh;
    }

    void SpawnConeChild(string name, Vector3 localPos, float length, float baseRadius, bool includeCap, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        go.transform.localPosition = localPos;
        go.AddComponent<MeshFilter>().mesh = BuildConeMesh(length, baseRadius, 12, includeCap);
        var mr  = go.AddComponent<MeshRenderer>();
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetColor("_BaseColor", color);
        mat.SetFloat("_Cull", 0);
        mr.material = mat;
    }

    // ── Input ────────────────────────────────────────────────────────────────

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Asteroid>() != null)
            GameManager.Instance?.GameOver();
    }

    void Update()
    {
        var kb    = Keyboard.current;
        var mouse = Mouse.current;

        // Rotation
        float pitch = 0f, yaw = 0f, roll = 0f;
        if (kb.wKey.isPressed) pitch = -1f;
        if (kb.sKey.isPressed) pitch =  1f;
        if (kb.aKey.isPressed) yaw   = -1f;
        if (kb.dKey.isPressed) yaw   =  1f;
        if (kb.qKey.isPressed) roll  =  1f;
        if (kb.eKey.isPressed) roll  = -1f;

        var mouseDelta = mouse.delta.ReadValue();
        pitch += -mouseDelta.y * mouseSensitivity;
        yaw   +=  mouseDelta.x * mouseSensitivity;

        transform.Rotate(
            pitch * rotationSpeed * Time.deltaTime,
            yaw   * rotationSpeed * Time.deltaTime,
            roll  * rotationSpeed * Time.deltaTime,
            Space.Self
        );

        // Fire — left shift, right shift, or left mouse button
        bool wantsToFire = kb.leftShiftKey.isPressed
                        || kb.rightShiftKey.isPressed
                        || mouse.leftButton.isPressed;
        if (wantsToFire && Time.time - lastFireTime >= shotCooldown)
        {
            Fire();
            lastFireTime = Time.time;
        }

        // View toggle — TAB is intercepted by the Unity editor UI; use V instead
        if (kb.vKey.wasPressedThisFrame)
            ToggleView();
    }

    void FixedUpdate()
    {
        if (Keyboard.current.spaceKey.isPressed)
            rb.AddForce(transform.forward * thrustForce, ForceMode.Acceleration);
    }

    // ── Firing ───────────────────────────────────────────────────────────────

    void Fire()
    {
        Vector3 spawnPos = transform.TransformPoint(new Vector3(0f, 0f, 2f));

        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Shot";
        go.transform.position   = spawnPos;
        go.transform.localScale = Vector3.one * 0.4f;

        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetColor("_BaseColor", Color.red);
        go.GetComponent<Renderer>().material = mat;

        go.GetComponent<SphereCollider>().isTrigger = true;

        var shotRb = go.AddComponent<Rigidbody>();
        shotRb.useGravity     = false;
        shotRb.linearDamping  = 0f;
        shotRb.angularDamping = 0f;
        shotRb.linearVelocity = rb.linearVelocity + transform.forward * shotSpeed;

        go.AddComponent<Projectile>();
    }
}
