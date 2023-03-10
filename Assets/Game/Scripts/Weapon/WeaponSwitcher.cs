using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] int currentWeapon = 0;
    bool canSwitch = true;
    void Start()
    {
        SetActiveWeapon();
    }
    void Update()
    {
        int previousWeapon = currentWeapon;

        ProcessKeyInput();
        ProcessScrollInput();
        
        if(previousWeapon != currentWeapon)
        {
            SetActiveWeapon();
        }
    }

    void ProcessScrollInput()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && canSwitch)
        {
            if(currentWeapon >= transform.childCount - 1)
            {
                currentWeapon = 0;
            }
            else {currentWeapon++;}
        }

        if(Input.GetAxis("Mouse ScrollWheel") < 0 && canSwitch)
        {
            if(currentWeapon <= 0)
            {
                currentWeapon = transform.childCount -1;
            }
            else {currentWeapon--;}
        }

    }

    public void SetCanSwitch(bool isAiming)
    {
        // canSwitch = false;
        if(isAiming)
        {
            canSwitch = false;
        }
        else
        {
            canSwitch = true;
        }
    }

    void SetActiveWeapon()
    {
        int weaponIndex = 0;

        foreach(Transform weapon in transform)
        {
            if(weaponIndex == currentWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            weaponIndex++;
        }
    }
    
    void ProcessKeyInput()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1) && canSwitch)
        {
            currentWeapon = 0;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2) && canSwitch)
        {
            currentWeapon = 1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3) && canSwitch)
        {
            currentWeapon = 2;
        }
    }
}
