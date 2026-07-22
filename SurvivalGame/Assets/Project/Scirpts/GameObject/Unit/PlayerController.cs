using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    [Header("MoveMent")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float moveSpeed = 5f;
    [Header("Mouse Look")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 100f;


    private CharacterController characterController;
    private float verticalVelocity;
    private float cameraPitch;
    
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;
        if(cameraTransform == null)
        {
            Debug.Log("[PlayerController]카메라가 없습니다");
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
    // Update is called once per frame
    void Update()
    {
        Look();
        Move();
        //움직임 구현
        
    }


     private void Move()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * inputX + transform.forward * inputZ;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 플레이어 몸은 좌우로 회전
        transform.Rotate(Vector3.up * mouseX);

        // 카메라는 위아래로 회전
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
}
