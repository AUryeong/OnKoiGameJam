using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] Image mouseImage;
    [SerializeField] SpriteRenderer heartBubble;
    [SerializeField] Image heart;
    [SerializeField] List<Enemy> enemies;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    [SerializeField] Image overPowerImage;
    [SerializeField] Image ultimateImage;
    public SpriteRenderer bubble;
    public SpriteRenderer bigBubble;
    public List<CameraFilter_EarthQuake> cameraFilter_EarthQuakes = new List<CameraFilter_EarthQuake>();

    [SerializeField] List<GameObject> buildings;
    List<GameObject> buildingsList = new List<GameObject>();

    public float score = 0;
    float enemyCooltime = 0.5f;
    float enemyDuration = 0;
    float buildCooltime = 1f;
    float buildDuration = 0;

    float patternCooltime = 20;
    float patternDuration = 0;
    protected int _hp = 4;
    public int hp
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = Mathf.Clamp(value, 0, 4);
            if (_hp <= 0)
            {
                if (PlayerPrefs.GetInt("Score") < score)
                {
                    PlayerPrefs.SetInt("Score", (int)score);
                }
                SceneManager.LoadScene("Title");
                return;
            }
            heart.DOFillAmount(_hp / 4f, 0.2f);
        }
    }

    public float power;
    void Start()
    {
        Cursor.visible = false;
        Player.Instance.line = 1;
        score = 0;
    }
    void CameraControl()
    {
        Vector3 shake = Vector3.zero;
        if (cameraFilter_EarthQuakes.Count > 0)
        {
            List<CameraFilter_EarthQuake> filters = new List<CameraFilter_EarthQuake>();
            foreach (CameraFilter_EarthQuake cameraFilter in cameraFilter_EarthQuakes)
            {
                cameraFilter.Time -= Time.deltaTime;
                if (cameraFilter.Time <= 0)
                {
                    filters.Add(cameraFilter);
                }
                else
                {
                    Vector2 rand = UnityEngine.Random.insideUnitCircle;
                    shake += new Vector3(rand.x * cameraFilter.X, rand.y * cameraFilter.Y, 0);
                }
            }
            foreach (CameraFilter_EarthQuake cameraFilter in filters)
            {
                cameraFilter_EarthQuakes.Remove(cameraFilter);
            }
        }
        Camera.main.transform.localPosition = shake + new Vector3(0, 0, -10);
    }
    void Update()
    {
        mouseImage.transform.position = Input.mousePosition;
        CheckClick();
        CameraControl();
        EnemyCreate();
        CreateBuilding();
        MoveBuilding();
        CheckPattern();
        score += Time.deltaTime;
        textMeshProUGUI.text = "Á¡¼ö : " + ((int)score).ToString();
    }

    void CheckPattern()
    {
        patternDuration += Time.deltaTime;
        if (patternDuration >= patternCooltime)
        {
            patternDuration -= patternCooltime;
            int pattern = Random.Range(1, 4);
            switch (pattern)
            {
                case 1:
                    int line = Random.Range(1, 5);
                    for (int i = 1; i <= 4; i++)
                    {
                        if (i != line)
                        {
                            GameObject obj = PoolManager.Instance.Init(enemies[2].gameObject);
                            Enemy enemy = obj.GetComponent<Enemy>();
                            obj.transform.position = new Vector3(9, UtilManager.GetLineY(i), 0);
                            enemy.line = i;
                        }
                    }
                    break;
                case 2:
                    for (int i = 1; i <= 4; i++)
                    {
                        GameObject obj = PoolManager.Instance.Init(enemies[1].gameObject);
                        Enemy enemy = obj.GetComponent<Enemy>();
                        obj.transform.position = new Vector3(9, UtilManager.GetLineY(i), 0);
                        enemy.line = i;
                    }
                    break;
                case 3:
                    int line2 = Player.Instance.line;
                    for (int i = 1; i <= 4; i++)
                    {
                        if (i != line2)
                        {
                            GameObject obj = PoolManager.Instance.Init(enemies[3].gameObject);
                            Enemy enemy = obj.GetComponent<Enemy>();
                            obj.transform.position = new Vector3(-4.42f, -6, 0);
                            enemy.line = i;
                        }
                    }
                    break;
            }
        }
    }

    void MoveBuilding()
    {
        foreach (var obj in buildingsList)
        {
            obj.transform.position += new Vector3(-12 * Time.deltaTime, 0, 0);
        }
    }

    void CreateBuilding()
    {
        buildDuration += Time.deltaTime;
        if (buildDuration >= buildCooltime)
        {
            buildDuration -= buildCooltime * Random.Range(0.5f, 1f);
            GameObject obj = PoolManager.Instance.Init(buildings[Random.Range(0, buildings.Count)].gameObject, 20);
            obj.transform.position = new Vector3(12, obj.transform.position.y, 0);
            if (!buildingsList.Contains(obj))
                buildingsList.Add(obj);
        }
    }

    void EnemyCreate()
    {
        enemyDuration += Time.deltaTime;
        if (enemyDuration >= enemyCooltime)
        {
            enemyDuration -= enemyCooltime;
            GameObject obj = PoolManager.Instance.Init(enemies[Random.Range(0, enemies.Count)].gameObject);
            int line = Random.Range(1, 5);
            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy is Enemy_Trap)
                obj.transform.position = new Vector3(-4.42f, -6, 0);
            else
                obj.transform.position = new Vector3(9, UtilManager.GetLineY(line), 0);
            enemy.line = line;
        }
    }

    void CheckClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, LayerMask.GetMask("Enemy"));
            if (rayHit.collider != null)
            {
                rayHit.collider.GetComponent<Enemy>().Hit();
            }
            GameObject obj = PoolManager.Instance.Init(heartBubble.gameObject, 3);
            obj.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 10));
        }
    }

    public void AddScore(float addScore)
    {
        score += addScore;
        if (power < 100)
        {
            power = Mathf.Min(power + addScore / 3f, 100);
            overPowerImage.fillAmount = power / 100;
            if (power >= 100)
            {
                power = 0;
                overPowerImage.fillAmount = 0;
                Time.timeScale = 0;
                ultimateImage.gameObject.SetActive(true);
                ultimateImage.rectTransform.anchoredPosition = new Vector2(215, 108);
                ultimateImage.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
                ultimateImage.rectTransform.DOAnchorPos(new Vector2(165, -108), 0.5f).SetUpdate(true).OnComplete(() =>
                {
                    Time.timeScale = 1;
                    foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                    {
                        enemy.Hit();
                    }
                    ultimateImage.gameObject.SetActive(false);
                    cameraFilter_EarthQuakes.Add(new CameraFilter_EarthQuake()
                    {
                        Time = 0.2f,
                        X = 0.6f,
                        Y = 0.3f
                    });
                    score += 100;
                    hp++;
                });
            }
        }
    }
}

public static class UtilManager
{
    public static int GetLineY(int line)
    {
        return -5 + Mathf.Clamp(line, 1, 4) * 2;
    }
}
public class CameraFilter_EarthQuake
{
    public float X;
    public float Y;
    public float Time;

}