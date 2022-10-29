using UnityEngine;

public static class BoardSizeController
{
    public static void ResizeSetCallback()
    {
        Test(new Vector2(int.Parse(GameObject.Find("WidthInput").GetComponent<UnityEngine.UI.InputField>().text) / 10f, int.Parse(GameObject.Find("HeightInput").GetComponent<UnityEngine.UI.InputField>().text) / 10f));
    }
    public static void Test(Vector2 size)
    {
        for (int i = 0; i < GameObject.Find("BoardItems").transform.childCount; i++)
        {
            if (Mathf.Abs(GameObject.Find("BoardItems").transform.GetChild(i).position.x) > size.x / 2 || Mathf.Abs(GameObject.Find("BoardItems").transform.GetChild(i).position.y) > size.y / 2)
            {
                GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/ConfirmDialog"), GameObject.Find("Canvas").transform);
                obj.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = "This will result in the deletion of items.\nAre you sure?";
                obj.transform.Find("Accept").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
                {
                    for (int j = GameObject.Find("BoardItems").transform.childCount - 1; j >= 0; j--)
                    {
                        if (Mathf.Abs(GameObject.Find("BoardItems").transform.GetChild(j).position.x) > size.x / 2 || Mathf.Abs(GameObject.Find("BoardItems").transform.GetChild(j).position.y) > size.y / 2)
                        {
                            Object.Destroy(GameObject.Find("BoardItems").transform.GetChild(j).gameObject);
                        }
                    }
                    SetBoardSize(size);
                    Object.Destroy(obj);
                }));
                obj.transform.Find("Cancel").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
                {
                    Object.Destroy(obj);
                }));
                return;
            }
        }
        SetBoardSize(size);
    }

    public static void SetBoardSize(Vector2 size)
    {
        //print((Camera.main.transform.position.x > size.x / 2 + 1.25f) + " " + (Camera.main.transform.position.x < -size.x / 2 - 1.25f) + " " + (Camera.main.transform.position.y > size.y / 2 + 1.25f) + " " + (Camera.main.transform.position.y < -size.y / 2 - 1.25f));
        /*if (Camera.main.transform.position.x > size.x/2 + 1.25f)
        {
            //print("a");
            Camera.main.GetComponent<CharacterController>().Set(new Vector3(size.x / 2 - 1, Camera.main.transform.position.y, Camera.main.transform.position.z));
        }
        if (Camera.main.transform.position.x < -size.x/2 - 1.25f)
        {
            //print("b");
            Camera.main.GetComponent<CharacterController>().Set(new Vector3(-size.x / 2 + 1, Camera.main.transform.position.y, Camera.main.transform.position.z));
        }
        if (Camera.main.transform.position.y > size.y/2 + 1.25f)
        {
            //print("c");
            Camera.main.GetComponent<CharacterController>().Set(new Vector3(Camera.main.transform.position.x, size.y / 2 - 1, Camera.main.transform.position.z));
        }
        if (Camera.main.transform.position.y < -size.y/2 - 1.25f)
        {
            //print("d");
            Camera.main.GetComponent<CharacterController>().Set(new Vector3(Camera.main.transform.position.x, -size.y / 2 + 1, Camera.main.transform.position.z));
        }*/
        Camera.main.GetComponent<CharacterController>().Set(new Vector3(0, 0, Camera.main.transform.position.z));

        GameObject.Find("Back").transform.localScale = new Vector3(size.x, size.y, .05f);

        GameObject.Find("LeftEdge").transform.localPosition = new Vector3(-size.x/2 - .25f, 0, -.1f);
        GameObject.Find("LeftEdge").transform.localScale = new Vector3(.5f, size.y + 1, .1f);

        GameObject.Find("RightEdge").transform.localPosition = new Vector3(size.x/2 + .25f, 0, -.1f);
        GameObject.Find("RightEdge").transform.localScale = new Vector3(.5f, size.y + 1, .1f);

        GameObject.Find("TopEdge").transform.localPosition = new Vector3(0, size.y/2 + .25f, -.1f);
        GameObject.Find("TopEdge").transform.localScale = new Vector3(size.x, .5f, .1f);

        GameObject.Find("BottomEdge").transform.localPosition = new Vector3(0, -size.y/2 - .25f, -.1f);
        GameObject.Find("BottomEdge").transform.localScale = new Vector3(size.x, .5f, .1f);

        if (!SettingsManager._2dmode) GameObject.Find("BackWall").transform.localScale = new Vector3(size.x + 1000f, size.y + 1000f, .1f);

        GameObject.Find("LeftWall").transform.localScale = new Vector3(25, size.y + 2.5f, .1f);
        GameObject.Find("RightWall").transform.localScale = new Vector3(25, size.y + 2.5f, .1f);
        GameObject.Find("TopWall").transform.localScale = new Vector3(size.x + 2.5f, .1f, 25);
        GameObject.Find("BottomWall").transform.localScale = new Vector3(size.x + 2.5f, .1f, 25);

        GameObject.Find("LeftWall").transform.localPosition = new Vector3(-size.x / 2 - 1.25f, 0, -12.5f);
        GameObject.Find("RightWall").transform.localPosition = new Vector3(size.x / 2 + 1.25f, 0, -12.5f);
        GameObject.Find("TopWall").transform.localPosition = new Vector3(0, size.y / 2 + 1.25f, -12.5f);
        GameObject.Find("BottomWall").transform.localPosition = new Vector3(0, -size.y / 2 - 1.25f, -12.5f);
    }

    public static void Update()
    {
        GameObject.Find("BackWall").transform.localScale = new Vector3(GameObject.Find("Back").transform.localScale.x + 1000f, GameObject.Find("Back").transform.localScale.y + 1000f, .1f);

        if (SettingsManager._2dmode)
        {
            GameObject.Find("LeftWall").GetComponent<MeshRenderer>().enabled = false;
            GameObject.Find("RightWall").GetComponent<MeshRenderer>().enabled = false;
            GameObject.Find("TopWall").GetComponent<MeshRenderer>().enabled = false;
            GameObject.Find("BottomWall").GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            GameObject.Find("LeftWall").GetComponent<MeshRenderer>().enabled = true;
            GameObject.Find("RightWall").GetComponent<MeshRenderer>().enabled = true;
            GameObject.Find("TopWall").GetComponent<MeshRenderer>().enabled = true;
            GameObject.Find("BottomWall").GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
