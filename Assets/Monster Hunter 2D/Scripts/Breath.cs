using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breath : MonoBehaviour
{
    public int damage = 35;
    public Transform hunterTransform;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hunter"))
        {
            Hunter hunter = collision.GetComponent<Hunter>();
            hunter.status.TakeDamage(damage, GetAttackDirection());

            if (Random.value <= 0.5f)
            {
                hunter.status.ApplyBurn();
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Object"))
        {
            Destroy(gameObject);
        }
    }

    public HunterAttack.GuardDirection GetAttackDirection()
    {
        Vector2 direction = hunterTransform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle > -45 && angle <= 45) return HunterAttack.GuardDirection.FL;
        else if (angle > 45 && angle <= 135) return HunterAttack.GuardDirection.FR;
        else if (angle > -135 && angle <= -45) return HunterAttack.GuardDirection.BL;
        else return HunterAttack.GuardDirection.BR;
    }
}
