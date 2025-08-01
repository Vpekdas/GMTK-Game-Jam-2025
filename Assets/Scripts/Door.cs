using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] float _speed;
    private bool _isOpening;
    private float _maxHeight;
    private GameObject _door;

    private void Awake()
    {
        _isOpening = false;
        _maxHeight = 30.0f;
    }

    private void Start()
    {
        _door = transform.Find("Door").gameObject;
    }

    private void Update()
    {
        if (_isOpening && _door.transform.position.y < _maxHeight)
        {
            _door.transform.position += _speed * Time.deltaTime * Vector3.up;
            if (_door.transform.position.y > _maxHeight)
            {
                Vector3 pos = _door.transform.position;
                pos.y = _maxHeight;
                _door.transform.position = pos;
                _isOpening = false;
            }
        }
    }

    public void OpenDoor()
    {
        _isOpening = true;
    }
}