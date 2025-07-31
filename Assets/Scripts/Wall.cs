using UnityEngine;

public class Wall : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Destroying");
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void Onx2CollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Destroying");
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
