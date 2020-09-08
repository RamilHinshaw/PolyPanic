using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon : MonoBehaviour
{   
    //Will be attached to this
    public string name;
    public Projectile projectile; //Used to keep track of prefab
    public bool canHoldTrigger = false;
    private bool isTriggerReset = true; //Used force player to press repeatly
    public GameObject muzzleFlash;

    public GameObject shootSpawn;
    [Range(0,1f)]
    public float fireVolume = 0.303f;
    public AudioClip sfx_fire;
    public AudioClip sfx_reload;

    public int ammoAmount = 10;
    public int currentAmmoCount;

    public int projectilePoolSize = 5;
    public float fireRate = 0.2f;
    public float reloadSpeed = 3f;
    public float reloadTimer;
    public bool isReloading;


    private List<Projectile> projectiles = new List<Projectile>();  //ObjectPool this
    public int currentProjectile = 0;

    private float fireRateTimer;

    private void Start()
    {
        PoolProjectiles();

    }

    private void Update()
    {
        if (fireRateTimer > 0)
            fireRateTimer -= Time.deltaTime;
    }

    //Used outside because if not enabled doesn't update
    public void UpdateReloading()
    {
        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;

            //RELOAD END
            if (reloadTimer <= 0)
            {
                isReloading = false;
                currentAmmoCount = 0;
                GameManager.Instance.PlayAudio(sfx_reload);
            }
        }
    }

    public void PoolProjectiles()
    {
        for (int i = 0; i < projectilePoolSize; i++)
        {
            //Instantiate projectiles
            Projectile prefab = Instantiate(projectile, shootSpawn.transform.position, shootSpawn.transform.rotation);
            prefab.gameObject.SetActive(false);

            projectiles.Add(prefab); //REALLY SMALL CODING HERE CAUSED ME HEADACHE :(
        }
    }

    virtual public void Use(float inputValue)
    {
        //Debug.Log(inputValue);
        if (gameObject.activeSelf == false) return;
        if (inputValue < 0) return;


        if (isReloading) return;
        if (isTriggerReset == false)
        {
            //Debug.Log(inputValue);
            if (inputValue != 0)
                return;

            else
                isTriggerReset = true;
        }

        if (!canHoldTrigger && inputValue == 0) return; //Don't fire if not hold trigger

        if (fireRateTimer > 0) return;

        //Debug.Log("FIRING!");
        //Play SFX

        //var origVol = GameManager.Instance.audioSource.volume;
        //GameManager.Instance.audioSource.volume = fireVolume;
        GameManager.Instance.PlayAudio(sfx_fire, fireVolume);

        if (muzzleFlash != null)
            muzzleFlash.SetActive(true);
        //GameManager.Instance.audioSource.volume = origVol;

        //Move Projectile to ShootSpawn
        projectiles[currentProjectile].transform.position = shootSpawn.transform.position;
        projectiles[currentProjectile].transform.rotation = shootSpawn.transform.rotation;

        //Enable projectile
        projectiles[currentProjectile].Reset();
        projectiles[currentProjectile].gameObject.SetActive(true);

        fireRateTimer = fireRate;

        if (canHoldTrigger == false && inputValue > 0.15f)
        {
            isTriggerReset = false;
        }

        NextProjectile();
    }



    private void NextProjectile()
    {
        currentProjectile++;
        currentAmmoCount++;

        if (currentProjectile >= projectilePoolSize)
        {

            currentProjectile = 0;
        }

        if (currentAmmoCount >= ammoAmount)
        {

            Reload();
        }
    }

    public void Reload()
    {
        isReloading = true;
        currentAmmoCount = ammoAmount;
        reloadTimer = reloadSpeed;
    }

}
