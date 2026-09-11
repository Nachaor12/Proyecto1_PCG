using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuPanel;
    public CityController cityController;

    void Update()
    {
        // Alternar pausa con la tecla espacio
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        isPaused = true;
    }

    // Este método se enlazará al botón "Reconstruir" de tu UI
    public void RebuildCity()
    {
        // Llama a la regeneración de la ciudad
        cityController.RunProceduralGeneration();
        Resume();
    }
}