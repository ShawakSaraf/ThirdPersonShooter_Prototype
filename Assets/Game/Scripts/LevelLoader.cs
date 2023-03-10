using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelLoader : MonoBehaviour
{
    // [SerializeField] Button retry, quite;

    void Start()
    {
        // retry.onClick.AddListener(OnClick);
    }

    public void Reload()
    {
        print("Loading");
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Quit()
    {
        print("Quiting");
        Application.Quit();
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
