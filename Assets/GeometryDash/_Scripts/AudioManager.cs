using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private const string MIXER_SFX = "SFX";
    private const string MIXER_MUSIC = "Music";

    #region Singleton
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(gameObject);
    }
    #endregion


    public AudioMixer Mixer;
    public Slider Music;
    public Slider SFX;
    private GameObject SettingsPanel;

    private float _musicValue;
    private float _sfxValue;

    public float SfxValue { get => _sfxValue; set => _sfxValue = value; }
    public float MusicValue { get => _musicValue; set => _musicValue = value; }

    /// <summary>
    /// ilk AnaMenüde 1 defa çalýþýyor Singeleton olduðu için
    /// ilk sahnedeki sliderlarý initialize ediyor.
    /// </summary>
    private void Start()
    {
        SfxValue = .7f;
        MusicValue = .7f;
        InitializeSettings();
        AddSliderListeners();
        SceneManager.sceneUnloaded += SceneUnloaded;
        SceneManager.sceneLoaded += SceneLoaded;
    }

    /// <summary>
    /// Yeni sahnedeki sliderlarý initialize ediyor.
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        InitializeSettings();
        AddSliderListeners();
    }

    /// <summary>
    /// Eski sahnedeki sliderlarýn listenerlarýný temizliyor.
    /// </summary>
    /// <param name="arg0"></param>
    private void SceneUnloaded(Scene arg0)
    {
        RemoveSliderListeners();
    }

    private void AddSliderListeners()
    {
        Music.onValueChanged.AddListener(MusicOnValueChanged);
        SFX.onValueChanged.AddListener(SFXOnValueChanged);
        
        Music.value = MusicValue;
        SFX.value = SfxValue;
    }

    private void RemoveSliderListeners()
    {
        Music.onValueChanged.RemoveListener(MusicOnValueChanged);
        SFX.onValueChanged.RemoveListener(SFXOnValueChanged);
    }

    private void InitializeSettings()
    {
        Music = GameObject.Find("MusicSlider").GetComponent<Slider>();
        SFX = GameObject.Find("SFXSlider").GetComponent<Slider>();
        
        SettingsPanel = GameObject.Find("SettingsPanel");
        SettingsPanel.SetActive(false);
        
    }

    private void SFXOnValueChanged(float arg0)
    {
        SfxValue = arg0;
        Mixer.SetFloat(MIXER_SFX, Mathf.Log(SfxValue) * 20f);
    }

    private void MusicOnValueChanged(float arg0)
    {
        MusicValue = arg0;
        Mixer.SetFloat(MIXER_MUSIC, Mathf.Log(MusicValue) * 20f);

    }
}
