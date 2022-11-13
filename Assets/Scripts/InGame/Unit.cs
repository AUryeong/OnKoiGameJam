using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SocialPlatforms.Impl;

public class Unit : MonoBehaviour
{
    protected int _line;
    protected float lineMoveDuration = 0.2f;
    protected bool isAttackable = true;
    protected bool isBouding = true;

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    public int line
    {
        get
        {
            return _line;
        }
        set
        {
            _line = Mathf.Clamp(value, 1, 4);
            transform.DOMoveY(UtilManager.GetLineY(_line), lineMoveDuration);
        }
    }

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected virtual void Update()
    {
    }

    protected virtual void Hit(Player player)
    {
        if (isAttackable)
        {
            player.Hurt();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttackable)
            if (collision != null)
                if (collision.CompareTag("Player"))
                {
                    Hit(collision.GetComponent<Player>());
                    if (!isBouding)
                        transform.DOMoveX(4, 0.1f).SetRelative();
                }
    }
}
