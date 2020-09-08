using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(CharacterController))]
public class PlayerController : Character
{
    [Header("Player Settings")]
    //List of Weapons
    public int playerID = 0;
    public string horizontalInput;
    public string verticalInput;
    public string attackInput;
    public string sprintButton;
    public string switchWeaponInput;
    public string reloadInput;
    public string rotateXInput, rotateZInput;

    [Header("General Settings")]
    public float rotationSpeed = 500f;

    public List<Weapon> weaponsPrefab = new List<Weapon>();  //Currently Equipped Weapons

    private List<Weapon> weapons = new List<Weapon>();

    public Transform weaponHandPos;

    [Header("Body Settings")]
    public GameObject body;
    public float bodyBobYOffset = 1f;//, bodyBobYOffsetNeg = -1f;
    public float bodyBobSpeed = 3f;
    private float bobValue;

    private int currentWeapon = 0;

    public float invincibilityTimer = 0.15f;
    private float invincibilityCurrentTimer;

    private Vector3 middleOfScreen;

    private float constantYPosition;

    //PlayerClass

    private CharacterController cc;
    private NavMeshAgent nav;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        cc = GetComponent<CharacterController>();

        //Prepare Weapons
        InitWeapons();

        nav = GetComponent<NavMeshAgent>();

        middleOfScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0f);

        constantYPosition = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (!isStunned)
        {
            RotatePlayer();
            RotatePlayerMouse();

            MovePlayer();
            Sprint();
            UseWeapon();
            ReloadWeapon();
            SwitchWeapons();
            BodyBob();
        }

        StaminaRegen();
        HealthRegen();
        Invincibility();
        PassiveReloading();

        if (transform.position.y != constantYPosition)
            transform.position = new Vector3(transform.position.x, constantYPosition, transform.position.z);
    }//

    private void BodyBob()
    {
        if (!isMovementControllerUsed()) return;
        //As the player moves, bob head up and down
        bobValue += Time.deltaTime * bodyBobSpeed * sprintModifier;
        float yPos = Mathf.Sin(bobValue) * bodyBobYOffset;

        Vector3 lerpToVector = Vector3.Lerp(body.transform.position, new Vector3(body.transform.position.x, body.transform.position.y + yPos, body.transform.position.z), Time.deltaTime);
        body.transform.position = lerpToVector;


    }

    private void Invincibility()
    {
        if (invincibilityCurrentTimer >= 0)
            invincibilityCurrentTimer -= Time.deltaTime;

        if (invincibilityTimer <= 0)
        {
            rb.isKinematic = false;
            isStunned = false;
        }
    }

    private void RotatePlayerMouse()
    {
        if (!isMouseRotationUsed()) return;

        Vector3 camVec = Input.mousePosition - middleOfScreen;
        Vector3 flipped = new Vector3(camVec.x, 0f, camVec.y);
        transform.LookAt(flipped);
    }

    private void HealthRegen()
    {
        if (health < maxHealth)
        {
            health += healthRegen * Time.deltaTime;
        }
    }

    private void PassiveReloading()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].UpdateReloading();
        }
    }

    private void ReloadWeapon()
    {
        if (Input.GetButtonDown(reloadInput))
        {
            weapons[currentWeapon].Reload();
        }
    }

    private void StaminaRegen()
    {
        if (stamina < maxStamina)
        {
            stamina += staminaRegenRate * Time.deltaTime;
        }

        else
            isTired = false;
    }

    private void Sprint()
    {
        if (isTired) return;

        //if (Input.GetAxisRaw(attackInput) == -1)
        if (Input.GetButton(sprintButton))
        {
            sprintModifier = sprintMultiplier;
            stamina -= staminaDepletionRate * Time.deltaTime;

            if (stamina <= 0)
            {
                isTired = true;
                sprintModifier = 1;
            }
        }

        else
            sprintModifier = 1;
    }

    public Weapon CurrentWeapon()
    {
        return weapons[currentWeapon];
    }

    public override void Damage(float dmg, Vector3 force, float stunDur)
    {

        //if (invincibilityCurrentTimer >= 0)
        //    return;

        //rb.isKinematic = true;
        //isStunned = true;
        
        base.Damage(dmg, force, stunDur);

        if (dmg > 0 && health > 0)
            GameManager.Instance.guiManager.TakeDamage(playerID);

        //int playerID = GameManager.Instance.GetPlayer(this);

        //invincibilityCurrentTimer = invincibilityTimer;
    }

    private void InitWeapons()
    {
        Debug.Log("Weapons Preparing");

        for (int i = 0; i < weaponsPrefab.Count; i++)
        {
            //Spawn Each weapon in hand Weapon Position
            Weapon weapon = Instantiate(weaponsPrefab[i], weaponHandPos.position, weaponHandPos.rotation);

            weapon.transform.parent = weaponHandPos.transform;
            weapon.gameObject.SetActive(false);

            weapons.Add(weapon);            
        }

        weapons[0].gameObject.SetActive(true); //Start with first Weapon

        //Clear Memory for weaponsPrefab
        weaponsPrefab.Clear();
    }

    private void UseWeapon()
    {
        if (weapons[currentWeapon].canHoldTrigger)
        {
            if (Input.GetAxisRaw(attackInput) > 0)
            {
                weapons[currentWeapon].Use(Input.GetAxisRaw(attackInput));
            }
        }

        else
        {
            //if (Input.GetAxisRaw(attackInput) > 0)
            //{
            //NEED TO PASS ATTACK INPUT
                weapons[currentWeapon].Use(Input.GetAxisRaw(attackInput));
            //}
        }
    }

    private void SwitchWeapons()
    {
        if (Input.GetButtonDown(switchWeaponInput))
        {
            weapons[currentWeapon].gameObject.SetActive(false);
            currentWeapon++;

            if (currentWeapon >= weapons.Count)
                currentWeapon = 0;

            weapons[currentWeapon].gameObject.SetActive(true);
        }
    }

    private void RotatePlayer()
    {
        if (isControllerRotationUsed() == false) return;

        float inputZ = Input.GetAxis(rotateZInput);
        float inputX = Input.GetAxis(rotateXInput);

        Vector3 lookDirection = new Vector3(inputX, 0, inputZ);
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

        float step = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(lookRotation, transform.rotation, step);
    }

    private void MovePlayer()
    {
        //if (!isControllerRotationUsed()) return;



        Vector3 movement = new Vector3(Input.GetAxis(verticalInput), 0f, -Input.GetAxis(horizontalInput)).normalized;
        Vector3 moveModifiers = movement * movementSpeed * sprintModifier * Time.deltaTime;

        //if (moveModifiers != Vector3.zero)
        //{
        //nav.speed = 5;
        //nav.Move(movement);
        //nav.speed = movementSpeed * sprintModifier;
        //nav.SetDestination(transform.position + (movement * 2f));
        //rb.velocity = moveModifiers;
        cc.Move(moveModifiers);
        //Debug.Log("Player Moving!");
        //}
    }

    public override void Death()
    {
        gameObject.SetActive(false);
        GameManager.Instance.SetDeath(playerID, Revive);
        GameManager.Instance.CheckIfAllDead();
    }

    public void Revive()
    {
        health = maxHealth;
        gameObject.SetActive(true);
        transform.position = GameManager.Instance.GetPlayerSpawnPoint(playerID);
    }

    private bool isControllerRotationUsed()
    {
        return (new Vector3(Input.GetAxisRaw(rotateXInput), 0, Input.GetAxisRaw(rotateZInput)).magnitude != 0) ? true : false;
    }

    private bool isMouseRotationUsed()
    {
        return (new Vector3(-Input.GetAxisRaw("Mouse X"), 0, -Input.GetAxisRaw("Mouse Y")).magnitude != 0) ? true : false;
    }

    private bool isMovementControllerUsed()
    {
        return (new Vector3(Input.GetAxisRaw(verticalInput), 0, Input.GetAxisRaw(horizontalInput)).magnitude != 0) ? true : false;
    }
}
