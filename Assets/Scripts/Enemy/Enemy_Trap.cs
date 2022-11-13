using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Trap : Enemy
{
    protected bool isMoving = false;
    protected override void OnEnable()
    {
        base.OnEnable();
        isMoving = true;
        isAttackable = false;
    }
    protected override void Update()
    {
        if (isMoving)
            if (transform.position.x <= -6.42f)
            {
                isAttackable = true;
                isMoving = false;
            }
        base.Update();
    }

    protected override void Move()
    {
        if (isMoving)
            base.Move();
    }
    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (!isDying)
            if (isAttackable)
                if (collision != null)
                    if (collision.CompareTag("Player"))
                        Hit(collision.GetComponent<Player>());
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
    }
}
