using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Xml.Linq;
using System;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class CartoonManager : Singleton<CartoonManager>
{
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected RectTransform cutSceneParent;

    protected List<CutScene> cutScenes = new List<CutScene>();
    protected CutScene cutScene = null;
    protected int cutSceneIdx = -1;

    protected Cartoon cartoon = null;
    protected int cartoonIdx = -1;

    protected Action endEvent = null;

    protected float waitTime = 0;

    public override void OnReset()
    {
        base.OnReset();
        Cursor.visible = false;

        foreach (RectTransform rectTransform in cutSceneParent)
            cutScenes.Add(rectTransform.GetComponent<CutScene>());

        foreach (var cutScene in cutScenes)
            for (int i = 0; i < cutScene.cartoons.Count; i++)
            {
                Transform rectTransform = cutScene.rectTransform.GetChild(i);
                Image image = rectTransform.GetComponent<Image>();
                if (image != null)
                {
                    image.gameObject.SetActive(false);
                    cutScene.cartoons[i].image = image;
                }
                else
                    cutScene.cartoons[i].text = rectTransform.GetComponent<TextMeshProUGUI>();
            }

        CartoonPlay(0, () => CartoonPlay(1, () => CartoonPlay(2, () => CartoonPlay(3, () => SceneManager.LoadScene("Title")))));
    }

    protected void Update()
    {
        if (cutScene == null)
            return;
        if (cartoon == null)
            NextCartoon();
        WaitTime();
    }

    protected void WaitTime()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0 || Input.GetMouseButtonDown(0))
        {
            if (cartoon.image != null)
            {
                cartoon.image.DOComplete(true);
                cartoon.image.rectTransform.DOComplete(true);
            }
            else
            {
                cartoon.text.DOComplete(true);
                cartoon.text.rectTransform.DOComplete(true);
            }
            NextCartoon();
        }
    }

    protected void NextCartoon()
    {
        cartoonIdx++;
        if (cartoonIdx >= cutScene.cartoons.Count)
        {
            cutScene = null;
            cutSceneIdx = -1;
            cartoonIdx = -1;
            foreach (var cut in cutScenes)
                cut.gameObject.SetActive(false);

            if (endEvent != null)
                endEvent.Invoke();
            return;
        }
        cartoon = cutScene.cartoons[cartoonIdx];
        if (cartoon.image != null)
        {
            cartoon.image.gameObject.SetActive(true);
        }
        else
        {
            cartoon.text.gameObject.SetActive(true);
        }

        float topDuration = 0;
        float addDuration = 0;

        RectTransform rect;
        if (cartoon.image == null)
            rect = cartoon.text.rectTransform;
        else
            rect = cartoon.image.rectTransform;

        foreach (CartoonEffect effect in cartoon.effects)
        {
            switch (effect.type)
            {
                case EffectType.Up_Scale:
                    rect.localScale = Vector2.zero;
                    rect.DOScale(effect.power, effect.duration);
                    break;
                case EffectType.One_Scale:
                    rect.localScale = Vector2.one * effect.power;
                    rect.DOScale(1, effect.duration);
                    break;
                case EffectType.One_Scale_Bounding:
                    rect.localScale = Vector2.one * effect.power;
                    rect.DOScale(1, effect.duration).SetEase(Ease.OutBack);
                    break;
                case EffectType.Fade_In:
                    if (cartoon.image == null)
                    {
                        cartoon.text.color = new Color(cartoon.text.color.r, cartoon.text.color.g, cartoon.text.color.b, effect.power);
                        cartoon.text.DOFade(1, effect.duration);
                    }
                    else
                    {
                        cartoon.image.color = new Color(cartoon.image.color.r, cartoon.image.color.g, cartoon.image.color.b, effect.power);
                        cartoon.image.DOFade(1, effect.duration);
                    }
                    break;
                case EffectType.Rotate:
                    rect.rotation = Quaternion.identity;
                    rect.DORotate(new Vector3(0, 0, effect.power * 360), effect.duration, RotateMode.FastBeyond360).SetRelative(true);
                    break;
                case EffectType.Move_Left:
                    Vector2 origin = rect.anchoredPosition;
                    rect.anchoredPosition += new Vector2(rect.sizeDelta.x * effect.power, 0);
                    rect.DOAnchorPos(origin, effect.duration);
                    break;
                case EffectType.Move_Down:
                    Vector2 originPos = rect.anchoredPosition;
                    rect.anchoredPosition += new Vector2(0, -rect.sizeDelta.x * effect.power);
                    rect.DOAnchorPos(originPos, effect.duration);
                    break;
                case EffectType.Shake_Pos:
                    rect.DOShakePosition(effect.duration, effect.power);
                    break;
                case EffectType.Wait:
                    addDuration += effect.duration;
                    break;
            }
            if (effect.type != EffectType.Wait && topDuration < effect.duration)
                topDuration = effect.duration;
        }
        waitTime = topDuration + addDuration;
    }


    public void CartoonPlay(int cutSceneIdx, Action endEvent)
    {
        this.cutSceneIdx = cutSceneIdx;
        this.endEvent = endEvent;
        cutScene = cutScenes[cutSceneIdx];
        foreach (var cut in cutScenes)
            if (cut == cutScene)
                cut.gameObject.SetActive(true);
            else
                cut.gameObject.SetActive(false);

        foreach (var cartoon in cutScene.cartoons)
            if (cartoon.image == null)
                cartoon.text.gameObject.SetActive(false);
            else
                cartoon.image.gameObject.SetActive(false);

        cartoonIdx = -1;
        NextCartoon();
    }

    protected void OnDestroy()
    {
        Destroy(canvas);
    }
}
