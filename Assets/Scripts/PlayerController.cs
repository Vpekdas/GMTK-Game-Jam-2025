using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static UnityEvent explosionEvent = new();

    [SerializeField] private float _speed = 7.0f;
    [SerializeField] private float _lookSpeedH = 3.0f;
    [SerializeField] private float _lookSpeedV = 3.0f;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _head;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform _pistol;

    [SerializeField] private float _explosionShakeTime = 0.2f;
    [SerializeField] private AnimationCurve _explosionShakeCurve;

    private Rigidbody _rb;
    private Vector2 _input;
    private float _yaw = 0.0f;
    private float _pitch = 0.0f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        explosionEvent.AddListener(ShakeScreen);
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        Vector3 camForward = _camera.transform.forward;
        Vector3 camRight = _camera.transform.right;
        // Y = 0 ensure that we are not moving slower if we look above or below.
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = camForward * _input.y + camRight * _input.x;
        _rb.linearVelocity = moveDirection.normalized * _speed + new Vector3(0, _rb.linearVelocity.y, 0);
    }

    private void LateUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            _head.localEulerAngles = new Vector3(_pitch, _head.localEulerAngles.y, 0.0f);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, _yaw, 0.0f);
        }
    }

    public void Kill()
    {
        // Debug.Log("Player is ded :(");
    }

    public void ShakeScreen()
    {
        StartCoroutine(ShakeShakeShake());
    }

    private IEnumerator ShakeShakeShake()
    {
        Vector3 startPosition = transform.position;
        float elaspedTime = 0.0f;

        while (elaspedTime < _explosionShakeTime)
        {
            elaspedTime += Time.deltaTime;
            float strength = _explosionShakeCurve.Evaluate(elaspedTime / _explosionShakeTime);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }

    public void MouseInput(InputAction.CallbackContext context)
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 value = context.ReadValue<Vector2>();
            _yaw += _lookSpeedH * value.x * 0.1f;
            _pitch -= _lookSpeedV * value.y * 0.1f;
        }
    }

    public void LockMouse(InputAction.CallbackContext context)
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        GameObject bullet = Instantiate(_bullet, _pistol.transform.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Bullet b = bullet.GetComponent<Bullet>();

        rb.AddForce(_head.transform.forward * 8.0f, ForceMode.Force);
        rb.excludeLayers |= 1 << LayerMask.NameToLayer("Player");
        b.SetTrailColor(Color.white);

        _pistol.GetComponent<AudioSource>().Play();
    }

    public void UnlockMouse(InputAction.CallbackContext context)
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
