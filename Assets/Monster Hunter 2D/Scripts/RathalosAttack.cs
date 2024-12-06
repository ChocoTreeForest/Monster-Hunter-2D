using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
#if UNITY_EDITOR
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
#endif
using static UnityEngine.GraphicsBuffer;

public class RathalosAttack : MonoBehaviour
{
    private Rathalos rathalos;
    private Animator animator;
    private Hunter hunter;

    void Awake()
    {
        rathalos = GetComponentInParent<Rathalos>();
        animator = rathalos.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((rathalos.currentState == Rathalos.State.Chasing || rathalos.currentState == Rathalos.State.Attacking) && collision.GetComponent<Rigidbody2D>() == rathalos.target)
        {
            hunter = collision.GetComponent<Hunter>();

            if (gameObject.CompareTag("BiteRange"))
            {
                rathalos.availableAttacks.Add("Bite");
                rathalos.hunterInBiteRange = true;
            }

            if (gameObject.CompareTag("ClawRange"))
            {
                rathalos.availableAttacks.Add("Claw");
                rathalos.hunterInClawRange = true;
            }

            if (gameObject.CompareTag("TailRange"))
            {
                rathalos.availableAttacks.Add("Tail");
                rathalos.hunterInTailRange = true;
            }

            if (gameObject.CompareTag("BreathRange"))
            {
                rathalos.availableAttacks.Add("Breath");
            }

            if (rathalos.availableAttacks.Count > 0 && !rathalos.isStunned)
            {
                string randomAttack = rathalos.GetRandomAttack();
                float distanceToHunter = Vector2.Distance(transform.position, rathalos.hunterTransform.position);

                if (distanceToHunter < rathalos.minBreathDistance && randomAttack == "Breath") return;
                if ((rathalos.currentDirection == Rathalos.Direction.L || rathalos.currentDirection == Rathalos.Direction.R) && randomAttack == "Claw") return;

                StartCoroutine(Delay());
                StartCoroutine(rathalos.PerformAttack(randomAttack));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((rathalos.currentState == Rathalos.State.Chasing || rathalos.currentState == Rathalos.State.Attacking) && collision.GetComponent<Rigidbody2D>() == rathalos.target)
        {
            if (gameObject.CompareTag("BiteRange"))
            {
                rathalos.availableAttacks.Remove("Bite");
                rathalos.hunterInBiteRange = false;
            }

            if (gameObject.CompareTag("ClawRange"))
            {
                rathalos.availableAttacks.Remove("Claw");
                rathalos.hunterInClawRange = false;
            }

            if (gameObject.CompareTag("TailRange"))
            {
                rathalos.availableAttacks.Remove("Tail");
                rathalos.hunterInTailRange = false;
            }

            if (gameObject.CompareTag("BreathRange"))
            {
                rathalos.availableAttacks.Remove("Breath");
            }
        }
    }

    public void BiteAttack()
    {
        if (rathalos.hunterInBiteRange)
        {
            rathalos.ApplyDamage(hunter);
        }
    }

    public void ClawAttack()
    {
        if (rathalos.hunterInClawRange)
        {
            rathalos.ApplyDamage(hunter);
            
            if(Random.value <= 0.25f)
            {
                hunter.status.ApplyPoison();
            }
        }
    }

    public void TailAttack()
    {
        if (rathalos.hunterInTailRange)
        {
            rathalos.ApplyDamage(hunter);
        }
    }

    private IEnumerator Delay()
    {
        rathalos.canMove = false;
        rathalos.speed = 0;
        rathalos.currentState = Rathalos.State.Attacking;

        yield return new WaitForSeconds(0.5f);
    }
}
