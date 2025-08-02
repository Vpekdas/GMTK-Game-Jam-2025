using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] float _speed;
    private bool _isOpening;
    private bool _isClosing;
    private float _maxHeight;
    private GameObject _door;
    public MapGenerator.Room Room;

    public bool IsOpening { get => _isOpening; set => _isOpening = value; }
    public bool IsClosing { get => _isClosing; set => _isClosing = value; }

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
        if (_isOpening && !_isClosing && _door.transform.position.y < _maxHeight)
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
        else if (_isClosing && _door.transform.position.y > 0.0)
        {
            _door.transform.position += _speed * Time.deltaTime * Vector3.down;
            _isOpening = false;
            if (_door.transform.position.y <= 0.0)
            {
                Vector3 pos = _door.transform.position;
                pos.y = 0.0f;
                _door.transform.position = pos;
                _isClosing = false;
            }
        }
    }
}
