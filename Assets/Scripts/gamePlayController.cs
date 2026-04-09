using UnityEngine;
using UnityEngine.UIElements;

public class GamePlayUIController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    private Label scoreLabel;
    private VisualElement gameOverPanel;
    private Label finalScoreLabel;

    private void Awake()
    {
        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }
    }

    private void OnEnable()
    {
        VisualElement root = uiDocument.rootVisualElement;

        scoreLabel = root.Q<Label>("scoreLabel");
        gameOverPanel = root.Q<VisualElement>("gameOverPanel");
        finalScoreLabel = root.Q<Label>("finalScoreLabel");

        SetScore(0);
        ShowGameOver(false, 0);
    }

    public void SetScore(int score)
    {
        if (scoreLabel != null)
        {
            scoreLabel.text = $"Score: {score}";
        }
    }

    public void ShowGameOver(bool show, int finalScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }

        if (finalScoreLabel != null)
        {
            finalScoreLabel.text = $"Score: {finalScore}";
        }
    }
}
