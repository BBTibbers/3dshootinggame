using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum EGameState
{
    Ready,
    Run,
    Over,
    Pause
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public EGameState CurrentState { get; private set; }

    public TextMeshProUGUI MainText;
    public UI_OptionPopup OptionPopup;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        StartCoroutine(GameFlowCoroutine());
    }

    private void Update()
    {
       
    }

    public void Pause()
    {
        CurrentState = EGameState.Pause;
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;

        PopupManager.Instance.Open(EPopupType.UI_OptionPopup, closeCallback: Continue);
    }

    public void Continue()
    {
        CurrentState = EGameState.Run;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Restart()
    {
        CurrentState = EGameState.Run;
        Time.timeScale = 1;

        PopupManager.Instance.Clear();
        Cursor.lockState = CursorLockMode.Locked;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);


        // 다시시작을 했더니 게임이 망가지는 경우가 있다...
        // 싱글톤 처리를 잘못했을경우 망가진다.
    }

private IEnumerator GameFlowCoroutine()
    {
        // 1. Ready 상태
        SetGameState(EGameState.Ready);
        MainText.text = "Ready!";

        // UI 표시 후 잠깐 대기
        yield return new WaitForSecondsRealtime(2f);
        MainText.text = "Go!";

        yield return new WaitForSecondsRealtime(0.5f);
        MainText.gameObject.SetActive(false);
        SetGameState(EGameState.Run);

    }

    public void GameOver()
    {
        SetGameState(EGameState.Over);
        MainText.gameObject.SetActive(true);
        MainText.text = "Game Over!";
    }

    private void SetGameState(EGameState state)
    {
        CurrentState = state;

        // 상태에 따른 카메라/입력 제어 예시
        switch (state)
        {
            case EGameState.Ready:
                Time.timeScale = 0f; // 정지 (카메라 포함)
                break;
            case EGameState.Over:
                Time.timeScale = 0f; // 정지 (카메라 포함)
                break;

            case EGameState.Run:
                Time.timeScale = 1f; // 플레이 재개
                break;
        }

        Debug.Log("현재 상태: " + state);
    }
}
