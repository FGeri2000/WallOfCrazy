using System.IO;
using UnityEngine;
using UnityEngine.UI;

public static class SettingsManager
{
    public static bool _2dmode;
    public static bool imageLimit;
    public static Vector2Int window_size;
    public static DirectoryInfo lastImagePath = new DirectoryInfo(".");
    public static DirectoryInfo lastSavePath = new DirectoryInfo(".");

    public static int saveVersion = 3;

    public static void Write()
    {
        using (StreamWriter write = new StreamWriter("settings.cfg", false, System.Text.Encoding.Default))
        {
            write.WriteLine("{0}={1}", "fullscreen", Screen.fullScreen);
            if (Screen.fullScreen) {
                write.WriteLine("{0}={1}", "window_width", window_size.x);
                write.WriteLine("{0}={1}", "window_height", window_size.y);
            }
            else {
                write.WriteLine("{0}={1}", "window_width", Screen.width);
                write.WriteLine("{0}={1}", "window_height", Screen.height);
            }
            write.WriteLine("{0}={1}", "shadows", GameObject.Find("Sun").GetComponent<Light>().shadows == LightShadows.Soft ? true : false);
            write.WriteLine("{0}={1}", "2dmode", _2dmode);
            write.WriteLine("{0}={1}", "pinlimit", BoardItemController.Singleton.UsePinLimit);
            write.WriteLine("{0}={1}", "imagelimit", imageLimit);
            write.WriteLine("{0}={1}", "lastimagedir", lastImagePath);
            write.WriteLine("{0}={1}", "lastsavedir", lastSavePath);
        }
    }
    public static void Start()
    {
        bool fullscreen = true;
        int width = Screen.width, height = Screen.height;
        bool shadows = true;
        _2dmode = false;
        Screen.SetResolution(width, height, fullscreen);


        if (File.Exists("settings.cfg"))
        {
            StreamReader read = new StreamReader("settings.cfg");

            while (!read.EndOfStream)
            {
                string line = read.ReadLine();
                switch (line.Split('=')[0])
                {
                    case "fullscreen":
                        fullscreen = bool.Parse(line.Split('=')[1]);
                        break;
                    case "window_width":
                        width = int.Parse(line.Split('=')[1]);
                        break;
                    case "window_height":
                        height = int.Parse(line.Split('=')[1]);
                        break;
                    case "shadows":
                        shadows = bool.Parse(line.Split('=')[1]);
                        break;
                    case "2dmode":
                        _2dmode = bool.Parse(line.Split('=')[1]);
                        break;
                    case "pinlimit":
                        BoardItemController.Singleton.UsePinLimit = bool.Parse(line.Split('=')[1]);
                        break;
                    case "imagelimit":
                        imageLimit = bool.Parse(line.Split('=')[1]);
                        break;
                    case "lastimagedir":
                        lastImagePath = Directory.Exists(line.Split('=')[1]) ? new DirectoryInfo(line.Split('=')[1]) : new DirectoryInfo(".");
                        break;
                    case "lastsavedir":
                        lastSavePath = Directory.Exists(line.Split('=')[1]) ? new DirectoryInfo(line.Split('=')[1]) : new DirectoryInfo(".");
                        break;
                }
            }
            read.Close();
        }
        else
        {
            Write();
        }

        window_size = new Vector2Int(width, height);


        if (fullscreen) {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        else {
            Screen.SetResolution(width, height, false);
        }


        GameObject.Find("FullscreenToggle").GetComponent<Toggle>().isOn = fullscreen;
        GameObject.Find("ShadowsToggle").GetComponent<Toggle>().isOn = shadows;
        GameObject.Find("2DModeToggle").GetComponent<Toggle>().isOn = _2dmode;
        GameObject.Find("Sun").GetComponent<Light>().shadows = shadows ? LightShadows.Soft : LightShadows.None;
    }

    public static void MenuOpen(bool toggle)
    {
        if (toggle)
            GameObject.Find("MenuParent").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50);
        else
            GameObject.Find("MenuParent").GetComponent<RectTransform>().anchoredPosition = new Vector2(400, -50);
        Write();
    }

    public static void ShadowToggle(bool toggle)
    {
        if (toggle)
            GameObject.Find("Sun").GetComponent<Light>().shadows = LightShadows.Soft;
        else
            GameObject.Find("Sun").GetComponent<Light>().shadows = LightShadows.None;
        Write();
    }
    public static void FullscreenToggle(bool toggle)
    {
        if (toggle)
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        else
            Screen.SetResolution(window_size.x, window_size.y, false);
        Write();
    }
    public static void _2DModeToggle(bool toggle)
    {
        _2dmode = toggle;
        Write();
    }
    public static void ImgLimitToggle(bool toggle)
    {
        imageLimit = toggle;
        Write();
    }

