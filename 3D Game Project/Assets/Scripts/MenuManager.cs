using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject petals;
    [SerializeField] private GameObject optionsMenu;

    public void OnPlayButton() {
        SceneManager.LoadScene(1);
    }

    public void OpenOptions() {
        petals.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void CloseOptions() {
        optionsMenu.SetActive(false);
        petals.SetActive(true);
    }

    public void OnQuitButton() {
        Application.Quit();
    }
}
