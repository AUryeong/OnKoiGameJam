using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : Unit
{
    public float speed;

    protected override void Update()
    {
        base.Update();
        transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
        if (transform.position.x < -10)
        {
            gameObject.SetActive(false);
        }
    }
    protected override void Hit(Player player)
    {
        base.Hit(player);
        gameObject.SetActive(false);
    }
}
