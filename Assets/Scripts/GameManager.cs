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


    // 보너스 스테이지용 필드
    bool isAllMainStagesCleared = false;
    public bool IsAllMainStagesCleared => isAllMainStagesCleared;

    [SerializeField] GameObject congratulationCanvas;



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


        // World 1~3의 1~8 스테이지 클리어 상태로 설정
        //clearStages = new int[] { 0, 8, 8, 8, 0 }; // World 0, World 1, World 2, World 3, World 4
        //isAllMainStagesCleared = AreAllMainStagesCleared(); // false가 되어야 함
        //Debug.Log($"Start: clearStages = [{string.Join(", ", clearStages)}], isAllMainStagesCleared = {isAllMainStagesCleared}");
        //SaveGame();


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

            // World 1~3 모든 스테이지 클리어 확인
            bool wasAllCleared = isAllMainStagesCleared;
            isAllMainStagesCleared = AreAllMainStagesCleared();
            if (!wasAllCleared && isAllMainStagesCleared)
            {
                Debug.Log("모든 메인 스테이지 클리어 상태로 전환!");
            }

            SaveGame(); // 상태 변경 후 즉시 저장

            // World 4 클리어 시 축하 캔버스 표시
            if (curWorld == 4 && curScene == 1)
            {
                ShowCongratulationCanvas();
                return;
            }

            SceneController.Instance.ChangeScene($"Stage{curWorld}SelectScene");
        }
    }

    private bool AreAllMainStagesCleared()
    {
        for (int i = 1; i < Worlds.Count - 1; i++) // World 0 제외, World 4 제외
        {
            int lastStageIndex = Worlds[i].Length - 1;
            if (clearStages[i] < lastStageIndex)
            {
                Debug.Log($"모든 메인 스테이지 클리어 아님: World {i}, clearStages[{i}] = {clearStages[i]}, 마지막 스테이지 = {lastStageIndex}");
                return false;
            }
        }
        Debug.Log("모든 메인 스테이지 클리어 확인됨!");
        return true;
    }

    private void ShowCongratulationCanvas()
    {
        if (congratulationCanvas != null)
        {
            //congratulationCanvas.SetActive(true);
            Instantiate(congratulationCanvas);
            Debug.Log("축하 캔버스 표시: 모든 맵 클리어!");
        }
        else
        {
            Debug.LogWarning("축하 캔버스가 할당되지 않았습니다!");
            SceneController.Instance.ChangeScene("WorldSelectScene");
        }
    }

    private void GoNextStage()
    {
        SceneController.Instance.ChangeScene(Worlds[curWorld][++curScene]);
    }

    private void SaveGame()
    {
        savedData = string.Join(",", clearStages) + "|" + isAllMainStagesCleared.ToString();
        GameSave.SetEncryptedString("save", savedData);
    }

    private void LoadGame()
    {
        savedData = GameSave.GetEncryptedString("save");
        if (savedData != null)
        {
            try
            {
                string[] parts = savedData.Split('|');
                if (parts.Length < 1)
                {
                    throw new Exception("savedData 형식이 잘못됨: 구분자 '|' 누락");
                }

                clearStages = Array.ConvertAll(parts[0].Split(','), int.Parse);
                if (clearStages.Length != Worlds.Count)
                {
                    Debug.LogWarning($"clearStages 길이 불일치: 예상 {Worlds.Count}, 실제 {clearStages.Length}. 초기화합니다.");
                    clearStages = new int[Worlds.Count]; // 기본값 0으로 초기화
                }

                // isAllMainStagesCleared 로드
                if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]))
                {
                    isAllMainStagesCleared = bool.Parse(parts[1]);
                }
                else
                {
                    isAllMainStagesCleared = AreAllMainStagesCleared(); // 저장된 값 없으면 계산
                    Debug.Log($"isAllMainStagesCleared 값 없음. 계산 결과: {isAllMainStagesCleared}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"저장 데이터 파싱 실패: {e.Message}. 초기화합니다.");
                clearStages = new int[Worlds.Count]; // Worlds와 길이 맞춤
                isAllMainStagesCleared = false;
            }
        }
        else
        {
            clearStages = new int[Worlds.Count]; // Worlds와 길이 맞춤
            isAllMainStagesCleared = false;
            Debug.Log($"저장 데이터 없음. 초기화: clearStages = [{string.Join(",", clearStages)}], isAllMainStagesCleared = {isAllMainStagesCleared}");
        }

        // 로드 후 올클리어 상태 검증
        bool calculatedAllCleared = AreAllMainStagesCleared();
        if (isAllMainStagesCleared != calculatedAllCleared)
        {
            Debug.LogWarning($"isAllMainStagesCleared 불일치: 저장된 값 = {isAllMainStagesCleared}, 계산된 값 = {calculatedAllCleared}. 계산된 값으로 갱신");
            isAllMainStagesCleared = calculatedAllCleared;
            SaveGame(); // 불일치 시 저장
        }
        else
        {
            Debug.Log($"LoadGame: clearStages = [{string.Join(",", clearStages)}], isAllMainStagesCleared = {isAllMainStagesCleared}");
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

        // World 4: World 1~3 모든 스테이지 클리어 후 해금
        if (worldNumber == 4)
        {
            bool unlocked = isAllMainStagesCleared;
            Debug.Log($"World 4 잠금 해제 여부: {unlocked} (isAllMainStagesCleared = {isAllMainStagesCleared})");
            return unlocked;
        }

        int previousWorld = worldNumber - 1;
        int requiredStages = 5;

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

    
    // 스테이지 클리어 여부 검사
    public bool IsStageCleared(int worldNumber, int stageIndex)
    {
        bool isCleared = clearStages[worldNumber] >= stageIndex;
        Debug.Log($"World {worldNumber} - Stage {stageIndex} 클리어 여부: {isCleared} (clearStages[{worldNumber}] = {clearStages[worldNumber]})");
        return isCleared;
    }
}

