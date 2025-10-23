using UnityEngine;

public class Mine : MonoBehaviour
{

    public bool isRed;
    public bool isChecked;
    private SpriteRenderer sp;
    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
    }


    public void Redraw()
    {
        if (isRed)
            sp.color = Color.red;
        else
            sp.color = Color.blue;
    }
}
