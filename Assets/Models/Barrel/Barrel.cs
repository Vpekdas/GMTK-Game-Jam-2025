using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    private bool _isDestroyed;

    void Start()
    {
    }

    void Update()
    {
        if (!_particleSystem.IsAlive() && _isDestroyed)
        {
            Destroy(gameObject);
        }
    }

    public void Explode()
    {
        if (_isDestroyed)
        {
            return;
        }

        _particleSystem.Play();
        _isDestroyed = true;

        foreach (Transform t in transform)
        {
            if (t.gameObject != _particleSystem.gameObject)
            {
                t.gameObject.SetActive(false);
            }
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, 2.0f);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Mortal mortal))
            {
                mortal.Damage(20.0f);
            }
            else if (collider.TryGetComponent(out PlayerController player))
            {
                player.Kill();
            }
            else if (collider.TryGetComponent(out Barrel barrel))
            {
                barrel.Explode();
            }
        }

        PlayerController.explosionEvent.Invoke();
    }
}
