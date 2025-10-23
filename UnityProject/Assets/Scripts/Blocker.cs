using System.Collections;
using UnityEngine;

public class Blocker : MonoBehaviour
{
    private SpriteRenderer sp;
    private Color greyColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
    }

    public void Hide()
    {
        Color transparent = greyColor;
        transparent.a = 0;
        sp.color = transparent;
    }
    public void Show()
    {
        sp.color = greyColor;
    }
}