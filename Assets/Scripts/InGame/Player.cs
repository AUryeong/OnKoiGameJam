using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public static Player _instance;
    public static Player Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<Player>();
            return _instance;
        }
    }
    [SerializeField] SpriteRenderer gunArm;
    bool inv;

    protected override void Awake()
    {
        base.Awake();
        _instance = this;
        line = 1;
    }
    protected override void Update()
    {
        base.Update();
        LookArmMouse();
        CheckMove();
    }

    protected void CheckMove()
    {
        if (Input.GetKeyDown(KeyCode.S))
            line--;
        else if (Input.GetKeyDown(KeyCode.W))
            line++;
    }

    public void Hurt()
    {
        if (!inv)
        {
            GameManager.Instance.hp--;
            GameManager.Instance.cameraFilter_EarthQuakes.Add(new CameraFilter_EarthQuake()
            {
                X = 0.2f,
                Y = 0.4f,
                Time = 0.3f
            });
            gameObject.layer = LayerMask.NameToLayer("PlayerInv");
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            inv = true;
            StartCoroutine(WaitPlayerInv());
        }
    }

    IEnumerator WaitPlayerInv()
    {
        yield return new WaitForSeconds(1);
        gameObject.layer = LayerMask.NameToLayer("Player");
        spriteRenderer.color = Color.white;
        inv = false;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
    }

    protected void LookArmMouse()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        gunArm.transform.rotation = Quaternion.AngleAxis((Mathf.Atan2(mouse.y - gunArm.transform.position.y, mouse.x - gunArm.transform.position.x) * Mathf.Rad2Deg), Vector3.forward);
    }
}
