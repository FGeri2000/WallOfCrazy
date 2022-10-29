using UnityEngine;
using System.Collections;

public class LineScript : MonoBehaviour
{
    public PinScript pin1, pin2;
    public bool valid = false;
    public Color color;
    public int width = 2;

    public void SetColor(Color c)
    {
        GetComponent<LineRenderer>().material.color = c;
        color = c;
        /*switch (c)
        {
            case ColorEnum.Red:
                GetComponent<LineRenderer>().material = Resources.Load<Material>("Materials/Colors/Red");
                break;
            case ColorEnum.Green:
                GetComponent<LineRenderer>().material = Resources.Load<Material>("Materials/Colors/Green");
                break;
            case ColorEnum.Blue:
                GetComponent<LineRenderer>().material = Resources.Load<Material>("Materials/Colors/Blue");
                break;
            case ColorEnum.Cyan:
                GetComponent<LineRenderer>().material = Resources.Load<Material>("Materials/Colors/Cyan");
                break;
            case ColorEnum.Yellow:
                GetComponent<LineRenderer>().material = Resources.Load<Material>("Materials/Colors/Yellow");
                break;
            case ColorEnum.Magenta:
                GetComponent<LineRenderer>().material = Resources.Load<Material>("Materials/Colors/Magenta");
                break;
        }*/
    }
    public void SetWidth(int w)
    {
        GetComponent<LineRenderer>().startWidth = w / 100f;
        width = w;
    }

    private void Update()
    {
        if (valid)
        {
            GetComponent<LineRenderer>().SetPosition(0, pin1 ? pin1.transform.Find("LineTarget").position : Vector3.zero);
            GetComponent<LineRenderer>().SetPosition(1, pin2 ? pin2.transform.Find("LineTarget").position : Vector3.zero);
            //GetComponent<LineRenderer>().startWidth = width / 100f;
            if (!pin1 || !pin2)
            {
                Destroy(gameObject);
            }
        }
    }
}
