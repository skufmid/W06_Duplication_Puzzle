using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectSceneHandler : MonoBehaviour
{
    [SerializeField] int worldNumber = 1; // Inspector���� ���� ��ȣ ���� (1, 2, 3)
    [SerializeField] GameObject worldNavigation; // ���: �ؽ�Ʈ (�̵� ��ư�� ���� ����)
    [SerializeField] GameObject stageButtons; // �ߴ�: �������� ��ư 10��
    [SerializeField] GameObject backButton; // �ϴ�: SelectWorldScene���� ���ư��� ��ư
    [SerializeField] TextMeshProUGUI worldText; // "World X" ǥ��

    [SerializeField] string[] stageSceneNames; // Inspector���� �Է¹��� �� �̸� �迭

    bool isTransitioning = false;

    void Start()
    {
        // ���� ��ȣ ��ȿ�� üũ
        if (worldNumber < 1 || worldNumber > 3)
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
        worldText.text = $"World {worldNumber}";

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
                // worldNumber�� 1�̸� PrevWorldButton ��Ȱ��ȭ
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
                // worldNumber�� 3�̸� NextWorldButton ��Ȱ��ȭ
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
            // stageSceneNames�� �� ª�Ƽ� �ش� �ε����� ���ų�, �� �̸��� ��� �ִ� ���
            if (i >= stageSceneNames.Length || string.IsNullOrEmpty(stageSceneNames[i]))
            {
                Debug.LogWarning($"StageButton{i + 1}�� �����ϴ� �� �̸��� ��� �ֽ��ϴ�!");
                buttons[i].interactable = false;
                continue;
            }

            string sceneName = stageSceneNames[i];
            buttons[i].interactable = true; // �� �̸��� ������ Ȱ��ȭ
            buttons[i].onClick.RemoveAllListeners(); // ���� ������ ����
            buttons[i].onClick.AddListener(() => GoToStage(sceneName));
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