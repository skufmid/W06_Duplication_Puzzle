using UnityEngine;
using UnityEngine.UI;

public class WorldSelectSceneHandler : MonoBehaviour
{
    [SerializeField] GameObject buttons; // World 1, 2, 3 ��ư �θ� ������Ʈ
    [SerializeField] GameObject howToPlayButton; // "How to Play" ��ư
    [SerializeField] GameObject stage4Button; // World 4 ��ư (Inspector���� ��Ȱ��ȭ ���·� ����)
    bool isTransitioning = false;

    private void Start()
    {
        if (buttons == null)
        {
            Debug.LogError("Buttons ���Ҵ� ���� on WorldSelectSceneHandler");
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
            Debug.LogWarning($"��ư 3���� �ʿ��մϴ�. �Ҵ�� ��ư �� : {buttonArray.Length}");
        }

        foreach (Button button in buttonArray)
        {
            if (int.TryParse(button.name.Replace("Select", "").Replace("World", "").Replace("Button", ""), out int worldNumber))
            {
                bool isUnlocked = GameManager.Instance.IsWorldUnlocked(worldNumber);
                button.interactable = isUnlocked;
                Debug.Log($"World {worldNumber} ��ư Ȱ��ȭ ����: {isUnlocked}");

                if (isUnlocked)
                {
                    string sceneName = $"Stage{worldNumber}SelectScene";
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() =>
                    {
                        GameManager.Instance.CurWorld = worldNumber; // Ŭ�� �� CurWorld ����
                        OnWorldButtonClick(sceneName);
                    });
                }
            }
        }
    }

    private void UpdateBonusStageUI()
    {
        bool allCleared = GameManager.Instance.IsAllMainStagesCleared;
        Debug.Log($"UpdateBonusStageUI: ��� ���� �������� Ŭ���� ���� = {allCleared}");

        if (howToPlayButton != null)
        {
            howToPlayButton.SetActive(!allCleared);
        }
        else
        {
            Debug.LogWarning("How to Play ��ư�� �Ҵ���� �ʾҽ��ϴ�!");
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
            Debug.LogWarning("Stage 4 ��ư�� �Ҵ���� �ʾҽ��ϴ�!");
        }
    }

    private void OnWorldButtonClick(string sceneName)
    {
        if (isTransitioning) return;
        isTransitioning = true;
        SceneController.Instance.ChangeScene(sceneName);
    }
}