using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHunterCollide : MonoBehaviour
{
    public CapsuleCollider2D hunterCollider;
    public CapsuleCollider2D hunterBlockerCollider;

    void Start()
    {
        Physics2D.IgnoreCollision(hunterCollider, hunterBlockerCollider, true);
    }
}
