using System;
using UnityEngine;
using UnityEngine.Events;

public class Mortal : MonoBehaviour
{
    [SerializeField] private float _health = 10.0f;

    private float _currentHealth;

    [NonSerialized] public UnityEvent death = new();

    void Start()
    {
        _currentHealth = _health;
    }

    void Update()
    {
    }

    public void Damage(float health)
    {
        if (_currentHealth <= 0.0)
        {
            return;
        }

        _currentHealth -= health;

        if (IsDead())
        {
            death.Invoke();
        }
    }

    public bool IsDead()
    {
        return _currentHealth <= 0.0f;
    }
}
