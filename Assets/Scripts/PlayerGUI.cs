using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class PlayerGUI : MonoBehaviour
{
    public Slider healthSlider;
    public Slider staminaSlider;
    public Slider reloadSlider;
    public Text weaponName;
    public Text ammo;
    public Image respawnPanel;
    public Text respawnTextTimer;

    private bool isDead;
    private float respawnTimer = 10;
    private Action OnRespawn;

    public void UpdateEverything(PlayerController player)
    {
        SetHealth(player.health, player.maxHealth);
        SetStamina(player.stamina, player.maxStamina);

        //if Reloading
        if (player.CurrentWeapon().isReloading)
        {
            ammo.gameObject.SetActive(false);
            reloadSlider.gameObject.SetActive(true);

            //Set Slider Amount
            reloadSlider.maxValue = player.CurrentWeapon().reloadSpeed;
            reloadSlider.value = player.CurrentWeapon().reloadSpeed - player.CurrentWeapon().reloadTimer;
        }

        else
        {
            ammo.gameObject.SetActive(true);
            reloadSlider.gameObject.SetActive(false);
        }

        SetAmmo(player.CurrentWeapon().ammoAmount - player.CurrentWeapon().currentAmmoCount, player.CurrentWeapon().ammoAmount);

        //Set Weapon Name
        weaponName.text = player.CurrentWeapon().name;
    }

    public void SetHealth(float val, float max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = val;
    }

    //public void SetWeaponName()
    //{

    //}

    public void SetStamina(float val, float max)
    {
        staminaSlider.maxValue = max;
        staminaSlider.value = val;
    }

    public void SetAmmo(int left, int max)
    {
        //if (left != 0)
            ammo.text = left + "/" + max;
        //else
        //    ammo.text = "Reloading...";
    }

    private void Update()
    {
        if (isDead)
        {
            respawnPanel.gameObject.SetActive(true);

            respawnTimer -= Time.deltaTime;
            respawnTextTimer.text = ((int)respawnTimer).ToString();

            if (respawnTimer <= 0)
            {
                isDead = false;

                if (OnRespawn != null)
                    OnRespawn.Invoke();

                OnRespawn = null;
                respawnPanel.gameObject.SetActive(false);
                respawnTimer = 10f;
            }
        }
    }

    public void StartRespawnTimer(Action action)
    {
        isDead = true;
        OnRespawn = action;
        respawnTextTimer.gameObject.SetActive(true);
        respawnPanel.gameObject.SetActive(true);
    }

    public void FlashRed()
    {
        respawnTextTimer.gameObject.SetActive(false);
        respawnPanel.gameObject.SetActive(true);
        DOVirtual.DelayedCall(0.1f, () => respawnPanel.gameObject.SetActive(false));
    }
}
