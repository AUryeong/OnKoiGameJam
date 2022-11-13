using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_BigGun : Enemy
{
    protected float cooltime = 3;
    protected float duration;

    protected bool isMoving = false;
    protected override void OnEnable()
    {
        base.OnEnable();
        duration = 0;
        isMoving = true;
        isBouding = true;
    }
    protected override void Update()
    {
        base.Update();
        if (!isDying)
        {
            duration += Time.deltaTime;
            if (duration >= cooltime)
            {
                duration -= cooltime;
                StartCoroutine(ShootGun());
            }
        }
    }

    protected override void Remove()
    {
        base.Remove();
        Hit(Player.Instance);
    }

    protected override void Move()
    {
        if (isMoving)
            base.Move();
    }

    IEnumerator ShootGun()
    {
        animator.Play("Shoot", -1, 0);
        isMoving = false;
        yield return new WaitForSeconds(1f);
        if (!isDying)
        {
            GameObject obj = PoolManager.Instance.Init(GameManager.Instance.bigBubble.gameObject, 10);
            obj.transform.position = transform.position + new Vector3(-2.5f, 0, 0);
            obj.GetComponent<Unit>().line = line;
            isMoving = true;
            animator.Play("Idle", -1, 0);
        }
    }
}
