using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 싱글톤 패턴
    public enum GameMode { None, Easy, Hard }
    public GameMode currentMode = GameMode.None;

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Easy 버튼에서 호출될 함수
    public void SetEasyMode()
    {
        currentMode = GameMode.Easy;
        Debug.Log("게임 모드: Easy 선택됨");
        StartGame();
    }

    // Hard 버튼에서 호출될 함수
    public void SetHardMode()
    {
        currentMode = GameMode.Hard;
        Debug.Log("게임 모드: Hard 선택됨");
        StartGame();
    }

    // 모드에 따른 게임 시작
    private void StartGame()
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

            default:
                Debug.LogWarning("게임 모드가 선택되지 않음");
                break;
        }
    }
}