    public static void ResolutionSet()
    {
        int val = GameObject.Find("ResolutionDropdown").GetComponent<Dropdown>().value;
        Screen.SetResolution(Screen.resolutions[val].width, Screen.resolutions[val].height, GameObject.Find("FullscreenToggle").GetComponent<Toggle>().isOn);
        Write();
    }
    
    enum SaveBlockType {
        Label = 0,
        Text = 1,
        Picture = 2,
        Line = 3,
        Connector = 4
    }

    public static void Load(string path)
    {
        path = path.Trim('\n', '\r', '\"', '\'', ' ', '\t');
        if (File.Exists(path))
        {
            BoardItemController.Singleton.ClearContext();
            BoardItemController.Singleton.PinCount = 0;
            GameObject.Find("Camera").transform.position = new Vector3(0, 0, -10);
            for (int i = GameObject.Find("BoardItems").transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(GameObject.Find("BoardItems").transform.GetChild(i).gameObject);
            }

            using (FileStream writer = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                int fileVersion = writer.ReadByte();

                //default colors
                float pr = writer.ReadByte() / 255f, pg = writer.ReadByte() / 255f, pb = writer.ReadByte() / 255f;
                float lr = writer.ReadByte() / 255f, lg = writer.ReadByte() / 255f, lb = writer.ReadByte() / 255f;
                BoardItemController.Singleton.SetPinColor(new Color(pr, pg, pb));
                BoardItemController.Singleton.SetLineColor(new Color(lr, lg, lb));
                BoardItemController.Singleton.UpdateColorSliderAndInput();

                BoardItemController.Singleton.SetLineWidth(writer.ReadByte());
                GameObject.Find("LineWidthSlider").GetComponent<Slider>().value = BoardItemController.Singleton.LineWidth;
                GameObject.Find("LineWidthLabel").GetComponent<Text>().text = "Line width: " + BoardItemController.Singleton.LineWidth;



                //board size
                int width = writer.ReadDWord(), height = writer.ReadDWord();
                if (fileVersion <= 1) {
                    GameObject.Find("WidthInput").GetComponent<Slider>().value = width;
                    GameObject.Find("HeightInput").GetComponent<Slider>().value = height;
                }
                else {
                    GameObject.Find("WidthInput").GetComponent<InputField>().text = Mathf.Lerp(17.5f, 100f, width).ToString();
                    GameObject.Find("HeightInput").GetComponent<InputField>().text = Mathf.Lerp(7.5f, 30f, height).ToString();
                }
                BoardSizeController.SetBoardSize(new Vector2(width / 10f, height / 10f));


                while (writer.Position < writer.Length)
                {
                    SaveBlockType type = (SaveBlockType)writer.ReadByte();
                    Vector3 vec;
                    float rot;
                    GameObject created;
                    char[] arr;

                    switch (type)
                    {
                        case SaveBlockType.Label:
                            vec = new Vector3((writer.ReadDWord() / 100f), (writer.ReadDWord() / 100f));
                            rot = writer.ReadDWord() / 100f;
                            created = BoardItemController.Singleton.CreateLabel(vec, rot);
                            created.GetComponent<PinScript>().SetColor(new Color(writer.ReadByte() / 255f, writer.ReadByte() / 255f, writer.ReadByte() / 255f));
                            created.name = "Pin" + writer.ReadDWord().ToString("d4");

                            created.GetComponent<PinScript>().scale = writer.ReadDWord() / 100f;
                            created.transform.localScale = new Vector3(created.GetComponent<PinScript>().scale, created.GetComponent<PinScript>().scale, created.GetComponent<PinScript>().scale);

                            arr = new char[writer.ReadWord()];
                            for (int i = 0; i < arr.Length; i++)
                            {
                                arr[i] = (char)writer.ReadByte();
                                arr[i] += (char)(writer.ReadByte() << 8);
                            }
                            created.transform.Find("Header").Find("Text").GetComponent<TMPro.TextMeshPro>().text = string.Concat(arr);
                            BoardItemController.Singleton.PinCount++;
                            break;
                        case SaveBlockType.Text:
                            vec = new Vector3((writer.ReadDWord() / 100f), (writer.ReadDWord() / 100f));
                            rot = writer.ReadDWord() / 100f;
                            created = BoardItemController.Singleton.CreateText(vec, rot);
                            created.GetComponent<PinScript>().SetColor(new Color(writer.ReadByte() / 255f, writer.ReadByte() / 255f, writer.ReadByte() / 255f));
                            created.name = "Pin" + writer.ReadDWord().ToString("d4");

                            arr = new char[writer.ReadWord()];
                            for (int i = 0; i < arr.Length; i++)
                            {
                                arr[i] = (char)writer.ReadByte();
                                arr[i] += (char)(writer.ReadByte() << 8);
                            }
                            created.transform.Find("Text").Find("Title").GetComponent<TMPro.TextMeshPro>().text = string.Concat(arr);

                            arr = new char[writer.ReadWord()];
                            for (int i = 0; i < arr.Length; i++)
                            {
                                arr[i] = (char)writer.ReadByte();
                                arr[i] += (char)(writer.ReadByte() << 8);
                            }
                            created.transform.Find("Text").Find("Text").GetComponent<TMPro.TextMeshPro>().text = string.Concat(arr);
                            BoardItemController.Singleton.PinCount++;
                            break;
                        case SaveBlockType.Picture:
                            vec = new Vector3((writer.ReadDWord() / 100f), (writer.ReadDWord() / 100f));
                            rot = writer.ReadDWord() / 100f;
                            created = BoardItemController.Singleton.CreatePicture(vec, rot);
                            created.GetComponent<PinScript>().SetColor(new Color(writer.ReadByte() / 255f, writer.ReadByte() / 255f, writer.ReadByte() / 255f));
                            created.name = "Pin" + writer.ReadDWord().ToString("d4");

                            created.GetComponent<PinScript>().width = writer.ReadDWord();
                            created.GetComponent<PinScript>().height = writer.ReadDWord();
                            created.GetComponent<PinScript>().scale = writer.ReadDWord() / 100f;

                            if (fileVersion <= 2)
                            {
                                Vector3 imgscale = new Vector3(writer.ReadDWord() / 100f, 1, writer.ReadDWord() / 100f);
                                created.transform.Find("Picture").localScale = imgscale;
                                created.transform.Find("Picture").localPosition = new Vector3(created.transform.Find("Picture").localPosition.x, -.3333333f - (created.transform.Find("Picture").localScale.z - 1) / 2.6666666f, created.transform.Find("Picture").localPosition.z);
                            }
                            else
                            {
                                created.GetComponent<PinScript>().ResizeImage();
                            }

                            byte[] imgdat = new byte[writer.ReadDWord()];
                            writer.Read(imgdat, 0, imgdat.Length);

                            Texture2D tx = new Texture2D(512, 512);
                            tx.LoadImage(imgdat);
                            created.transform.Find("Picture").Find("Image").GetComponent<MeshRenderer>().material.mainTexture = tx;
                            BoardItemController.Singleton.PinCount++;
                            break;
                        case SaveBlockType.Line:
                            created = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Line"), GameObject.Find("BoardItems").transform);
                            created.GetComponent<LineScript>().SetColor(new Color(writer.ReadByte() / 255f, writer.ReadByte() / 255f, writer.ReadByte() / 255f));
                            created.GetComponent<LineScript>().SetWidth(writer.ReadByte());
                            created.name = "Line" + writer.ReadDWord().ToString("d7");
                            string n1 = "Pin" + writer.ReadDWord().ToString("d4");
                            string n2 = "Pin" + writer.ReadDWord().ToString("d4");
                            created.GetComponent<LineScript>().pin1 = GameObject.Find(n1).GetComponentInChildren<PinScript>();
                            created.GetComponent<LineScript>().pin2 = GameObject.Find(n2).GetComponentInChildren<PinScript>();
                            created.GetComponent<LineScript>().valid = true;
                            break;
                        case SaveBlockType.Connector:
                            vec = new Vector3((writer.ReadDWord() / 100f), (writer.ReadDWord() / 100f));
                            rot = writer.ReadDWord() / 100f;
                            created = BoardItemController.Singleton.CreateEmpty(vec, rot);
                            created.GetComponent<PinScript>().SetColor(new Color(writer.ReadByte() / 255f, writer.ReadByte() / 255f, writer.ReadByte() / 255f));
                            created.name = "Pin" + writer.ReadDWord().ToString("d4");
                            BoardItemController.Singleton.PinCount++;
                            break;
                    }
                }
            }
        }
    }
    public static void Save(string path)
    {
        path = path.Trim('\n', '\r', '\"', '\'', ' ', '\t');
        using (FileStream writer = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            BoardItemController.Singleton.ClearContext();
            GameObject.Find("Camera").transform.position = new Vector3(0, 0, -10);

            writer.WriteByte((byte)saveVersion);
            //default pin color/line color 1-1 byte
            writer.WriteByte((byte)Mathf.RoundToInt(BoardItemController.Singleton.PinColor.r * 255));
            writer.WriteByte((byte)Mathf.RoundToInt(BoardItemController.Singleton.PinColor.g * 255));
            writer.WriteByte((byte)Mathf.RoundToInt(BoardItemController.Singleton.PinColor.b * 255));
            writer.WriteByte((byte)Mathf.RoundToInt(BoardItemController.Singleton.LineColor.r * 255));
            writer.WriteByte((byte)Mathf.RoundToInt(BoardItemController.Singleton.LineColor.g * 255));
            writer.WriteByte((byte)Mathf.RoundToInt(BoardItemController.Singleton.LineColor.b * 255));

            writer.WriteByte((byte)BoardItemController.Singleton.LineWidth);


            //board width 4 byte int
            writer.WriteDWord(int.Parse(GameObject.Find("WidthInput").GetComponent<InputField>().text));
            //board height 4 byte int
            writer.WriteDWord(int.Parse(GameObject.Find("HeightInput").GetComponent<InputField>().text));

            foreach (var item in Object.FindObjectsOfType<PinScript>())
            {
                Transform tform = item.transform;

                //label 0
                //text 1
                //picture 2
                //connector 4
                writer.WriteByte((byte)item.type);
                //x 4 byte
                writer.WriteDWord(Mathf.RoundToInt(tform.position.x * 100f));
                //y 4 byte
                writer.WriteDWord(Mathf.RoundToInt(tform.position.y * 100f));
                //rot 4 byte
                writer.WriteDWord(Mathf.RoundToInt(tform.eulerAngles.z * 100f));
                //color 3 byte
                writer.WriteByte((byte)Mathf.RoundToInt(item.color.r * 255));
                writer.WriteByte((byte)Mathf.RoundToInt(item.color.g * 255));
                writer.WriteByte((byte)Mathf.RoundToInt(item.color.b * 255));
                //number 4 byte
                writer.WriteDWord(int.Parse(item.name.Substring(3)));


                switch (item.type)
                {
                    case PinTypeEnum.Label:
                        writer.WriteDWord(Mathf.RoundToInt(item.scale * 100));
                        string text = item.transform.Find("Header").Find("Text").GetComponent<TMPro.TextMeshPro>().text;
                        //text size 2 byte int
                        writer.WriteWord((short)text.Length);
                        //text varsize vchar
                        for (int i = 0; i < (short)text.Length; i++)
                        {
                            writer.WriteByte((byte)text[i]);
                            writer.WriteByte((byte)(text[i] >> 8));
                        }
                        break;
                    case PinTypeEnum.Text:

                        //title size 2 byte int
                        string text1 = item.transform.Find("Text").Find("Title").GetComponent<TMPro.TextMeshPro>().text;
                        writer.WriteWord((short)text1.Length);
                        //title varsize wchar
                        for (int i = 0; i < (short)text1.Length; i++)
                        {
                            writer.WriteByte((byte)text1[i]);
                            writer.WriteByte((byte)(text1[i] >> 8));
                        }

                        //text size 2 byte int
                        //text varsize wchar
                        string text2 = item.transform.Find("Text").Find("Text").GetComponent<TMPro.TextMeshPro>().text;
                        writer.WriteWord((short)text2.Length);
                        for (int i = 0; i < (short)text2.Length; i++)
                        {
                            writer.WriteByte((byte)text2[i]);
                            writer.WriteByte((byte)(text2[i] >> 8));
                        }
                        break;
                    case PinTypeEnum.Picture:
                        writer.WriteDWord(item.width);
                        writer.WriteDWord(item.height);
                        writer.WriteDWord(Mathf.RoundToInt(item.scale * 100f));
                        if (saveVersion <= 2)
                        {
                            //x scale 2 byte short
                            writer.WriteDWord(Mathf.RoundToInt(item.transform.Find("Picture").localScale.x * 100f));
                            //y scale 2 byte short
                            writer.WriteDWord(Mathf.RoundToInt(item.transform.Find("Picture").localScale.z * 100f));
                        }

                        byte[] arr = ((Texture2D)item.transform.Find("Picture").Find("Image").GetComponent<MeshRenderer>().material.mainTexture).EncodeToPNG();
                        //data length 4 byte int
                        writer.WriteDWord(arr.Length);
                        //data varsize
                        writer.Write(arr, 0, arr.Length);
                        break;
                    case PinTypeEnum.Empty:
                        break;
                }
            }



            foreach (var item in Object.FindObjectsOfType<LineScript>())
            {
                Transform tform = item.transform;

                //line 3
                writer.WriteByte(3);
                //color 1 byte
                writer.WriteByte((byte)Mathf.RoundToInt(item.color.r * 255));
                writer.WriteByte((byte)Mathf.RoundToInt(item.color.g * 255));
                writer.WriteByte((byte)Mathf.RoundToInt(item.color.b * 255));
                //width 1 byte

                writer.WriteByte((byte)item.width);

                writer.WriteDWord(int.Parse(item.name.Substring(5)));

                //pin1 name 4 byte int
                writer.WriteDWord(int.Parse(item.pin1.name.Substring(3)));
                //pin2 name 4 byte int
                writer.WriteDWord(int.Parse(item.pin2.name.Substring(3)));
            }
        }
    }
}
