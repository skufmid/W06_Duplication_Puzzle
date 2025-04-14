using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; // Pause Menu�� GameObject
    [SerializeField] private Button resumeButton; // �̾��ϱ� ��ư
    [SerializeField] private Button restartButton; // ����� ��ư
    [SerializeField] private Button quitButton; // ���� ��ư

    private void Start()
    {
        // �ʱ� ����: �޴� ����
        SetMenuActive(false);

        // ��ư ������ ����
        resumeButton.onClick.AddListener(OnResumeClicked);
        restartButton.onClick.AddListener(OnRestartClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    public void SetMenuActive(bool active)
    {
        pauseMenuUI.SetActive(active);
        // Ŀ�� ���� ���� (�����÷��̿� UI �� ��ȯ)
        //Cursor.visible = active;
        //Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void OnResumeClicked()
    {
        GameManager.Instance.TogglePause(); // Pause ���� ���
    }

    private void OnRestartClicked()
    {
        Time.timeScale = 1f; // �ð� �缳��
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ���� �������� �����
        GameManager.Instance.TogglePause(); // Pause ���� ���
    }

    private void OnQuitClicked()
    {
        Time.timeScale = 1f; // �ð� �缳��
        int worldNumber = GameManager.Instance.CurWorld;
        string sceneName = $"Stage{worldNumber}SelectScene";
        GameManager.Instance.TogglePause(); // Pause ���� ���
        SceneManager.LoadScene(sceneName);
        // �Ǵ� ���� ����:
        // Application.Quit();
        // #if UNITY_EDITOR
        // UnityEditor.EditorApplication.isPlaying = false;
        // #endif
    }
}