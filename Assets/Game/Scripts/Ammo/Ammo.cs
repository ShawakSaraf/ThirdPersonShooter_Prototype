using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] AmmoSlot[] ammoSlots;

    [System.Serializable]
    private class AmmoSlot
    {
        public AmmoType ammoType;
        public int currentAmmo;
    }

    public int GetCurAmmo(AmmoType ammoType)
    {
        return GetAmmoSlot(ammoType).currentAmmo;
    }

    public int ReduceCurAmmo(AmmoType ammoType)
    {
        return GetAmmoSlot(ammoType).currentAmmo--;
    }

    public void ReloadAmmo(AmmoType ammoType, int LastClip)
    {
        GetAmmoSlot(ammoType).currentAmmo += LastClip;
    }

    public void AddAmmoPickup(int ammoAmount, AmmoType ammoType)
    {
        GetAmmoSlot(ammoType).currentAmmo += ammoAmount;
    }

    private AmmoSlot GetAmmoSlot(AmmoType ammoType)
    {
        foreach(AmmoSlot slot in ammoSlots)
        {
            if(slot.ammoType == ammoType)
            {
                return slot;
            }
        }
        return null;
    }
}
