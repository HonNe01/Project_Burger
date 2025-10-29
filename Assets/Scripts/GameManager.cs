using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameMode { None, Easy, Hard }
    [Header(" === Game Setting === ")]
    public GameMode currentMode = GameMode.None;

    [Header(" === Game UI === ")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject easyPanel;
    [SerializeField] private GameObject hardPanel;
    [SerializeField] private GameObject exitPanel;

    [Header(" === Score UI === ")]
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private GameObject endPanel;


    void Awake()
    {
        // 싱글톤 초기화
        if (instance == null) instance = this;
        else Destroy(gameObject);

        easyPanel.SetActive(false);
        hardPanel.SetActive(false);
        exitPanel.SetActive(false);
        if (scorePanel != null) scorePanel.SetActive(false);
    }

    // Easy 버튼에서 호출될 함수
    public void SetEasy()
    {
        currentMode = GameMode.Easy;
        Debug.Log("게임 모드: Easy 선택됨");

        easyPanel.SetActive(true);
    }

    public void EasyNo()
    {
        currentMode = GameMode.None;

        easyPanel.SetActive(false);
    }

    // Hard 버튼에서 호출될 함수
    public void SetHard()
    {
        currentMode = GameMode.Hard;
        Debug.Log("게임 모드: Hard 선택됨");

        hardPanel.SetActive(true);
    }

    public void HardNo()
    {
        currentMode = GameMode.None;

        hardPanel.SetActive(false);
    }

    // 모드에 따른 게임 시작
    public void StartGame()
    {
        easyPanel.SetActive(false);
        hardPanel.SetActive(false);

        switch (currentMode)
        {
            case GameMode.Easy:
                // Easy 모드 세팅
                // 예: 적 체력 낮추기, 적 생성 수 줄이기 등
                Debug.Log("Easy 모드로 게임 시작!");
                break;

            case GameMode.Hard:
                // Hard 모드 세팅
                // 예: 적 체력 높이기, 적 생성 수 늘리기 등
                Debug.Log("Hard 모드로 게임 시작!");
                break;
        }

        if (scorePanel != null) scorePanel.SetActive(true);
        mainPanel.SetActive(false);
        CustomerManager.instance.GameStart();
    }

    public void EndGame()
    {
        endPanel.SetActive(true);
    }

    public void EndYes()
    {
        endPanel.SetActive(false);
        scorePanel.SetActive(false);
        
        CustomerManager.instance.GameStop();
        mainPanel.SetActive(true);
    }
    
    public void EndNo()
    {
        endPanel.SetActive(false);
    }

    public void ExitGame()
    {
        exitPanel.SetActive(true);
    }

    public void ExitYes()
    {
        exitPanel.SetActive(false);

        Debug.Log("Game Exit");
        Application.Quit();
    }

    public void ExitNo()
    {
        exitPanel.SetActive(false);
    }
    
}
