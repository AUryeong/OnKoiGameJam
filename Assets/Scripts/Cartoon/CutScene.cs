using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum EffectType
{
    Up_Scale,
    One_Scale,
    One_Scale_Bounding,
    Fade_In,
    Rotate,
    Wait,
    Move_Left,
    Move_Down,
    Shake_Pos
}
[System.Serializable]
public class CartoonEffect : ISerializationCallbackReceiver
{
    [HideInInspector]
    public string name;
    public EffectType type;
    [Range(0f, 3f)]
    public float power;
    [Range(0f, 5f)]
    public float duration;
    public void OnAfterDeserialize()
    {
        name = type.ToString();
    }

    public void OnBeforeSerialize()
    {
    }

}
[System.Serializable]
public class Cartoon
{
    [HideInInspector]
    public Image image;
    [HideInInspector]
    public TextMeshProUGUI text;
    public List<CartoonEffect> effects = new List<CartoonEffect>();
}
public class CutScene : MonoBehaviour
{
    RectTransform _rectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }
    public List<Cartoon> cartoons = new List<Cartoon>();
}