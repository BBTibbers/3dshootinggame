using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum GameState
{
    Ready,
    Run,
    Over
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }

    public TextMeshProUGUI MainText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        StartCoroutine(GameFlowCoroutine());
    }

    private IEnumerator GameFlowCoroutine()
    {
        // 1. Ready 상태
        SetGameState(GameState.Ready);
        MainText.text = "Ready!";

        // UI 표시 후 잠깐 대기
        yield return new WaitForSecondsRealtime(2f);
        MainText.text = "Go!";

        yield return new WaitForSecondsRealtime(0.5f);
        MainText.text = "";
        SetGameState(GameState.Run);

    }

    public void GameOver()
    {
        SetGameState(GameState.Over);
        MainText.text = "Game Over!";
    }

    private void SetGameState(GameState state)
    {
        CurrentState = state;

        // 상태에 따른 카메라/입력 제어 예시
        switch (state)
        {
            case GameState.Ready:
                Time.timeScale = 0f; // 정지 (카메라 포함)
                break;
            case GameState.Over:
                Time.timeScale = 0f; // 정지 (카메라 포함)
                break;

            case GameState.Run:
                Time.timeScale = 1f; // 플레이 재개
                break;
        }

        Debug.Log("현재 상태: " + state);
    }
}
