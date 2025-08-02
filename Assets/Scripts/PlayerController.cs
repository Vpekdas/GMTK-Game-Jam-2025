using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static UnityEvent explosionEvent = new();

    [SerializeField] private float _speed = 7.0f;
    [SerializeField] private float _lookSpeedH = 3.0f;
    [SerializeField] private float _lookSpeedV = 3.0f;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _head;
    [SerializeField] private GameObject _bullet, _enemy;
    [SerializeField] private Transform _pistol;

    [SerializeField] private float _explosionShakeTime = 0.2f;
    [SerializeField] private AnimationCurve _explosionShakeCurve;
    [SerializeField] private int _enemyToSpawn;
    [SerializeField] private MapGenerator _map;
    [SerializeField] GameManager _gameManager;

    private Rigidbody _rb;
    private Vector2 _input;
    private float _yaw = 0.0f;
    private float _pitch = 0.0f;
    private float _time;
    private float _lastBulletTime;
    private float _bulletCooldown = 0.4f;
    private readonly int _roomLength = 30, _wallHeight = 16;
    private bool _isGrounded;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        explosionEvent.AddListener(ShakeScreen);
        _isGrounded = true;
    }

    private void Start()
    {
    }

    private void Update()
    {
        _time += Time.deltaTime;
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
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void ShakeScreen()
    {
        StartCoroutine(ShakeShakeShake());
    }

    private IEnumerator ShakeShakeShake()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0.0f;

        while (elapsedTime < _explosionShakeTime)
        {
            elapsedTime += Time.deltaTime;
            float strength = _explosionShakeCurve.Evaluate(elapsedTime / _explosionShakeTime);
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
        if (!context.performed)
            return;

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        if (_time - _lastBulletTime < _bulletCooldown)
        {
            return;
        }

        if (!context.performed)
        {
            return;
        }

        _lastBulletTime = _time;

        GameObject bullet = Instantiate(_bullet, _pistol.transform.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Bullet b = bullet.GetComponent<Bullet>();

        rb.AddForce(_head.transform.forward * 12.0f, ForceMode.Force);
        rb.excludeLayers |= 1 << LayerMask.NameToLayer("Player");
        b.SetTrailColor(Color.white);

        _pistol.GetComponent<AudioSource>().Play();
    }

    public void UnlockMouse(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        Cursor.lockState = CursorLockMode.None;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.performed || !_isGrounded)
            return;

        _rb.AddForce(transform.up * 6.0f, ForceMode.Impulse);
        _isGrounded = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // For Laser Room
        if (other.CompareTag("Collectible"))
        {
            Transform room = other.gameObject.transform.parent.transform.parent;
            char roomNumber = room.name[^1];
            roomNumber++;
            GameObject nextObj = GameObject.Find("Room" + roomNumber);
            Transform nextTrans = nextObj.transform.Find("Door Wall(Clone)");
            nextTrans.GetComponent<Door>().IsOpening = true;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Laser"))
        {
            SpawnEnemy(other.GetComponent<Laser>().Room);
        }
        else if (other.TryGetComponent(out Door door))
        {
            door.IsOpening = false;
            door.IsClosing = true;
            _map.CurrentRoom = door.Room;
        }
        else if (other.CompareTag("Portal"))
        {
            _gameManager.IsGameActive = false;
        }
    }

    private void SpawnEnemy(GameObject room)
    {
        for (int i = 0; i < _enemyToSpawn; i++)
        {
            GameObject enemy = Instantiate(_enemy, room.transform.transform);
            Vector3 position = enemy.transform.localPosition;

            int x = Random.Range(-_roomLength / 2, _roomLength / 2);
            int y = Random.Range(0, _wallHeight / 2);
            int z = Random.Range(-_roomLength / 2, _roomLength / 2);

            position.x = x;
            position.y = y;
            position.z = z;
            enemy.transform.localPosition = position;

            Vector3 playerDirection = (transform.position - enemy.transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(playerDirection);
            enemy.transform.rotation = rotation;

            enemy.GetComponent<FlyingDrone>().Target = transform;


        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
    }
}
