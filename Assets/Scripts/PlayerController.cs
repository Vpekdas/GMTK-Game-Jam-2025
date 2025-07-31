using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 12.0f;
    [SerializeField] private Camera _camera;
    private Rigidbody _rb;
    private PlayerInput _playerInput;
    private Vector2 _input;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        Vector3 camForward = _camera.transform.forward;
        Vector3 camRight = _camera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = camForward * _input.y + camRight * _input.x;
        _rb.linearVelocity = moveDirection.normalized * _speed + new Vector3(0, _rb.linearVelocity.y, 0);
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }
}
