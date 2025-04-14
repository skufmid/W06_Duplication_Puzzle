using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; // Pause Menu의 GameObject
    [SerializeField] private Button resumeButton; // 이어하기 버튼
    [SerializeField] private Button restartButton; // 재시작 버튼
    [SerializeField] private Button quitButton; // 종료 버튼

    private void Start()
    {
        // 초기 상태: 메뉴 숨김
        SetMenuActive(false);

        // 버튼 리스너 설정
        resumeButton.onClick.AddListener(OnResumeClicked);
        restartButton.onClick.AddListener(OnRestartClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    public void SetMenuActive(bool active)
    {
        pauseMenuUI.SetActive(active);
        // 커서 상태 관리 (게임플레이와 UI 간 전환)
        //Cursor.visible = active;
        //Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void OnResumeClicked()
    {
        GameManager.Instance.TogglePause(); // Pause 상태 토글
    }

    private void OnRestartClicked()
    {
        Time.timeScale = 1f; // 시간 재설정
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 스테이지 재시작
        GameManager.Instance.TogglePause(); // Pause 상태 토글
    }

    private void OnQuitClicked()
    {
        Time.timeScale = 1f; // 시간 재설정
        int worldNumber = GameManager.Instance.CurWorld;
        string sceneName = $"Stage{worldNumber}SelectScene";
        GameManager.Instance.TogglePause(); // Pause 상태 토글
        SceneManager.LoadScene(sceneName);
        // 또는 게임 종료:
        // Application.Quit();
        // #if UNITY_EDITOR
        // UnityEditor.EditorApplication.isPlaying = false;
        // #endif
    }
}