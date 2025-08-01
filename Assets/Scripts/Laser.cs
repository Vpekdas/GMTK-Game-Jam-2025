using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed, _maxHorizontal, _maxVertical;
    private bool _isMovingHorizontal, _isMovingVertical, _revertDirection;

    private void Awake()
    {
        _isMovingHorizontal = false;
        _isMovingVertical = false;
        _revertDirection = false;
        if (Random.Range(0, 100) <= 30)
        {
            _isMovingHorizontal = true;
        }
        else if (Random.Range(0, 100) <= 60)
        {
            _isMovingVertical = true;
        }

    }

    private void Update()
    {
        if (_isMovingHorizontal)
        {
            MoveHorizontal();
        }
        else if (_isMovingVertical)
        {
            MoveVertical();
        }
    }

    private void MoveHorizontal()
    {
        if (transform.localPosition.x >= 14)
        {
            _revertDirection = true;
        }
        else if (transform.localPosition.x <= -14)
        {
            _revertDirection = false;
        }
        if (!_revertDirection)
        {
            transform.localPosition += _speed * Time.deltaTime * Vector3.right;
        }
        else
        {
            transform.localPosition += _speed * Time.deltaTime * Vector3.left;
        }

    }

    private void MoveVertical()
    {
        if (transform.localPosition.y >= _maxVertical)
        {
            _revertDirection = true;
        }
        else if (transform.localPosition.y <= 0)
        {
            _revertDirection = false;
        }
        if (!_revertDirection)
        {
            transform.localPosition += _speed * Time.deltaTime * Vector3.up;
        }
        else
        {
            transform.localPosition += _speed * Time.deltaTime * Vector3.down;
        }
    }
}
