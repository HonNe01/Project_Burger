using System;
using UnityEngine;
using TMPro; // ✅ TextMeshPro 사용

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameMode { None, Easy, Hard }

    [Header("=== Game Setting ===")]
    public GameMode currentMode = GameMode.None;

    [Header("=== Game UI ===")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject easyPanel;
    [SerializeField] private GameObject hardPanel;
    [SerializeField] private GameObject exitPanel;

    [Header("=== Score UI ===")]
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI scoreTMP;

    [Header("=== Timer (TextMeshPro) ===")]
    [SerializeField] private TextMeshProUGUI timerTMP;

    [SerializeField] private float startTime = 180f;    

    // 내부 관리 변수들
    private float remainingTime;
    private bool isTimerRunning = false;
    private int currentScore = 0;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        easyPanel.SetActive(false);
        hardPanel.SetActive(false);
        exitPanel.SetActive(false);

        if (scorePanel != null) scorePanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);

        ResetTimer();
        ResetScore();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                StopTimer();
                EndGame();
            }

            UpdateTimerUI(remainingTime);
        }
    }

    // ====== 모드 선택 ======
    public void SetEasy()
    {
        ClickSound();
        Debug.Log("게임 모드: Easy 선택됨");

        currentMode = GameMode.Easy;
        easyPanel.SetActive(true);
    }

    public void EasyNo()
    {
        ClickSound();

        currentMode = GameMode.None;
        easyPanel.SetActive(false);
    }

    public void SetHard()
    {
        ClickSound();
        Debug.Log("게임 모드: Hard 선택됨");

        currentMode = GameMode.Hard;
        hardPanel.SetActive(true);
    }

    public void HardNo()
    {
        ClickSound();

        currentMode = GameMode.None;
        hardPanel.SetActive(false);
    }

    // ====== 게임 시작 ======
    public void StartGame()
    {
        ClickSound();
        easyPanel.SetActive(false);
        hardPanel.SetActive(false);

        switch (currentMode)
        {
            case GameMode.Easy:
                startTime = 180f;
                Debug.Log("Easy 모드로 게임 시작!");
                break;
            case GameMode.Hard:
                startTime = 120f;
                Debug.Log("Hard 모드로 게임 시작!");
                break;
        }

        if (scorePanel != null) scorePanel.SetActive(true);
        mainPanel.SetActive(false);

        CustomerManager.instance.GameStart();

        // 시간과 점수 초기화 후 시작
        ResetTimer();
        ResetScore();
        StartTimer();

        SoundManager.instance.PlayGameBGM();
    }

    // ====== 게임 종료 ======
    public void EndGame()
    {
        Debug.Log("⏰ 제한시간 종료! 게임 오버");

        StopTimer();
        endPanel.SetActive(true);
    }

    public void EndYes()
    {
        ClickSound();

        endPanel.SetActive(false);
        if (scorePanel != null) scorePanel.SetActive(false);

        CustomerManager.instance.GameStop();
        mainPanel.SetActive(true);

        ResetTimer();
        ResetScore();
    }

    public void EndNo()
    {
        ClickSound();

        endPanel.SetActive(false);
        StartTimer();
    }

    // ====== 종료 관련 ======
    public void ExitGame()
    {
        ClickSound();
        
        exitPanel.SetActive(true);
    }

    public void ExitYes()
    {
        ClickSound();
        Debug.Log("Game Exit");

        exitPanel.SetActive(false);
        Application.Quit();
    }

    public void ExitNo()
    {
        ClickSound();

        exitPanel.SetActive(false);
    }

    // ====== 타이머 ======
    public void StartTimer() => isTimerRunning = true;
    public void StopTimer() => isTimerRunning = false;

    public void ResetTimer()
    {
        remainingTime = startTime;
        UpdateTimerUI(remainingTime);
    }

    private void UpdateTimerUI(float timeSec)
    {
        if (timerTMP == null) return;

        int total = Mathf.FloorToInt(timeSec);
        int mm = total / 60;
        int ss = total % 60;

        timerTMP.text = $"<b>TIME</b>\n{mm:00}:{ss:00}";
    }

    // ====== ✅ 점수 관리 ======
    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
        Debug.Log($"현재 점수: {currentScore}");
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreTMP == null) return;
        scoreTMP.text = $"<b>SCORE</b>\n{currentScore}";
    }

    public void ClickSound()
    {
        SoundManager.instance.PlaySFX(SoundManager.SFX.Click);
    }
}
