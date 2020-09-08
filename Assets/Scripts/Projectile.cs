using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 1f;
    public float knockbackPower = 10f;
    public float damage = 25f;
    public float stunDuration = 3f;
    public bool piercing;

    private float lifetimeTimer;

    public string[] ignoreTags = { "Projectile", "Weapon", "Ground" };
    

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        lifetimeTimer -= Time.deltaTime;

        if (lifetimeTimer <= 0)
            gameObject.SetActive(false);
    }

    public void Reset()
    {
        lifetimeTimer = lifetime;
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("HIT!");

        if (collision.transform.tag == "Player" || collision.transform.tag == "Enemy")
        {
            Debug.Log("COLLISION ON CHARACTER!");

            //Get Character Rigidbody and apply force

            //SHOULD PULL FROM GAMEMANAGER OR SOMETHING BASED ON ID
            Character character = collision.transform.GetComponent<Character>();

            Vector3 knockbackDirection = transform.forward * knockbackPower;

            character.Damage(damage, knockbackDirection, stunDuration);

            //if (!piercing)
            //    gameObject.SetActive(false);
        }

        //else if (collision.transform.tag != "Projectile" || collision.transform.tag != "Weapon")
        for (int i = 0; i < ignoreTags.Length; i++)
        {
            if (ignoreTags[i] == collision.tag)
                return;
        }

        if (!piercing)
            gameObject.SetActive(false);
    }
}
