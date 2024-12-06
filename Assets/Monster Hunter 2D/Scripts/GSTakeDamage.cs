using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSTakeDamage : MonoBehaviour
{
    private HunterAttack hunterAttack;

    private void Awake()
    {
        hunterAttack = GetComponentInParent<HunterAttack>();        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.CompareTag("GreatSword"))
        {
            if (collision.CompareTag("Rathalos"))
            {
                Rathalos rathalos = collision.GetComponentInParent<Rathalos>();
                if (rathalos != null && hunterAttack != null)
                {
                    hunterAttack.ApplyDamage(rathalos);
                }
            }
        }       
    }
}
