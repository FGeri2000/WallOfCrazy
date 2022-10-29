using UnityEngine;
using System.Collections;

public enum PinTypeEnum
{
    Label = 0,
    Text = 1,
    Picture = 2,
    //Line = 3,
    Empty = 4
}
public enum ColorEnum
{
    Red, Green, Blue,
    Cyan, Yellow, Magenta
}

public class PinScript : MonoBehaviour
{
    public PinTypeEnum type;

    public int width, height;
    public float scale = 1;
    public string imagePath;
    public byte[] imageData;
    public Color color;

    public void ResizeImage()
    {
        if (((float)width) / height > 1)
        {
            transform.Find("Picture").localScale = new Vector3(scale * width / height, transform.Find("Picture").localScale.y, scale);
            transform.Find("Picture").localPosition = new Vector3(transform.Find("Picture").localPosition.x, -.3333333f - (scale - 1) / 2.6666666f, transform.Find("Picture").localPosition.z);
        }
        else if (((float)width) / height < 1)
        {
            transform.Find("Picture").localScale = new Vector3(scale, transform.Find("Picture").localScale.y, scale * height / width);
            transform.Find("Picture").localPosition = new Vector3(transform.Find("Picture").localPosition.x, -.3333333f * ((float)height / width) - (scale - 1) / 2.3333333f, transform.Find("Picture").localPosition.z);
        }
        else
        {
            transform.Find("Picture").localScale = new Vector3(scale, transform.Find("Picture").localScale.y, scale);
            transform.Find("Picture").localPosition = new Vector3(transform.Find("Picture").localPosition.x, -.3333333f - (scale - 1) / 2.6666666f, transform.Find("Picture").localPosition.z);
        }
    }
    public void SetColor(Color c)
    {
        color = c;
        transform.Find("Pin").Find("Pin").GetComponent<MeshRenderer>().material.color = c;
        /*switch (c)
        {
            case ColorEnum.Red:
                transform.Find("Pin").Find("Pin").GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Colors/Red");
                break;
            case ColorEnum.Green:
                transform.Find("Pin").Find("Pin").GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Colors/Green");
                break;
            case ColorEnum.Blue:
                transform.Find("Pin").Find("Pin").GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Colors/Blue");
                break;
            case ColorEnum.Cyan:
                transform.Find("Pin").Find("Pin").GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Colors/Cyan");
                break;
            case ColorEnum.Yellow:
                transform.Find("Pin").Find("Pin").GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Colors/Yellow");
                break;
            case ColorEnum.Magenta:
                transform.Find("Pin").Find("Pin").GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Colors/Magenta");
                break;
        }*/
    }
}
