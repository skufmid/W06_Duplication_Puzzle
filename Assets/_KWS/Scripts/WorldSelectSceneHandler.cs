using UnityEngine;
using UnityEngine.UI;

public class WorldSelectSceneHandler : MonoBehaviour
{
    [SerializeField] GameObject buttons;
    bool isTransitioning = false;

    private void Start()
    {
        if (buttons == null)
        {
            Debug.LogError("Buttons ���Ҵ� ���� on WorldSelectSceneHandler");
            return;
        }

        Button[] buttonArray = buttons.GetComponentsInChildren<Button>();

        if (buttonArray.Length != 3)
        {
            Debug.LogWarning($"��ư 3���� �ʿ��մϴ�. �Ҵ�� ��ư �� : {buttonArray.Length}");
        }


        foreach (Button button in buttonArray)
        {
            if (int.TryParse(button.name.Replace("Select", "").Replace("World", "").Replace("Button", ""), out int worldNumber))
            {
                // ���� ��� ���� Ȯ��
                bool isUnlocked = GameManager.Instance.IsWorldUnlocked(worldNumber);
                button.interactable = isUnlocked;
                Debug.Log($"World {worldNumber} ��ư Ȱ��ȭ ����: {isUnlocked}");

                if (isUnlocked)
                {
                    GameManager.Instance.CurWorld = worldNumber;
                    string sceneName = $"Stage{worldNumber}SelectScene";
                    button.onClick.AddListener(() => OnWorldButtonClick(sceneName));
                }
            }
        }
    }

    private void OnWorldButtonClick(string sceneName)
    {
        if (isTransitioning) return;
        isTransitioning = true;
        SceneController.Instance.ChangeScene(sceneName);
    }
}
