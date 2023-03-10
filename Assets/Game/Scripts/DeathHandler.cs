using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    [SerializeField] Canvas deathUI;
    PlayerHealth playerHealth;
    void Start()
    {
        deathUI.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // print("LockState - " + Cursor.lockState);
        // print("Visibility - " + Cursor.visible);
    }
    public void ShowDeathScreen()
    {
        Time.timeScale = 0;
        // FindObjectOfType<WeaponSwitcher>().enabled = false;
        deathUI.enabled = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
