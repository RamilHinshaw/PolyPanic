using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : Character
{
    public GameObject target;
    private NavMeshAgent nav;
    public Projectile projectile;
    public float attackRange;
    public float hitRate = 0.45f;
    private float hitRateTimer = 0;

    public float attackDamage = 10f;
    public float knockbackPower = 3f;

    private void Start()
    {
        base.Start();

        if (nav == null)
            nav = GetComponent<NavMeshAgent>();

        //nav.speed = movementSpeed;
        nav.stoppingDistance = 0;

        //Go to closest Player
        target = GetClosestPlayer();
    }

    private void Update()
    {
        base.Update();

        if (target.activeSelf == false)
        {
            target = GetClosestPlayer();
        }

        //Debug.Log(Vector3.Distance(transform.position, target.transform.position));

        //If enemy is close enough attack
        if (Vector3.Distance(transform.position, target.transform.position) <= attackRange && hitRateTimer <= 0)
        {
            Debug.Log("ATTACK PLAYER!");
            nav.isStopped = true;
            AttackTarget();
        }
        //If enemy is far then go to player
        else
        {
            //if (isStunned)
                nav.isStopped = false;

            GoToPlayer();
        }

        if (hitRateTimer > 0)
            hitRateTimer -= Time.deltaTime;
        else if (!isStunned)
            nav.isStopped = false;


    }

    private void AttackTarget()
    {
        if (hitRateTimer > 0) return;

        //Attack up close
        if (projectile == null)
        {
            if (target.tag == "Player")
            {
                var playerTarget = GameManager.Instance.GetPlayer(target.GetInstanceID());
                playerTarget.Damage(attackDamage, transform.forward * knockbackPower, 0);

                nav.isStopped = true;
            }

            //ADD STRUCTURE HERE
        }

        //Attack Far Away
        else
        {

        }

        hitRateTimer = hitRate;
    }

    private void GoToPlayer()
    {
        nav.destination = target.transform.position;
    }

    public override void Stun(float dur)
    {
        //base.Stun(dur);
        stunDurationTimer = dur;
        nav.isStopped = true;
    }

    public override void EndStun()
    {
        nav.isStopped = false;
    }

    public void SetSpeed(float spd)
    {
        if (nav == null)
            nav = GetComponent<NavMeshAgent>();

        nav.speed = spd;
    }

    private GameObject GetClosestPlayer()
    {
        //GameObject target = GameObject.FindGameObjectWithTag("Player");
        target = GameManager.Instance.players[ UnityEngine.Random.Range(0, GameSettings.playersPlaying) ].gameObject;
        return target;
    }

    public override void Death()
    {
        //base.Death();

        Destroy(gameObject);
    }

    //public override void Damage(float dmg, Vector3 force, float stunDur)
    //{
    //    base.Damage(dmg, force, stunDur);



    //}
}
