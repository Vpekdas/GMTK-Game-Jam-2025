using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public void OnAnimationEnd()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
