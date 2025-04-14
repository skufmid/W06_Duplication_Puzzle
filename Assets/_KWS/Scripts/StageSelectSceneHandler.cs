using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectSceneHandler : MonoBehaviour
{
    [SerializeField] int worldNumber = 1; // Inspector에서 월드 번호 설정 (1, 2, 3)
    [SerializeField] GameObject worldNavigation; // 상단: 텍스트 (이동 버튼은 선택 사항)
    [SerializeField] GameObject stageButtons; // 중단: 스테이지 버튼 10개
    [SerializeField] GameObject backButton; // 하단: SelectWorldScene으로 돌아가는 버튼
    [SerializeField] TextMeshProUGUI worldText; // "World X" 표시

    [SerializeField] string[] stageSceneNames; // Inspector에서 입력받을 씬 이름 배열

    bool isTransitioning = false;

    void Start()
    {
        // 월드 번호 유효성 체크
        if (worldNumber < 1 || worldNumber > 3)
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
        worldText.text = $"World {worldNumber}";

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
                // worldNumber가 1이면 PrevWorldButton 비활성화
                button.interactable = (worldNumber > 1);
                if (button.interactable)
                {
                    button.onClick.AddListener(() =>
                    {
                        if (worldNumber > 1)
                        {
                            GameManager.Instance.CurWorld = worldNumber - 1;
                            SceneController.Instance.ChangeScene($"Stage{worldNumber - 1}SelectScene");
                        }
                    });
                }
            }
            else if (button.name == "NextWorldButton")
            {
                // worldNumber가 3이면 NextWorldButton 비활성화
                button.interactable = (worldNumber < 3);
                if (button.interactable)
                {
                    button.onClick.AddListener(() =>
                    {
                        if (worldNumber < 3)
                        {
                            GameManager.Instance.CurWorld = worldNumber + 1;
                            SceneController.Instance.ChangeScene($"Stage{worldNumber + 1}SelectScene");
                        }
                    });
                }
            }
        }
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
            // stageSceneNames가 더 짧아서 해당 인덱스가 없거나, 씬 이름이 비어 있는 경우
            if (i >= stageSceneNames.Length || string.IsNullOrEmpty(stageSceneNames[i]))
            {
                Debug.LogWarning($"StageButton{i + 1}에 대응하는 씬 이름이 비어 있습니다!");
                buttons[i].interactable = false;
                continue;
            }

            string sceneName = stageSceneNames[i];
            buttons[i].interactable = true; // 씬 이름이 있으면 활성화
            buttons[i].onClick.RemoveAllListeners(); // 기존 리스너 제거
            buttons[i].onClick.AddListener(() => GoToStage(sceneName));
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