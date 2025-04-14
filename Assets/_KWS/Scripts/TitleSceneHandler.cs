using TMPro;
using UnityEngine;

public class TitleSceneHandler : MonoBehaviour
{
    [SerializeField] string nextScene = "WorldSelectScene";
    bool isTransitioning = false;

    [SerializeField] TextMeshProUGUI continueText;
    [SerializeField] float blinkSpeed = 10f;

    private void Update()
    {
        if (isTransitioning) return;

        // Continue text ������ ȿ��
        if (continueText != null)
        {
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            Color textColor = continueText.color;
            textColor.a = alpha;
            continueText.color = textColor;
        }

        // Ű �Է½� �������� �Ѿ
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            isTransitioning = true;
            SceneController.Instance.ChangeScene(nextScene);
        }
    }
}
