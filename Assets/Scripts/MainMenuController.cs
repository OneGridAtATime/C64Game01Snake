using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    private Button startButton;
    private Button quitButton;

    private void OnEnable()
    {
        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }

        VisualElement root = uiDocument.rootVisualElement;

        startButton = root.Q<Button>("startButton");
        quitButton = root.Q<Button>("quitButton");

        if (startButton != null)
        {
            startButton.clicked += OnStartClicked;
        }

        if (quitButton != null)
        {
            quitButton.clicked += OnQuitClicked;
        }
    }

    private void OnDisable()
    {
        if (startButton != null)
        {
            startButton.clicked -= OnStartClicked;
        }

        if (quitButton != null)
        {
            quitButton.clicked -= OnQuitClicked;
        }
    }

    private void OnStartClicked()
    {
        Debug.Log("Start clicked");

        // change "game" if your scene name is different
        SceneManager.LoadScene("gamePlay");
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit clicked");

        Application.Quit();
    }
}