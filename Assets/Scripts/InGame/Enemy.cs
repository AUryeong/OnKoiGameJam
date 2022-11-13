using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Enemy : Unit
{
    protected bool isDying = false;

    protected float dieMoveX = 3;
    protected float dieDuration = 0.5f;

    public float speed;

    public void Hit()
    {
        if (!isDying)
        {
            isDying = true;
            transform.DOMoveX(dieMoveX, dieDuration).SetRelative();
            spriteRenderer.DOFade(0, dieDuration).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
            animator.Play("Die", -1, 0);
            GameManager.Instance.AddScore(Random.Range(3f, 3.5f));
        }
    }
    protected virtual void OnEnable()
    {
        isDying = false;
        spriteRenderer.color = Color.white;
        animator.Play("Idle", -1, 0);
    }

    protected override void Update()
    {
        base.Update();
        Move();
    }

    protected virtual void Move()
    {
        if (!isDying)
        {
            transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
            if (transform.position.x < -10)
            {
                Remove();
            }
        }
    }

    protected virtual void Remove()
    {
        gameObject.SetActive(false);
    }

    protected override void Hit(Player player)
    {
        if (!isDying)
            base.Hit(player);
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDying)
            base.OnTriggerEnter2D(collision);
    }
}
