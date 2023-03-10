using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] int ammoAmount = 5;
    [SerializeField] AmmoType ammoType;
    Ammo ammo;

    void Start()
    {
        ammo = FindObjectOfType<Ammo>();
    } 

    void OnTriggerEnter(Collider other)
    {
        // if(other.gameObject.tag == "Player")
        // {
            print("Ammo picked");
            Destroy(gameObject);
            ammo.AddAmmoPickup(ammoAmount, ammoType);
        // }
    }
}
