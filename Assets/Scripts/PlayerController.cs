using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Camera _camera;
    private Rigidbody _rb;
    private Vector3 _input;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        _input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        Vector3 camForward = _camera.transform.forward;
        Vector3 camRight = _camera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = camForward * _input.z + camRight * _input.x;
        _rb.linearVelocity = moveDirection * _speed + new Vector3(0, _rb.linearVelocity.y, 0);
    }
}