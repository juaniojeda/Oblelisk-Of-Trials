using UnityEngine;
using Photon.Pun;   // <- PUN
using Photon.Realtime;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PhotonView))]
public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Movimiento")]
    [Tooltip("Velocidad base en m/s")]
    public float moveSpeed = 5f;
    [Tooltip("Multiplicador al mantener Shift")]
    public float sprintMultiplier = 1.5f;

    [Header("Salto / Gravedad")]
    [Tooltip("Altura del salto en metros")]
    public float jumpHeight = 1.5f;
    [Tooltip("Gravedad (negativa)")]
    public float gravity = -9.81f;
    [Tooltip("Fuerza hacia abajo para mantener el contacto con el suelo")]
    public float groundedStickForce = -2f;

    [Header("Mouse Look")]
    [Tooltip("Transform que rota solo en X (pitch). Suele ser un vacío que contiene la cámara")]
    public Transform cameraHolder;
    [Tooltip("Sensibilidad del mouse")]
    public float mouseSensitivity = 2f;
    [Tooltip("Límite de mirada vertical (grados)")]
    public float verticalLookLimit = 80f;
    [Tooltip("Bloquear el cursor al iniciar (solo local)")]
    public bool lockCursorOnStart = true;

    [Header("Componentes opcionales (se desactivan en remotos)")]
    public Camera playerCamera;          // si la cámara está como hijo
    public AudioListener audioListener;  // si hay uno en la cámara

    private CharacterController controller;
    private Vector3 velocity;     // Y para salto/gravedad
    private float pitch;          // rotación vertical acumulada

    private bool IsGrounded => controller.isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Autoconfigura referencias si faltan
        if (playerCamera == null && cameraHolder != null)
            playerCamera = cameraHolder.GetComponentInChildren<Camera>();
        if (audioListener == null && playerCamera != null)
            audioListener = playerCamera.GetComponent<AudioListener>();

        // Si NO es nuestro, ponemos en modo réplica:
        if (!photonView.IsMine)
        {
            // Desactiva cámara y audio en réplicas para evitar dobles cámaras/sonido
            if (playerCamera) playerCamera.enabled = false;
            if (audioListener) audioListener.enabled = false;

            // No leeremos input ni moveremos este CharacterController
            // (el movimiento vendrá sincronizado por red)
            enabled = true; // el script queda activo, pero no hará nada en Update (ver chequeo al inicio)
        }
    }

    private void Start()
    {
        if (!photonView.IsMine) return;

        SetCursorLock(lockCursorOnStart);

        if (cameraHolder != null)
        {
            pitch = cameraHolder.localEulerAngles.x;
            if (pitch > 180f) pitch -= 360f;
            pitch = Mathf.Clamp(pitch, -verticalLookLimit, verticalLookLimit);
            ApplyCameraPitch();
        }
    }

    private void Update()
    {
        // *** Solo el dueño controla ***
        if (!photonView.IsMine) return;

        HandleMouseLook();
        HandleMovement();
        HandleJump();
    }

    private void HandleMouseLook()
    {
        if (cameraHolder == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Yaw en el cuerpo
        transform.Rotate(Vector3.up * mouseX);

        // Pitch en el holder
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -verticalLookLimit, verticalLookLimit);
        ApplyCameraPitch();

        if (Input.GetKeyDown(KeyCode.Escape))
            SetCursorLock(Cursor.lockState != CursorLockMode.Locked);
    }

    private void ApplyCameraPitch()
    {
        Vector3 e = cameraHolder.localEulerAngles;
        e.x = pitch; e.y = 0f; e.z = 0f;
        cameraHolder.localEulerAngles = e;
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(moveX, 0f, moveZ);
        input = Vector3.ClampMagnitude(input, 1f);

        Vector3 move = transform.TransformDirection(input);
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);

        controller.Move(move * speed * Time.deltaTime);

        if (IsGrounded && velocity.y < 0f) velocity.y = groundedStickForce;
        else velocity.y += gravity * Time.deltaTime;

        controller.Move(Vector3.up * velocity.y * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (!IsGrounded) return;

        if (Input.GetButtonDown("Jump"))
            velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
    }

    private void SetCursorLock(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.1f, transform.forward * 1.5f);
    }
}
