using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour, IDamagable
{
    public float maxHealth = 100f;
    public float health;

    public float maxStamina = 100f;
    public float staminaDepletionRate = 50f;
    public float staminaRegenRate = 25f;
    public float stamina;
    protected bool isTired;

    public float healthRegen = 0;
    public float movementSpeed = 5f;
    public float sprintMultiplier = 2.25f;
    protected float sprintModifier = 2f;
    public float strength;
    //public float staminaSpeedModifier = 1f;

    protected Rigidbody rb;

    public bool isStunned;
    public float stunDurationTimer;

    // Start is called before the first frame update
    protected void Start()
    {
        health = maxHealth;
        stamina = maxStamina;
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        //SetStunFunctions();
    }

    public virtual void Stun(float dur)
    {
        
    }
    
    public virtual void EndStun()
    {

    }

    // Update is called once per frame
    protected void Update()
    {
        if (isStunned == true)
        {
            stunDurationTimer -= Time.deltaTime;

            if (stunDurationTimer <= 0)
            {
                isStunned = false;
                EndStun();
            }
        }
    }

    virtual public void Damage(float dmg, Vector3 force, float stunDur)
    {
        Knockback(force);

        health -= dmg;

        health = Mathf.Clamp(health, 0, maxHealth);

        if (health <= 0)
            Death();

        //Debug.Log("OOF!");

        if (stunDur > 0)
        {
            //Debug.Log("OOF2!");
            isStunned = true;
            Stun(stunDur);
        }

    }

    virtual public void Death()
    {
        Debug.Log("Death!");
        gameObject.SetActive(false);

        //OnStun = null;
        //OnEndStun = null;
    }

    private void Knockback(Vector3 force)
    {
        //Debug.Log("KNOCKED BACK!");
        rb.AddForce(force, ForceMode.Impulse);
    }
}
