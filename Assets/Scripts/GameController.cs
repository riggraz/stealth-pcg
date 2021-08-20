using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Generator generator;
    public Instantiator instantiator;
    public Orchestrator orchestrator;

    public Text levelStatusText;
    public Text seedText;
    public Button nextLevelButton;

    private bool levelCompleted = false;

    private string randomState;

    public void OnLevelCompleted()
    {
        levelCompleted = true;
        nextLevelButton.interactable = false;
        StartCoroutine(OnNextLevelCoroutine());
    }

    IEnumerator OnNextLevelCoroutine()
    {
        levelStatusText.color = Color.green;

        for (int i = 3; i > 0; i--)
        {
            levelStatusText.text = "Completed! New level in " + i + " seconds...";
            yield return new WaitForSeconds(1);
        }

        OnNextLevel();
        levelCompleted = false;
        nextLevelButton.interactable = true;
    }

    public void OnNextLevel()
    {
        levelStatusText.color = Color.black;
        levelStatusText.text = "Generating...";

        orchestrator.Reset();
        instantiator.DestroyLevel();
        generator.GenerateLevel();
    }

    public void OnCopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = randomState;
    }

    public void SetRandomState(string randomState)
    {
        this.randomState = randomState;
        seedText.text = randomState;
    }

    public string GetRandomState()
    {
        return randomState;
    }

    public bool IsLevelCompleted()
    {
        return levelCompleted;
    }

}
