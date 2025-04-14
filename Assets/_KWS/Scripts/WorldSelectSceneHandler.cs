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
            Debug.LogError("Buttons 미할당 에러 on WorldSelectSceneHandler");
            return;
        }

        Button[] buttonArray = buttons.GetComponentsInChildren<Button>();

        if (buttonArray.Length != 3)
        {
            Debug.LogWarning($"버튼 3개가 필요합니다. 할당된 버튼 수 : {buttonArray.Length}");
        }


        foreach (Button button in buttonArray)
        {
            if (int.TryParse(button.name.Replace("Select", "").Replace("World", "").Replace("Button", ""), out int worldNumber))
            {
                // 월드 잠금 여부 확인
                bool isUnlocked = GameManager.Instance.IsWorldUnlocked(worldNumber);
                button.interactable = isUnlocked;
                Debug.Log($"World {worldNumber} 버튼 활성화 상태: {isUnlocked}");

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
