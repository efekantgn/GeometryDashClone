using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadScene(int pSceneID)
    {
        SceneManager.LoadScene(pSceneID);
    }
}
