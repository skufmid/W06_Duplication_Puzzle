using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<string[]> Worlds = new List<string[]>()
    {
        new string[] { },
        new string[] {"", "FIX_Stage01_A_01_JJH",  "FIX_Stage01_ABE_01_YDH"},
        new string[] { },
        new string[] { }
    };
    int curWorld = 1;
    public int CurWorld { get { return curWorld; } set { curWorld = value; } }
    int curScene;
    Goal[] goals;
    Spawner[] spawners;

    // 저장 관련
    public int[] clearStages;
    public string savedData;

    // sprite
    public Sprite[] RoseVines;

    // 일시정지 상태
    private bool isPaused = false;
    public bool IsPaused { get { return isPaused; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadGame();
        curScene = Array.FindIndex(Worlds[curWorld], x => x.Equals(SceneManager.GetActiveScene().name));
        if (curScene == -1)
        {
            curScene = 0;
        }
        StartStage();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartStage();
    }

    private void Update()
    {
        if (!isPaused && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void StartStage()
    {
        spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None);
        goals = FindObjectsByType<Goal>(FindObjectsSortMode.None);
    }

    public void CheckClear()
    {
        if (Array.TrueForAll(goals, goal => goal.isEntered))
        {
            Debug.Log("Clear");
            clearStages[curWorld] = Mathf.Max(clearStages[curWorld], curScene);
            SaveGame();
            GoNextStage();
        }
    }

    private void GoNextStage()
    {
        SceneManager.LoadScene(Worlds[curWorld][++curScene]);
    }

    private void SaveGame()
    {
        savedData = string.Join(",", clearStages);
        GameSave.SetEncryptedString("save", savedData);
    }

    private void LoadGame()
    {
        savedData = GameSave.GetEncryptedString("save");
        if (savedData != null)
        {
            clearStages = Array.ConvertAll(savedData.Split(','), int.Parse);
        }
        else
        {
            clearStages = new int [] { 0, 0, 0, 0};
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        Debug.Log($"Game Paused: {isPaused}");

        PauseMenuHandler pauseMenu = SceneController.Instance?.GetPauseMenuHandler();
        if (pauseMenu != null)
        {
            pauseMenu.SetMenuActive(isPaused);
        }
        else
        {
            Debug.LogWarning("PauseMenuHandler를 찾을 수 없습니다!");
        }

        // StageSelectScene에서는 커서 유지
        string currentScene = SceneManager.GetActiveScene().name;
    }
}

