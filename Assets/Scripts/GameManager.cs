using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] float _timer;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private GameObject _transition;
    private bool _isGameActive, _isGameFinished;

    public bool IsGameActive { get => _isGameActive; set => _isGameActive = value; }

    private void Awake()
    {
        _isGameActive = true;
        _isGameFinished = false;
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
        _transition.SetActive(true);
    }
}
