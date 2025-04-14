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

        new string[] {"",
            "World1_1",
            "World1_2",
            "World1_3",
            "World1_4",
            "World1_5",
            "World1_6",
            "World1_7",
            "World1_8",
            "World1_9"
        },
        new string[] {"",
            "World2_1",
            "World2_2",
            "World2_3",
            "World2_4",
            "World2_5",
            "World2_6",
            "World2_7",
            "World2_8",
            "World2_9",
        },
        new string[] {"",
            "World3_1",
            "World3_2",
            "World3_3",
            "World3_4",
            "World3_5",
            "World3_6",
            "World3_7",
            "World3_8",
            "World3_9"
        },
        new string[] { "",
            "World4_1"
        }
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
        // 진행상태 초기화
        //GameSave.ClearSaveData("save");

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
        // 씬 로드 후 curScene 업데이트
        curScene = Array.FindIndex(Worlds[curWorld], x => x.Equals(scene.name));
        Debug.Log($"OnSceneLoaded: curWorld = {curWorld}, curScene = {curScene}, 로드된 씬 = {scene.name}");
        if (curScene == -1)
        {
            Debug.LogWarning($"curScene이 -1로 설정됨. 씬 이름 확인 필요: {scene.name}");
        }
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
            Debug.Log($"스테이지 클리어: World {curWorld}, Stage {curScene}");
            int previousClearStages = clearStages[curWorld];
            clearStages[curWorld] = Mathf.Max(clearStages[curWorld], curScene);
            Debug.Log($"clearStages[{curWorld}] 업데이트: {previousClearStages} -> {clearStages[curWorld]}");
            SaveGame();
            SceneController.Instance.ChangeScene($"Stage{curWorld}SelectScene");
        }
    }

    private void GoNextStage()
    {
        SceneController.Instance.ChangeScene(Worlds[curWorld][++curScene]);
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
            try
            {
                clearStages = Array.ConvertAll(savedData.Split(','), int.Parse);
                if (clearStages.Length != Worlds.Count)
                {
                    Debug.LogWarning($"clearStages 길이 불일치: 예상 {Worlds.Count}, 실제 {clearStages.Length}. 초기화합니다.");
                    clearStages = new int[] { 0, 0, 0, 0 };
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"clearStages 파싱 실패: {e.Message}. 초기화합니다.");
                clearStages = new int[] { 0, 0, 0, 0 };
            }
        }
        else
        {
            // World 1의 Stage 1만 활성화 상태
            clearStages = new int[] { 0, 0, 0, 0 };
            Debug.Log($"저장 데이터가 없어 초기화됨: World 1의 Stage 1만 활성화 상태 (clearStages: {string.Join(",", clearStages)})");
            SaveGame();
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


    // 월드가 잠금 해제되었는지 확인
    public bool IsWorldUnlocked(int worldNumber)
    {
        if (worldNumber == 1) return true;

        int previousWorld = worldNumber - 1;
        int requiredStages;

        // World 2: World 1에서 4개 이상 스테이지 클리어
        // World 3: World 2에서 4개 이상 스테이지 클리어
        requiredStages = 5;

        bool isUnlocked = clearStages[previousWorld] >= requiredStages;
        Debug.Log($"World {worldNumber} 잠금 해제 여부: {isUnlocked} (clearStages[{previousWorld}] = {clearStages[previousWorld]}, requiredStages = {requiredStages})");
        return isUnlocked;
    }

    // 스테이지가 잠금 해제되었는지 확인
    public bool IsStageUnlocked(int worldNumber, int stageIndex)
    {
        if (stageIndex == 1) return true;
        bool isUnlocked = clearStages[worldNumber] >= (stageIndex - 1);
        Debug.Log($"World {worldNumber} - Stage {stageIndex} 잠금 해제 여부: {isUnlocked} (clearStages[{worldNumber}] = {clearStages[worldNumber]})");
        return isUnlocked;
    }
}

