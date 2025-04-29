using UnityEngine;
using UnityEngine.UI;

public class NextWaveButton : MonoBehaviour
{
    [SerializeField] private Button nextWaveButton;

    private void Awake()
    {
        if (nextWaveButton == null)
            nextWaveButton = GetComponent<Button>();
    }

    private void Start()
    {
        GameManager.I.OnWaveStarted += HideButton;
        GameManager.I.OnWaveEnded += ShowButton;

        HideButton();
    }

    private void OnDestroy()
    {
        if (GameManager.I != null)
        {
            GameManager.I.OnWaveStarted -= HideButton;
            GameManager.I.OnWaveEnded -= ShowButton;
        }
    }

    private void OnNextWaveClicked()
    {
        nextWaveButton.interactable = false;
        GameManager.I.NextWave();
    }

    private void ShowButton()
    {
        nextWaveButton.gameObject.SetActive(true);
        nextWaveButton.interactable = true;
        nextWaveButton.onClick.RemoveAllListeners();
        nextWaveButton.onClick.AddListener(OnNextWaveClicked);
    }

    private void HideButton()
    {
        nextWaveButton.gameObject.SetActive(false);
    }
}
