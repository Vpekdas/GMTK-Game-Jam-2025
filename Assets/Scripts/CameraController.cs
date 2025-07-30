using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private float _speedH;
    [SerializeField] private float _speedV;

    private float _yaw = 0.0f;
    private float _pitch = 0.0f;

    private void LateUpdate()
    {
        _yaw += _speedH * Input.GetAxis("Mouse X");
        _pitch -= _speedV * Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector3(_pitch, _yaw, 0.0f);
    }
}
