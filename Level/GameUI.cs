using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class GameUI : MonoBehaviour
{
    GameObject _player;
    HPController _playerHP;
    PlayerController _playerController;
    ShootingController _shootingController;
    Spawner _spawner;
    UIMenu _uiMenu;
    CanvasGroup _canvasGroup_U;
    CanvasGroup _canvasGroup_G;

    public Image fadePlane;
    public GameObject gameOverUI;
    public GameObject upgradeUI;
    public GameObject canvas;
    public GameObject shootingPlane;
    
    public event System.Action Shuffle;
    public RectTransform newWaveBanner;
    public RectTransform bannerBackground;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    public Text waveUIText;

    bool isPaused = false;

    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _playerHP = _player.GetComponent<HPController>();
        _playerController = _player.GetComponent<PlayerController>();
        _shootingController = _player.GetComponent<ShootingController>();
        _spawner = FindObjectOfType<Spawner>();
        _uiMenu = GetComponent<UIMenu>();
        _canvasGroup_U = upgradeUI.GetComponent<CanvasGroup>();
        _canvasGroup_G = gameOverUI.GetComponent<CanvasGroup>();

        _playerHP.OnDeath += OnGameOver;
        _spawner.OnNewWave += OnNewWave;
        _spawner.OnWaveOver += OnWaveOver;
        _spawner.OnGameClear += OnGameClear;
        _playerController.enabled = false;
    }

    void Update()
    {
        if(_uiMenu.play && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    void OnGameOver()
    {
        Destroy(shootingPlane);
        canvas.SetActive(false);
        gameOverUI.SetActive(true);
        StartCoroutine(Fade(Color.clear, Color.black, 1, _canvasGroup_G));
    }

    void OnWaveOver()
    {
        _playerHP.enabled = false;
        AudioManager.instance.PlaySound("Clear", _player.transform.position);
        StartCoroutine(PauseGradually(1));
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, 0.5f), 1, _canvasGroup_U));
    }

    IEnumerator PauseGradually(float time)
    {
        float speed = 1/time;
        float percent = 0;
        
        while((percent += Time.unscaledDeltaTime * speed) < 1)
        {
            Time.timeScale = 1-percent;
            yield return null;
        }

        Time.timeScale = 0;
        upgradeUI.SetActive(true);
        Shuffle();
    }

    void OnGameClear()
    {
        canvas.SetActive(false);
        gameOverUI.SetActive(true);
        gameOverUI.transform.GetChild(0).gameObject.SetActive(false);
        gameOverUI.transform.GetChild(1).gameObject.SetActive(true);
        StartCoroutine(Fade(Color.clear, new Color(1, 1, 1, 0.5f), 1, _canvasGroup_G));
    }

    IEnumerator Fade(Color from, Color to, float time, CanvasGroup canvasGroup)
    {
        float speed = 1/time;
        float percent = 0;
        
        while(percent < 1)
        {
            percent += Time.unscaledDeltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            if(canvasGroup != null)
            {
                canvasGroup.alpha = percent;
            }
            yield return null;
        }
    }

    void OnNewWave(int waveNumber)
    {
        Time.timeScale = 1;
        upgradeUI.SetActive(false);
        _playerController.enabled = true;
        _playerController.StopPlayer(true);
        _playerController.StopPlayer(false);
        StartCoroutine(Fade(fadePlane.color, Color.clear, 1, null));
        newWaveTitle.text = "Wave " + (waveNumber);
        newWaveEnemyCount.text = "Enemies: " + _spawner.waves[waveNumber - 1].enemyCount;
        waveUIText.text = "Wave " + (waveNumber);
        StartCoroutine(AnimateNewWaveBanner());
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 1;
        float speed = 3;
        float animatePercent = 0;
        float endDelayTime = Time.time + 1/speed + delayTime;

        while(animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed;
            if(animatePercent >= 1)
            {
                animatePercent = 1;
                if(Time.time > endDelayTime)
                {
                    animatePercent = 0;
                    break;
                }
            }
            newWaveBanner.anchoredPosition = Vector2.right * Mathf.Lerp(-1200, 0, animatePercent);
            bannerBackground.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, animatePercent * 300);
            yield return null;
        }
        while(animatePercent < 1)
        {
            animatePercent += Time.deltaTime * speed;
            newWaveBanner.anchoredPosition = Vector2.right * Mathf.Lerp(0, 1200, animatePercent);
            bannerBackground.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 300 - animatePercent * 300);
            yield return null;
        }
    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        _uiMenu.optionsMenuHolder.SetActive(false);
        _uiMenu.controlsMenuHolder.SetActive(false);
        if(isPaused)
        {
            Time.timeScale = 0;
            fadePlane.color = new Color(0, 0, 0, 0.5f);
            canvas.SetActive(false);
            _uiMenu.pauseMenuHolder.SetActive(true);
            _playerController.enabled = false;
        }
        else
        {
            Time.timeScale = 1;
            fadePlane.color = Color.clear;
            canvas.SetActive(true);
            _uiMenu.pauseMenuHolder.SetActive(false);
            _playerController.enabled = true;
        }
    }
}
