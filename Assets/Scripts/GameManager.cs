using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] float _timer;
    [SerializeField] TextMeshProUGUI _timerText;
    private bool _isGameActive;

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
            _timerText.text = "Time: " + _timer;
            yield return new WaitForSeconds(1.0f);
        }
    }
}
