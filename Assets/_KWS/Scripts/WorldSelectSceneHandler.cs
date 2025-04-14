using UnityEngine;
using UnityEngine.UI;

public class WorldSelectSceneHandler : MonoBehaviour
{
    [SerializeField] GameObject buttons; // World 1, 2, 3 버튼 부모 오브젝트
    [SerializeField] GameObject howToPlayButton; // "How to Play" 버튼
    [SerializeField] GameObject stage4Button; // World 4 버튼 (Inspector에서 비활성화 상태로 시작)
    bool isTransitioning = false;

    private void Start()
    {
        if (buttons == null)
        {
            Debug.LogError("Buttons 미할당 에러 on WorldSelectSceneHandler");
            return;
        }

        SetupWorldButtons();
        UpdateBonusStageUI();
    }

    private void SetupWorldButtons()
    {
        Button[] buttonArray = buttons.GetComponentsInChildren<Button>();

        if (buttonArray.Length != 3)
        {
            Debug.LogWarning($"버튼 3개가 필요합니다. 할당된 버튼 수 : {buttonArray.Length}");
        }

        foreach (Button button in buttonArray)
        {
            if (int.TryParse(button.name.Replace("Select", "").Replace("World", "").Replace("Button", ""), out int worldNumber))
            {
                bool isUnlocked = GameManager.Instance.IsWorldUnlocked(worldNumber);
                button.interactable = isUnlocked;
                Debug.Log($"World {worldNumber} 버튼 활성화 상태: {isUnlocked}");

                if (isUnlocked)
                {
                    string sceneName = $"Stage{worldNumber}SelectScene";
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() =>
                    {
                        GameManager.Instance.CurWorld = worldNumber; // 클릭 시 CurWorld 설정
                        OnWorldButtonClick(sceneName);
                    });
                }
            }
        }
    }

    private void UpdateBonusStageUI()
    {
        bool allCleared = GameManager.Instance.IsAllMainStagesCleared;
        Debug.Log($"UpdateBonusStageUI: 모든 메인 스테이지 클리어 여부 = {allCleared}");

        if (howToPlayButton != null)
        {
            howToPlayButton.SetActive(!allCleared);
        }
        else
        {
            Debug.LogWarning("How to Play 버튼이 할당되지 않았습니다!");
        }

        if (stage4Button != null)
        {
            stage4Button.SetActive(allCleared);
            if (allCleared)
            {
                Button button = stage4Button.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() =>
                    {
                        if (isTransitioning) return;
                        isTransitioning = true;
                        GameManager.Instance.CurWorld = 4;
                        SceneController.Instance.ChangeScene("World4_1");
                    });
                }
            }
        }
        else
        {
            Debug.LogWarning("Stage 4 버튼이 할당되지 않았습니다!");
        }
    }

    private void OnWorldButtonClick(string sceneName)
    {
        if (isTransitioning) return;
        isTransitioning = true;
        SceneController.Instance.ChangeScene(sceneName);
    }
}