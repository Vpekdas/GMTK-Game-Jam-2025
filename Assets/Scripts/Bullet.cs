using UnityEngine;

public class Bullet : MonoBehaviour
{

    void Start()
    {
    }

    void Update()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Mortal mortal))
        {
            mortal.Damage(20.0f);
        }
        else if (collision.collider.TryGetComponent(out Barrel barrel))
        {
            barrel.Explode();
        }
        else if (collision.collider.TryGetComponent(out PlayerController player))
        {
            player.Kill();
            player.IsDead = true;
        }

        Destroy(gameObject);
    }

    public void SetTrailColor(Color color)
    {
        TrailRenderer trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.startColor = trailRenderer.endColor = color;
    }
}
