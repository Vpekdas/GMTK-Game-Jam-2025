using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private float _speedH;
    [SerializeField] private float _speedV;

    private float _yaw = 0.0f;
    private float _pitch = 0.0f;

    private void Start()
    {
    }

    private void LateUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            transform.eulerAngles = new Vector3(_pitch, _yaw, 0.0f);
        }
    }

    public void MouseInput(InputAction.CallbackContext context)
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 value = context.ReadValue<Vector2>();
            _yaw += _speedH * value.x * 0.1f;
            _pitch -= _speedV * value.y * 0.1f;
        }
    }

    public void LockMouse(InputAction.CallbackContext context)
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockMouse(InputAction.CallbackContext context)
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
