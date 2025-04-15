using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectSceneHandler : MonoBehaviour
{
    [SerializeField] int worldNumber = 1; // Inspector���� ���� ��ȣ ���� (1, 2, 3, 4)
    [SerializeField] GameObject worldNavigation; // ���: �ؽ�Ʈ (�̵� ��ư�� ���� ����)
    [SerializeField] GameObject stageButtons; // �ߴ�: �������� ��ư 10��
    [SerializeField] GameObject backButton; // �ϴ�: SelectWorldScene���� ���ư��� ��ư
    [SerializeField] TextMeshProUGUI worldText; // "World X" �Ǵ� "Bonus Stage" ǥ��

    [SerializeField] string[] stageSceneNames; // Inspector���� �Է¹��� �� �̸� �迭

    bool isTransitioning = false;


    // ��ư ���� ���
    Color activeColor = new Color32(255, 120, 120, 255);
    Color clearedColor = Color.white;
    Color disabledColor = new Color32(200, 200, 200, 255);


    void Start()
    {
        // ���� ��ȣ ��ȿ�� üũ
        if (worldNumber < 1 || worldNumber > 4) // World 4���� ����
        {
            Debug.LogWarning($"��ȿ���� ���� ���� ��ȣ: {worldNumber}. �⺻�� 1�� ����.");
            worldNumber = 1;
        }

        // �ʼ� ������Ʈ Ȯ��
        if (worldNavigation == null || stageButtons == null || backButton == null)
        {
            Debug.LogError("�ʼ� ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        if (worldText == null)
        {
            Debug.LogError("World Text�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        if (stageSceneNames == null)
        {
            Debug.LogError("Stage Scene Names�� null�Դϴ�!");
            return;
        }

        // ���� �ؽ�Ʈ ����
        worldText.text = worldNumber == 4 ? "Bonus Stage" : $"���� {worldNumber}";

        // �������� ��ư ������ ����
        SetupStageButtons();

        // �ڷ� ���� ��ư ������ ����
        SetupBackButton();

        // (���� ����) ���� �̵� ��ư �߰�
        SetupWorldNavigationButtons();
    }

    private void SetupWorldNavigationButtons()
    {
        Button[] buttons = worldNavigation.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            if (button.name == "PrevWorldButton")
            {
                // World 1�� �׻� �رݵ�, worldNumber > 1�� ���� Ȱ��ȭ
                button.interactable = worldNumber > 1;
                if (button.interactable)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() =>
                    {
                        if (worldNumber <= 1)
                        {
                            Debug.Log("StageSelectSceneHandler: �̹� ù ��° �����Դϴ�!");
                            return;
                        }

                        int targetWorld = worldNumber - 1;
                        MoveToWorld(targetWorld);
                    });
                }
            }
            else if (button.name == "NextWorldButton")
            {
                // worldNumber�� �ִ� ���� �̸��̰�, ���� ���尡 �رݵǾ��� ���� Ȱ��ȭ
                int maxWorlds = GameManager.Instance.Worlds.Count - 2; // Worlds ����Ʈ�� ���� (0�� ����)
                bool isNextWorldUnlocked = worldNumber < maxWorlds && GameManager.Instance.IsWorldUnlocked(worldNumber + 1);
                button.interactable = isNextWorldUnlocked;
                Debug.Log($"NextWorldButton Ȱ��ȭ ����: {isNextWorldUnlocked} (worldNumber = {worldNumber}, maxWorlds = {maxWorlds}, IsWorldUnlocked({worldNumber + 1}) = {isNextWorldUnlocked})");

                if (button.interactable)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() =>
                    {
                        if (worldNumber >= maxWorlds)
                        {
                            Debug.Log("StageSelectSceneHandler: �̹� ������ �����Դϴ�!");
                            return;
                        }

                        int targetWorld = worldNumber + 1;
                        if (!GameManager.Instance.IsWorldUnlocked(targetWorld))
                        {
                            Debug.Log($"StageSelectSceneHandler: World {targetWorld}�� �رݵ��� �ʾҽ��ϴ�! (clearStages[{targetWorld - 1}] = {GameManager.Instance.clearStages[targetWorld - 1]})");
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
        Debug.Log($"StageSelectSceneHandler: World {targetWorld}�� �̵� - Stage{targetWorld}SelectScene");
    }

    private void SetupStageButtons()
    {
        Button[] buttons = stageButtons.GetComponentsInChildren<Button>();
        if (buttons.Length == 0)
        {
            Debug.LogWarning("StageButtons�� ��ư�� �����ϴ�!");
            return;
        }

        // ��ư�� stageSceneNames ���� ��
        if (stageSceneNames.Length < buttons.Length)
        {
            Debug.LogWarning($"Stage Scene Names ����({stageSceneNames.Length})�� ��ư ��({buttons.Length})���� �۽��ϴ�. ���� ��ư�� ��Ȱ��ȭ�˴ϴ�.");
        }
        else if (stageSceneNames.Length > buttons.Length)
        {
            Debug.LogWarning($"Stage Scene Names ����({stageSceneNames.Length})�� ��ư ��({buttons.Length})���� Ů�ϴ�. �ʰ��� �� �̸��� ���õ˴ϴ�.");
        }

        // ��ư�� �� �̸� ����
        for (int i = 0; i < buttons.Length; i++)
        {
            // stageSceneNames�� �� ª�ų� �� �̸��� ��� �ִ� ���
            if (i >= stageSceneNames.Length || string.IsNullOrEmpty(stageSceneNames[i]))
            {
                Debug.LogWarning($"StageButton{i + 1}�� �����ϴ� �� �̸��� ��� �ֽ��ϴ�!");
                buttons[i].interactable = false;
                buttons[i].GetComponent<Image>().color = disabledColor;
                continue;
            }

            // World 4�� ��� ù ��° ���������� ó��
            if (worldNumber == 4 && i > 0)
            {
                buttons[i].interactable = false;
                buttons[i].GetComponent<Image>().color = disabledColor;
                continue;
            }

            // �������� ��� �� Ŭ���� ���� Ȯ��
            bool isStageUnlocked = GameManager.Instance.IsStageUnlocked(worldNumber, i + 1);
            bool isStageCleared = GameManager.Instance.IsStageCleared(worldNumber, i + 1);

            // ��ư ���� �� ���� ����
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

            // ��ư Ŭ�� �̺�Ʈ ����
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
        Debug.Log($"���������� �̵�: {sceneName}");
        SceneController.Instance.ChangeScene(sceneName);
    }

    private void SetupBackButton()
    {
        Button button = backButton.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("BackButton�� Button ������Ʈ�� �����ϴ�!");
            return;
        }

        button.onClick.AddListener(GoToSelectWorldScene);
    }

    private void GoToSelectWorldScene()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        Debug.Log("SelectWorldScene���� �̵�");
        SceneController.Instance.ChangeScene("WorldSelectScene");
    }
}