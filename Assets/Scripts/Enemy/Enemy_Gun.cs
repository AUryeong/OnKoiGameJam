using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Gun : Enemy
{
    protected float cooltime = 1.5f;
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
        yield return new WaitForSeconds(0.3f);
        if (!isDying)
        {
            GameObject obj = PoolManager.Instance.Init(GameManager.Instance.bubble.gameObject, 10);
            obj.transform.position = transform.position + new Vector3(-2.5f, 0, 0);
            transform.DOMoveX(0.5f, 0.7f).SetRelative().SetEase(Ease.OutBack);
            obj.GetComponent<Unit>().line = line;
            isMoving = true;
            animator.Play("Idle", -1, 0);
        }
    }
}
