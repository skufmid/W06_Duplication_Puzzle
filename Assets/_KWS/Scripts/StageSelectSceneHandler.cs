using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectSceneHandler : MonoBehaviour
{
    [SerializeField] int worldNumber = 1; // Inspector에서 월드 번호 설정 (1, 2, 3, 4)
    [SerializeField] GameObject worldNavigation; // 상단: 텍스트 (이동 버튼은 선택 사항)
    [SerializeField] GameObject stageButtons; // 중단: 스테이지 버튼 10개
    [SerializeField] GameObject backButton; // 하단: SelectWorldScene으로 돌아가는 버튼
    [SerializeField] TextMeshProUGUI worldText; // "World X" 또는 "Bonus Stage" 표시

    [SerializeField] string[] stageSceneNames; // Inspector에서 입력받을 씬 이름 배열

    bool isTransitioning = false;


    // 버튼 색상 상수
    Color activeColor = new Color32(255, 120, 120, 255);
    Color clearedColor = Color.white;
    Color disabledColor = new Color32(200, 200, 200, 255);


    void Start()
    {
        // 월드 번호 유효성 체크
        if (worldNumber < 1 || worldNumber > 4) // World 4까지 지원
        {
            Debug.LogWarning($"유효하지 않은 월드 번호: {worldNumber}. 기본값 1로 설정.");
            worldNumber = 1;
        }

        // 필수 오브젝트 확인
        if (worldNavigation == null || stageButtons == null || backButton == null)
        {
            Debug.LogError("필수 오브젝트가 할당되지 않았습니다!");
            return;
        }

        if (worldText == null)
        {
            Debug.LogError("World Text가 할당되지 않았습니다!");
            return;
        }

        if (stageSceneNames == null)
        {
            Debug.LogError("Stage Scene Names가 null입니다!");
            return;
        }

        // 월드 텍스트 설정
        worldText.text = worldNumber == 4 ? "Bonus Stage" : $"월드 {worldNumber}";

        // 스테이지 버튼 리스너 설정
        SetupStageButtons();

        // 뒤로 가기 버튼 리스너 설정
        SetupBackButton();

        // (선택 사항) 월드 이동 버튼 추가
        SetupWorldNavigationButtons();
    }

    private void SetupWorldNavigationButtons()
    {
        Button[] buttons = worldNavigation.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            if (button.name == "PrevWorldButton")
            {
                // World 1은 항상 해금됨, worldNumber > 1일 때만 활성화
                button.interactable = worldNumber > 1;
                if (button.interactable)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() =>
                    {
                        if (worldNumber <= 1)
                        {
                            Debug.Log("StageSelectSceneHandler: 이미 첫 번째 월드입니다!");
                            return;
                        }

                        int targetWorld = worldNumber - 1;
                        MoveToWorld(targetWorld);
                    });
                }
            }
            else if (button.name == "NextWorldButton")
            {
                // worldNumber가 최대 월드 미만이고, 다음 월드가 해금되었을 때만 활성화
                int maxWorlds = GameManager.Instance.Worlds.Count - 2; // Worlds 리스트의 길이 (0번 제외)
                bool isNextWorldUnlocked = worldNumber < maxWorlds && GameManager.Instance.IsWorldUnlocked(worldNumber + 1);
                button.interactable = isNextWorldUnlocked;
                Debug.Log($"NextWorldButton 활성화 여부: {isNextWorldUnlocked} (worldNumber = {worldNumber}, maxWorlds = {maxWorlds}, IsWorldUnlocked({worldNumber + 1}) = {isNextWorldUnlocked})");

                if (button.interactable)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() =>
                    {
                        if (worldNumber >= maxWorlds)
                        {
                            Debug.Log("StageSelectSceneHandler: 이미 마지막 월드입니다!");
                            return;
                        }

                        int targetWorld = worldNumber + 1;
                        if (!GameManager.Instance.IsWorldUnlocked(targetWorld))
                        {
                            Debug.Log($"StageSelectSceneHandler: World {targetWorld}는 해금되지 않았습니다! (clearStages[{targetWorld - 1}] = {GameManager.Instance.clearStages[targetWorld - 1]})");
                            return;
                        }

                        MoveToWorld(targetWorld);
                    });
                }
            }
        }
    }

    private void MoveToWorld(int targetWorld)
    {
        if (isTransitioning) return;
        isTransitioning = true;
        GameManager.Instance.CurWorld = targetWorld;
        SceneController.Instance.ChangeScene($"Stage{targetWorld}SelectScene");
        Debug.Log($"StageSelectSceneHandler: World {targetWorld}로 이동 - Stage{targetWorld}SelectScene");
    }

    private void SetupStageButtons()
    {
        Button[] buttons = stageButtons.GetComponentsInChildren<Button>();
        if (buttons.Length == 0)
        {
            Debug.LogWarning("StageButtons에 버튼이 없습니다!");
            return;
        }

        // 버튼과 stageSceneNames 길이 비교
        if (stageSceneNames.Length < buttons.Length)
        {
            Debug.LogWarning($"Stage Scene Names 길이({stageSceneNames.Length})가 버튼 수({buttons.Length})보다 작습니다. 남는 버튼은 비활성화됩니다.");
        }
        else if (stageSceneNames.Length > buttons.Length)
        {
            Debug.LogWarning($"Stage Scene Names 길이({stageSceneNames.Length})가 버튼 수({buttons.Length})보다 큽니다. 초과된 씬 이름은 무시됩니다.");
        }

        // 버튼과 씬 이름 매핑
        for (int i = 0; i < buttons.Length; i++)
        {
            // stageSceneNames가 더 짧거나 씬 이름이 비어 있는 경우
            if (i >= stageSceneNames.Length || string.IsNullOrEmpty(stageSceneNames[i]))
            {
                Debug.LogWarning($"StageButton{i + 1}에 대응하는 씬 이름이 비어 있습니다!");
                buttons[i].interactable = false;
                buttons[i].GetComponent<Image>().color = disabledColor;
                continue;
            }

            // World 4의 경우 첫 번째 스테이지만 처리
            if (worldNumber == 4 && i > 0)
            {
                buttons[i].interactable = false;
                buttons[i].GetComponent<Image>().color = disabledColor;
                continue;
            }

            // 스테이지 잠금 및 클리어 여부 확인
            bool isStageUnlocked = GameManager.Instance.IsStageUnlocked(worldNumber, i + 1);
            bool isStageCleared = GameManager.Instance.IsStageCleared(worldNumber, i + 1);

            // 버튼 상태 및 색상 설정
            buttons[i].interactable = isStageUnlocked;
            if (!isStageUnlocked)
            {
                buttons[i].GetComponent<Image>().color = disabledColor;
            }
            else if (isStageCleared)
            {
                buttons[i].GetComponent<Image>().color = clearedColor; // #FFFFFF
            }
            else
            {
                buttons[i].GetComponent<Image>().color = activeColor; // #FF7878
            }

            // 버튼 클릭 이벤트 설정
            if (isStageUnlocked)
            {
                string sceneName = stageSceneNames[i];
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => GoToStage(sceneName));
            }

            Debug.Log($"World {worldNumber} - Stage {i + 1}: Unlocked={isStageUnlocked}, Cleared={isStageCleared}, Color={buttons[i].GetComponent<Image>().color}");
        }
    }

    private void GoToStage(string sceneName)
    {
        if (isTransitioning) return;
        isTransitioning = true;
        Debug.Log($"스테이지로 이동: {sceneName}");
        SceneController.Instance.ChangeScene(sceneName);
    }

    private void SetupBackButton()
    {
        Button button = backButton.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("BackButton에 Button 컴포넌트가 없습니다!");
            return;
        }

        button.onClick.AddListener(GoToSelectWorldScene);
    }

    private void GoToSelectWorldScene()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        Debug.Log("SelectWorldScene으로 이동");
        SceneController.Instance.ChangeScene("WorldSelectScene");
    }
}