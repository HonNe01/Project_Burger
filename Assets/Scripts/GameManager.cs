using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameMode { None, Easy, Hard }
    [Header(" === Game Setting === ")]
    public GameMode currentMode = GameMode.None;

    [Header(" === Game UI === ")]
    [SerializeField] private GameObject easyPanel;
    [SerializeField] private GameObject hardPanel;
    [SerializeField] private GameObject exitPanel;

    [Header(" === Score UI === ")]
    [SerializeField] private GameObject scorePanel;


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
    public void SetEasyMode()
    {
        currentMode = GameMode.Easy;
        Debug.Log("게임 모드: Easy 선택됨");

        easyPanel.SetActive(true);
    }

    // Hard 버튼에서 호출될 함수
    public void SetHardMode()
    {
        currentMode = GameMode.Hard;
        Debug.Log("게임 모드: Hard 선택됨");

        hardPanel.SetActive(true);
    }

    // 모드에 따른 게임 시작
    public void StartGame()
    {
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
        CustomerManager.instance.GameStart();
    }

    public void EndGame()
    {
        scorePanel.SetActive(false);
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
