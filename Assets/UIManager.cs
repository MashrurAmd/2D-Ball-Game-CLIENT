using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel; // Assign your Settings Panel here

    private bool isPaused = false;

    // Called when the Settings button is pressed
    public void ToggleSettingsPanel()
    {
        isPaused = !isPaused;
        settingsPanel.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f; // Pause the game
        }
        else
        {
            Time.timeScale = 1f; // Resume the game
        }
    }

    // Optional: Call this from a "Close" button inside the settings panel
    public void CloseSettingsPanel()
    {
        isPaused = false;
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
