using UnityEngine;

public class EndTransition : MonoBehaviour
{
    [SerializeField] private GameObject _background;
    public void OnAnimationEnd()
    {
        gameObject.SetActive(false);
        _background.SetActive(true);
    }
}
