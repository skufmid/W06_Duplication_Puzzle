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
            Debug.LogError("PauseMenu 프리팹을 Resources 폴더에서 찾을 수 없습니다!");
            return;
        }

        GameObject pauseMenuInstance = Instantiate(pauseMenuPrefab);
        DontDestroyOnLoad(pauseMenuInstance);
        pauseMenuHandler = pauseMenuInstance.GetComponent<PauseMenuHandler>();
        if (pauseMenuHandler == null)
        {
            Debug.LogError("PauseMenu 프리팹에 PauseMenuHandler 컴포넌트가 없습니다!");
            Destroy(pauseMenuInstance);
        }
    }

    void Update()
    {
        // Esc 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 현재 씬이 스테이지 씬인지 확인
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene.StartsWith("COM") && !currentScene.EndsWith("SelectScene"))
            {
                // 스테이지 씬에서만 PauseMenu 호출
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