using UnityEngine;

public class FlyingDrone : MonoBehaviour
{
    [SerializeField] private float _cooldownTime = 0.5f;
    [SerializeField] private GameObject _bullet;
    private ParticleSystem _particleSystem;
    private Rigidbody _rb;
    private AudioSource _explosionAudioSource;
    private AudioSource _gunAudioSource;

    public Transform target;

    private Mortal _mortal;
    private float _time;
    private float _lastBulletTime;
    private bool _isDestroyed;

    void Awake()
    {
        _mortal = GetComponent<Mortal>();
        _rb = GetComponent<Rigidbody>();

        AudioSource[] sources = GetComponents<AudioSource>();

        _explosionAudioSource = sources[0];
        _gunAudioSource = sources[1];

        _mortal.death.AddListener(OnDeath);
    }

    void Start()
    {
    }

    void FixedUpdate()
    {
        _time += Time.fixedDeltaTime;

        if (!_mortal.IsDead() && _time - _lastBulletTime >= _cooldownTime && CanSeeTarget(target, 60.0f, 10.0f))
        {
            GameObject bullet = Instantiate(_bullet, transform.position + transform.forward * 0.4f, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            Bullet b = bullet.GetComponent<Bullet>();

            _gunAudioSource.Play();

            // TODO: Rotate the turret & drone toward the player.
            //       The gun first then the drone lag behind a bit.
            Vector3 toPlayer = target.position - transform.position;
            toPlayer.Normalize();

            rb.AddForce(toPlayer * 12.0f, ForceMode.Force);
            rb.excludeLayers |= 1 << LayerMask.NameToLayer("Enemy");
            b.SetTrailColor(Color.yellowNice);

            _lastBulletTime = _time;
        }

        if (_isDestroyed && !_particleSystem.IsAlive())
        {
            Destroy(gameObject);
        }
    }

    void OnDeath()
    {
        _particleSystem.Play();
        _isDestroyed = true;
        _rb.useGravity = false;
        _rb.detectCollisions = false;

        _explosionAudioSource.Play();

        PlayerController.explosionEvent.Invoke();

        foreach (Transform t in transform)
        {
            if (t.gameObject != _particleSystem.gameObject)
            {
                t.gameObject.SetActive(false);
            }
        }
    }

    bool CanSeeTarget(Transform target, float viewAngle, float viewRange)
    {
        Vector3 toTarget = target.position - transform.position;

        if (Vector3.Angle(transform.forward, toTarget) <= viewAngle)
        {
            if (Physics.Raycast(transform.position, toTarget, out RaycastHit hit, viewRange))
            {
                if (hit.transform.root == target)
                    return true;
            }
        }

        return false;
    }
}
