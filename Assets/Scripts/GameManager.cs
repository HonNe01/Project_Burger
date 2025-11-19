using System;
using UnityEngine;
using TMPro; // ✅ TextMeshPro 사용

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameMode { None, Tutorial, Easy, Hard }

    [Header("=== Game Setting ===")]
    public GameMode currentMode = GameMode.None;
    private int currentScore = 0;
    private int successCount = 0;
    private int failCount = 0;

    [Header("=== Game UI ===")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject easyPanel;
    [SerializeField] private GameObject hardPanel;
    [SerializeField] private GameObject exitPanel;

    [Header("=== Score UI ===")]
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private GameObject scoreExitPanel;
    [SerializeField] private TextMeshProUGUI scoreTMP;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultScoreText;       // 총점
    [SerializeField] private TextMeshProUGUI successCustomerText;   // 성공한 손님 수
    [SerializeField] private TextMeshProUGUI failCustomerText;      // 실패한 손님 수

    [Header("=== Timer (TextMeshPro) ===")]
    [SerializeField] private TextMeshProUGUI timerTMP;
    [SerializeField] private float startTime = 180f;


    [Header("=== Tutorial ===")]
    public bool isTutorial = false;


    // 내부 관리 변수들
    private float remainingTime;
    private bool isTimerRunning = false;
    

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // 메인 패널
        easyPanel.SetActive(false);
        hardPanel.SetActive(false);
        exitPanel.SetActive(false);

        // 장사 패널
        scorePanel.SetActive(false);
        scoreExitPanel.SetActive(false);
        resultPanel.SetActive(false);

        ResetTimer();
        ResetScore();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            remainingTime -= Time.deltaTime;

            // 제한 시간 종료
            if (remainingTime <= 0f)
            {
                remainingTime = 0f;

                ResultGame();
                StopTimer();
            }

            UpdateTimerUI(remainingTime);
        }
    }

    // ====== 모드 선택 관련 ======
    public void SetTutorial()
    {
        Debug.Log("게임 모드: Tutorial 선택됨");

        currentMode = GameMode.Tutorial;
        tutorialPanel.SetActive(true);
    }
    
    public void TutorialNo()
    {
        currentMode = GameMode.None;
        tutorialPanel.SetActive(false);
    }

    public void SetEasy()
    {
        Debug.Log("게임 모드: Easy 선택됨");

        currentMode = GameMode.Easy;
        easyPanel.SetActive(true);
    }

    public void EasyNo()
    {
        currentMode = GameMode.None;
        easyPanel.SetActive(false);
    }

    public void SetHard()
    {
        Debug.Log("게임 모드: Hard 선택됨");

        currentMode = GameMode.Hard;
        hardPanel.SetActive(true);
    }

    public void HardNo()
    {
        currentMode = GameMode.None;
        hardPanel.SetActive(false);
    }

    // ====== 게임 시작 ======
    public void StartGame()
    {
        if (currentMode == GameMode.None)
        {
            Debug.LogWarning("[GameManager] 게임 모드 선택 안함");

            return;
        }
        else if (currentMode == GameMode.Tutorial)
        {
            if (isTutorial == true) return;

            Tutorial.instance.StartTutorial();
            return;
        }

        easyPanel.SetActive(false);
        hardPanel.SetActive(false);

        switch (currentMode)
        {
            case GameMode.Easy:
                startTime = 180f;
                Debug.Log("Easy 모드로 게임 시작!");
                break;
            case GameMode.Hard:
                startTime = 180f;
                Debug.Log("Hard 모드로 게임 시작!");
                break;
        }

        scorePanel.SetActive(true);
        mainPanel.SetActive(false);

        // 시간과 점수 초기화 후 시작
        ResetTimer();
        ResetScore();
        StartTimer();

        CustomerManager.instance.GameStart();
        SoundManager.instance.PlayGameBGM();
    }

    // ====== 게임 정산 관련 ======
    public void ResultGame()
    {
        Debug.Log("[GameManager] 정산 완료");
        CustomerManager.instance.GameStop();
        SoundManager.instance.PlayMainBGM();

        resultScoreText.text = $"Total Score: {currentScore}";
        successCustomerText.text = $"Success Order: {successCount}";
        failCustomerText.text = $"Fail Order: {failCount}";

        resultPanel.SetActive(true);
        scorePanel.SetActive(false);
    }

    public void ResultCheck()
    {
        ResetTimer();
        ResetScore();

        mainPanel.SetActive(true);
        resultPanel.SetActive(false);
    }

    // ====== 장사 종료 관련 ======
    public void StopGame()
    {
        scoreExitPanel.SetActive(true);
    }

    public void StopYes()
    {
        Debug.Log("[GameManager] 장사를 종료합니다.");
        mainPanel.SetActive(true);

        // 게임 초기화
        CustomerManager.instance.GameStop();
        SoundManager.instance.PlayMainBGM();
        ResetTimer();
        ResetScore();

        scoreExitPanel.SetActive(false);
        scorePanel.SetActive(false);
    }

    public void StopNo()
    {
        scoreExitPanel.SetActive(false);
    }

    // ====== 게임 종료 관련 ======
    public void ExitGame()
    {
        exitPanel.SetActive(true);
    }

    public void ExitYes()
    {
        Debug.Log("Game Exit");

        exitPanel.SetActive(false);
        Application.Quit();
    }

    public void ExitNo()
    {
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
        if (amount > 0) successCount++;
        else failCount++;

        currentScore += amount;
        UpdateScoreUI();
        Debug.Log($"[GameManager] 현재 점수: {currentScore}, 성공한 주문 수: {successCount}, 실패한 주문 수: {failCount}");
    }

    public void ResetScore()
    {
        Debug.Log("[GameManager] 게임 초기화");
        currentScore = 0;
        successCount = 0;
        failCount = 0;

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
