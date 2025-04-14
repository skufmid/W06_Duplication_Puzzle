using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }
    private PauseMenuHandler pauseMenuHandler;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePauseMenu();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePauseMenu()
    {
        GameObject pauseMenuPrefab = Resources.Load<GameObject>("PauseMenu");
        if (pauseMenuPrefab == null)
        {
            Debug.LogError("PauseMenu �������� Resources �������� ã�� �� �����ϴ�!");
            return;
        }

        GameObject pauseMenuInstance = Instantiate(pauseMenuPrefab);
        DontDestroyOnLoad(pauseMenuInstance);
        pauseMenuHandler = pauseMenuInstance.GetComponent<PauseMenuHandler>();
        if (pauseMenuHandler == null)
        {
            Debug.LogError("PauseMenu �����տ� PauseMenuHandler ������Ʈ�� �����ϴ�!");
            Destroy(pauseMenuInstance);
        }
    }

    void Update()
    {
        // Esc Ű �Է� ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ���� ���� �������� ������ Ȯ��
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene.StartsWith("COM") && !currentScene.EndsWith("SelectScene"))
            {
                // �������� �������� PauseMenu ȣ��
                GameManager.Instance.TogglePause();
            }
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public PauseMenuHandler GetPauseMenuHandler()
    {
        return pauseMenuHandler;
    }
}