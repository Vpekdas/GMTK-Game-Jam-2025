using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] float _timer;
    [SerializeField] TextMeshProUGUI _timerText;
    private bool _isGameActive;

    public bool IsGameActive { get => _isGameActive; set => _isGameActive = value; }

    private void Awake()
    {
        _isGameActive = true;
    }

    private void Start()
    {
        StartCoroutine(TimerRoutine());
    }


    IEnumerator TimerRoutine()
    {
        while (_timer > 0 && _isGameActive)
        {
            _timer--;
            _timerText.text = "Time before portal closes: " + _timer;
            yield return new WaitForSeconds(1.0f);
        }
    }
}
