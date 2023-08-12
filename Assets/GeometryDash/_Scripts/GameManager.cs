using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const float TIME_PAUSED = 0f;
    private const float TIME_CONTINUE = 1f;
    #region Singleton
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    [SerializeField] private GameObject _pausePanel;

    public GameObject PausePanel { get => _pausePanel; set => _pausePanel = value; }

    
    private void OnDisable()
    {
        //Sahneyi Terkederken timescale 0 da kalmamasý için.
        Time.timeScale = TIME_CONTINUE;
    }

    public void PauseGame()
    {
        Time.timeScale = TIME_PAUSED;
        PausePanel.SetActive(true);

    }
    public void ContinueGame()
    {
        PausePanel.SetActive(false);
        Time.timeScale = TIME_CONTINUE;
    }

}
