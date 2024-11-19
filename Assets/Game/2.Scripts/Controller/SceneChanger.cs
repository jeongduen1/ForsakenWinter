using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private int SceneNumber;

    public void SceneChange()
    {
        SceneManager.LoadScene(SceneNumber);
    }
}
