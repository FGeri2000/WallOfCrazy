using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BoardItemController : MonoBehaviour
{
    public static BoardItemController Singleton { get { return GameObject.Find("Room").GetComponent<BoardItemController>(); } }

    private readonly int pinlimit = 4096;
    private readonly int linelimit = 8386560;

    public bool UsePinLimit { get; set; } = true;

    public int PinCount = 0;

    public void PinButtonCallback(int col) {
        int r = 0, g = 0, b = 0;

        switch ((ColorEnum)col) {
            case ColorEnum.Red:
                r = 228;
                break;
            case ColorEnum.Green:
                g = 228;
                break;
            case ColorEnum.Blue:
                b = 228;
                break;
            case ColorEnum.Cyan:
                g = 228;
                b = 228;
                break;
            case ColorEnum.Yellow:
                r = 228;
                g = 228;
                break;
            case ColorEnum.Magenta:
                r = 228;
                b = 228;
                break;
        }
        SetPinColor(new Color(r / 255f, g / 255f, b / 255f));
        UpdateColorSliderAndInput();
    }
    public void LineButtonCallback(int col) {
        int r = 0, g = 0, b = 0;

        switch ((ColorEnum)col) {
            case ColorEnum.Red:
                r = 228;
                break;
            case ColorEnum.Green:
                g = 228;
                break;
            case ColorEnum.Blue:
                b = 228;
                break;
            case ColorEnum.Cyan:
                g = 228;
                b = 228;
                break;
            case ColorEnum.Yellow:
                r = 228;
                g = 228;
                break;
            case ColorEnum.Magenta:
                r = 228;
                b = 228;
                break;
        }
        SetLineColor(new Color(r / 255f, g / 255f, b / 255f));
        UpdateColorSliderAndInput();
    }

    public void UpdateColorSliderAndInput() {
        GameObject.Find("PinInputRed").GetComponent<InputField>().text = (PinColor.r * 255).ToString();
        GameObject.Find("PinInputGreen").GetComponent<InputField>().text = (PinColor.g * 255).ToString();
        GameObject.Find("PinInputBlue").GetComponent<InputField>().text = (PinColor.b * 255).ToString();

        GameObject.Find("PinSliderRed").GetComponent<Slider>().value = PinColor.r * 255;
        GameObject.Find("PinSliderGreen").GetComponent<Slider>().value = PinColor.g * 255;
        GameObject.Find("PinSliderBlue").GetComponent<Slider>().value = PinColor.b * 255;

        GameObject.Find("LineInputRed").GetComponent<InputField>().text = (LineColor.r * 255).ToString();
        GameObject.Find("LineInputGreen").GetComponent<InputField>().text = (LineColor.g * 255).ToString();
        GameObject.Find("LineInputBlue").GetComponent<InputField>().text = (LineColor.b * 255).ToString();

        GameObject.Find("LineSliderRed").GetComponent<Slider>().value = LineColor.r * 255;
        GameObject.Find("LineSliderGreen").GetComponent<Slider>().value = LineColor.g * 255;
        GameObject.Find("LineSliderBlue").GetComponent<Slider>().value = LineColor.b * 255;
    }

    public void InputCheck() {
        int pr = int.Parse(GameObject.Find("PinInputRed").GetComponent<InputField>().text);
        int pg = int.Parse(GameObject.Find("PinInputGreen").GetComponent<InputField>().text);
        int pb = int.Parse(GameObject.Find("PinInputBlue").GetComponent<InputField>().text);

        int lr = int.Parse(GameObject.Find("LineInputRed").GetComponent<InputField>().text);
        int lg = int.Parse(GameObject.Find("LineInputGreen").GetComponent<InputField>().text);
        int lb = int.Parse(GameObject.Find("LineInputBlue").GetComponent<InputField>().text);

        if (pr > 255 || pr < 0) {
            GameObject.Find("PinInputRed").GetComponent<InputField>().text = Mathf.Clamp(pr, 0, 255).ToString();
        }

        if (pg > 255 || pg < 0) {
            GameObject.Find("PinInputGreen").GetComponent<InputField>().text = Mathf.Clamp(pg, 0, 255).ToString();
        }

        if (pb > 255 || pb < 0) {
            GameObject.Find("PinInputBlue").GetComponent<InputField>().text = Mathf.Clamp(pb, 0, 255).ToString();
        }

        if (lr > 255 || lr < 0) {
            GameObject.Find("LineInputRed").GetComponent<InputField>().text = Mathf.Clamp(lr, 0, 255).ToString();
        }

        if (lg > 255 || lg < 0) {
            GameObject.Find("LineInputGreen").GetComponent<InputField>().text = Mathf.Clamp(lg, 0, 255).ToString();
        }

        if (lb > 255 || lb < 0) {
            GameObject.Find("LineInputBlue").GetComponent<InputField>().text = Mathf.Clamp(lb, 0, 255).ToString();
        }
    }

    public int LineWidth { get; private set; } = 2;
    public void SetLineWidth(float width) {
        //int val = (int)GameObject.Find("LineWidthSlider").GetComponent<Slider>().value;
        //GameObject.Find("LineWidthLabel").GetComponent<Text>().text = "Line width: " + (val);
        //LineWidth = val;

        GameObject.Find("LineWidthSlider").GetComponent<Slider>().value = width;
        GameObject.Find("LineWidthLabel").GetComponent<Text>().text = "Line width: " + width;
        LineWidth = (int)width;
    }
    public Color PinColor { get; private set; }
    public void SetPinColor(Color col) {
        PinColor = col;
        GameObject.Find("PinDisplay").GetComponent<Image>().color = col;
    }
    public Color LineColor { get; private set; }
    public void SetLineColor(Color col) {
        LineColor = col;
        GameObject.Find("LineDisplay").GetComponent<Image>().color = col;
    }

    public bool DialogPresent { get { return dialog != null; } }

    private GameObject selected;
    private GameObject line;
    private GameObject dragged;
    private Vector3 dragpivot;
    private GameObject editor;
    private GameObject dialog;
    private StandaloneInputModuleV2 StandaloneInputModuleV2 { get { return GameObject.Find("EventSystem").GetComponent<StandaloneInputModuleV2>(); } }

    public void Quit() {
        SettingsManager.Write();
        dialog = Instantiate(Resources.Load<GameObject>("Prefabs/ConfirmDialog"), GameObject.Find("Canvas").transform);
        dialog.transform.Find("Text").GetComponent<Text>().text = "Are you sure? Unsaved changes will be lost.";
        dialog.transform.Find("Accept").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
            Application.Quit();
        }));
        dialog.transform.Find("Cancel").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
            Destroy(dialog);
            dialog = null;
        }));
    }

    public void Start() {
        PinButtonCallback(0);
        LineButtonCallback(0);

        GameObject.Find("PinSliderRed").GetComponent<Slider>().onValueChanged.AddListener((val) => {
            GameObject.Find("PinInputRed").GetComponent<InputField>().text = val.ToString();
        });
        GameObject.Find("PinSliderGreen").GetComponent<Slider>().onValueChanged.AddListener((val) => {
            GameObject.Find("PinInputGreen").GetComponent<InputField>().text = val.ToString();
        });
        GameObject.Find("PinSliderBlue").GetComponent<Slider>().onValueChanged.AddListener((val) => {
            GameObject.Find("PinInputBlue").GetComponent<InputField>().text = val.ToString();
        });

        GameObject.Find("LineSliderRed").GetComponent<Slider>().onValueChanged.AddListener((val) => {
            GameObject.Find("LineInputRed").GetComponent<InputField>().text = val.ToString();
        });
        GameObject.Find("LineSliderGreen").GetComponent<Slider>().onValueChanged.AddListener((val) => {
            GameObject.Find("LineInputGreen").GetComponent<InputField>().text = val.ToString();
        });
        GameObject.Find("LineSliderBlue").GetComponent<Slider>().onValueChanged.AddListener((val) => {
            GameObject.Find("LineInputBlue").GetComponent<InputField>().text = val.ToString();
        });

        GameObject.Find("PinInputRed").GetComponent<InputField>().onValueChanged.AddListener((UnityEngine.Events.UnityAction<string>)((str) => {
            int val = int.Parse(str);
            if (val > 255 || val < 0) {
                val = Mathf.Clamp(val, 0, 255);
            }

            GameObject.Find("PinSliderRed").GetComponent<Slider>().value = val;
            SetPinColor(new Color(val / 255f, (float)PinColor.g, (float)PinColor.b));
        }));
        GameObject.Find("PinInputGreen").GetComponent<InputField>().onValueChanged.AddListener((UnityEngine.Events.UnityAction<string>)((str) => {
            int val = int.Parse(str);
            if (val > 255 || val < 0) {
                val = Mathf.Clamp(val, 0, 255);
            }

            GameObject.Find("PinSliderGreen").GetComponent<Slider>().value = val;
            SetPinColor(new Color((float)PinColor.r, val / 255f, (float)PinColor.b));
        }));
        GameObject.Find("PinInputBlue").GetComponent<InputField>().onValueChanged.AddListener((UnityEngine.Events.UnityAction<string>)((str) => {
            int val = int.Parse(str);
            if (val > 255 || val < 0) {
                val = Mathf.Clamp(val, 0, 255);
            }

            GameObject.Find("PinSliderBlue").GetComponent<Slider>().value = val;
            SetPinColor(new Color((float)PinColor.r, (float)PinColor.g, val / 255f));
        }));

        GameObject.Find("LineInputRed").GetComponent<InputField>().onValueChanged.AddListener((UnityEngine.Events.UnityAction<string>)((str) => {
            int val = int.Parse(str);
            if (val > 255 || val < 0) {
                val = Mathf.Clamp(val, 0, 255);
            }

            GameObject.Find("LineSliderRed").GetComponent<Slider>().value = val;
            SetLineColor(new Color(val / 255f, (float)LineColor.g, (float)LineColor.b));
        }));
        GameObject.Find("LineInputGreen").GetComponent<InputField>().onValueChanged.AddListener((UnityEngine.Events.UnityAction<string>)((str) => {
            int val = int.Parse(str);
            if (val > 255 || val < 0) {
                val = Mathf.Clamp(val, 0, 255);
            }

            GameObject.Find("LineSliderGreen").GetComponent<Slider>().value = val;
            SetLineColor(new Color((float)LineColor.r, val / 255f, (float)LineColor.b));
        }));
        GameObject.Find("LineInputBlue").GetComponent<InputField>().onValueChanged.AddListener((UnityEngine.Events.UnityAction<string>)((str) => {
            int val = int.Parse(str);
            if (val > 255 || val < 0) {
                val = Mathf.Clamp(val, 0, 255);
            }

            GameObject.Find("LineSliderBlue").GetComponent<Slider>().value = val;
            SetLineColor(new Color((float)LineColor.r, (float)LineColor.g, val / 255f));
        }));


        System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
        foreach (var item in Screen.resolutions) {
            list.Add(item.ToString());
        }

        GameObject.Find("ImageLimitToggle").GetComponent<Toggle>().onValueChanged.AddListener((val) => {
            Debug.Log("imglim change: " + !val);
            SettingsManager.imageLimit = !val;
        });

        GameObject created;
        GameObject.Find("LoadButton").GetComponent<Button>().onClick.AddListener(() => {
            created = Instantiate(Resources.Load<GameObject>("Prefabs/LoadDialog"), GameObject.Find("Canvas").transform);
            GameObject.Find("LoadButton").GetComponent<Button>().enabled = false;
            GameObject.Find("SaveButton").GetComponent<Button>().enabled = false;
            created.GetComponentInChildren<FileDisplay>().cancel = new System.Action(() => {
                GameObject.Find("LoadButton").GetComponent<Button>().enabled = true;
                GameObject.Find("SaveButton").GetComponent<Button>().enabled = true;
            });
            created.GetComponentInChildren<FileDisplay>().fileAction = new System.Action<FileInfo>((inf) => {
                if (!inf.Exists) return;
                SettingsManager.lastSavePath = inf.Directory;

                dialog = Instantiate(Resources.Load<GameObject>("Prefabs/ConfirmDialog"), GameObject.Find("Canvas").transform);
                dialog.transform.Find("Text").GetComponent<Text>().text = "Are you sure? Unsaved changes will be lost.";
                dialog.transform.Find("Accept").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                    SettingsManager.Load(inf.FullName);
                    GameObject.Find("SaveButton").GetComponent<Button>().enabled = true;
                    GameObject.Find("LoadButton").GetComponent<Button>().enabled = true;
                    Destroy(dialog);
                    Destroy(created);
                    dialog = null;
                }));
                dialog.transform.Find("Cancel").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                    Destroy(dialog);
                    dialog = null;
                }));
            });

            created.GetComponentInChildren<FileDisplay>().Display(SettingsManager.lastSavePath, ".cbs");
        });
        GameObject.Find("SaveButton").GetComponent<Button>().onClick.AddListener(() => {
            created = Instantiate(Resources.Load<GameObject>("Prefabs/FileSaveDialog"), GameObject.Find("Canvas").transform);

            GameObject.Find("LoadButton").GetComponent<Button>().enabled = false;
            GameObject.Find("SaveButton").GetComponent<Button>().enabled = false;

            created.GetComponentInChildren<FileDisplay>().cancel = new System.Action(() => {
                GameObject.Find("LoadButton").GetComponent<Button>().enabled = true;
                GameObject.Find("SaveButton").GetComponent<Button>().enabled = true;
            });

            created.transform.Find("Save").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                string path = created.transform.Find("Path").GetComponent<InputField>().text;
                string name = created.transform.Find("InputField").GetComponent<InputField>().text == "" ? "Unnamed" : created.transform.Find("InputField").GetComponent<InputField>().text;

                if (File.Exists(path + Path.DirectorySeparatorChar + name + ".cbs")) {
                    dialog = Instantiate(Resources.Load<GameObject>("Prefabs/ConfirmDialog"), GameObject.Find("Canvas").transform);
                    dialog.transform.Find("Text").GetComponent<Text>().text = "Are you sure you want to overwrite this file?";
                    dialog.transform.Find("Accept").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                        SettingsManager.Save(path + Path.DirectorySeparatorChar + name + ".cbs");
                        GameObject.Find("LoadButton").GetComponent<Button>().enabled = true;
                        GameObject.Find("SaveButton").GetComponent<Button>().enabled = true;
                        Destroy(dialog);
                        Destroy(created);
                        dialog = null;
                    }));
                    dialog.transform.Find("Cancel").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                        Destroy(dialog);
                        dialog = null;
                    }));
                }
                else {
                    SettingsManager.Save(path + Path.DirectorySeparatorChar + name + ".cbs");
                    GameObject.Find("LoadButton").GetComponent<Button>().enabled = true;
                    GameObject.Find("SaveButton").GetComponent<Button>().enabled = true;
                    Destroy(created);
                }
            }));

            created.GetComponentInChildren<FileDisplay>().fileAction = new System.Action<FileInfo>((inf) => {
                if (!inf.Exists) return;

                dialog = Instantiate(Resources.Load<GameObject>("Prefabs/ConfirmDialog"), GameObject.Find("Canvas").transform);
                dialog.transform.Find("Text").GetComponent<Text>().text = "Are you sure you want to overwrite this file?";
                dialog.transform.Find("Accept").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                    SettingsManager.Save(inf.FullName);
                    GameObject.Find("LoadButton").GetComponent<Button>().enabled = true;
                    GameObject.Find("SaveButton").GetComponent<Button>().enabled = true;
                    Destroy(dialog);
                    Destroy(created);
                    dialog = null;
                }));
                dialog.transform.Find("Cancel").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                    Destroy(dialog);
                    dialog = null;
                }));
            });

            created.GetComponentInChildren<FileDisplay>().Display(SettingsManager.lastSavePath, ".cbs");
        });
    }
    public void Update() {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono), out RaycastHit info, 50f);


        if (Input.GetMouseButtonDown(0)) {
            if (StandaloneInputModuleV2.GameObjectUnderPointer() == null) {
                ClearContext();
            }

            if (Input.GetKey(KeyCode.LeftShift)) {
                ShiftLeftClick();
            }
            else {
                LeftClick();
            }
        }
        if (Input.GetMouseButton(0) && info.transform != null && !selected) {
            if (!dragged && info.transform.parent?.parent?.GetComponent<PinScript>() && (info.transform.name == "Pin") && (StandaloneInputModuleV2.GameObjectUnderPointer() == null)) {
                Transform tform = info.transform.parent.parent;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono), out info, 50f, ~LayerMask.GetMask("BoardItem", "Ignore Raycast"));
                dragged = tform.gameObject;
                dragpivot = tform.position - info.point;
            }
            else if (dragged) {
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono), out info, 50f, ~LayerMask.GetMask("BoardItem", "Ignore Raycast"));
                if (info.transform.name == "Back") {
                    dragged.transform.position = info.point + dragpivot;
                }
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            dragged = null;
        }
        if (Input.GetMouseButtonDown(1)) {
            if (StandaloneInputModuleV2.GameObjectUnderPointer() == null) {
                ClearContext();
            }

            selected = null;
            Destroy(line);
            RightClick();
        }
        if (selected) {
            line.GetComponent<LineRenderer>().SetPosition(1, new Vector3(info.point.x, info.point.y, info.point.z - .20f));
        }
    }

    private void LeftClick() {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono), out RaycastHit info);
        if (selected) {
            if (info.transform.parent.parent.GetComponent<PinScript>()) {
                Transform tform = info.transform.parent.parent;

                foreach (var item in Object.FindObjectsOfType<LineScript>()) {
                    if ((item.pin1 == selected.GetComponent<PinScript>() && item.pin2 == tform.GetComponent<PinScript>()) || (item.pin2 == selected.GetComponent<PinScript>() && item.pin1 == tform.GetComponent<PinScript>())) {
                        Destroy(item.gameObject);
                    }
                }

                line.GetComponent<LineScript>().pin1 = selected.GetComponent<PinScript>();
                line.GetComponent<LineScript>().pin2 = tform.GetComponent<PinScript>();
                line.GetComponent<LineScript>().valid = true;

                line.GetComponent<LineRenderer>().SetPosition(1, new Vector3(info.transform.position.x, info.transform.position.y, info.transform.position.z - .20f));
                selected = null;
                line = null;
            }
            else {
                selected = null;
                Destroy(line);
            }
        }
    }
    private void ShiftLeftClick() {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono), out RaycastHit info);
        if (!selected && info.transform.parent.parent.GetComponent<PinScript>()) {
            Transform tform = info.transform.parent.parent;
            selected = tform.gameObject;

            line = Instantiate(Resources.Load<GameObject>("Prefabs/Line"), GameObject.Find("BoardItems").transform);
            for (int i = 1; i <= linelimit; i++) {
                if (!GameObject.Find("Line" + i.ToString("d7"))) {
                    line.name = "Line" + i.ToString("d7");
                    break;
                }
            }
            line.GetComponent<LineRenderer>().SetPosition(0, selected.transform.Find("LineTarget").position);
            line.GetComponent<LineRenderer>().material = Instantiate(Resources.Load<Material>("Materials/Colors/LineBasic"));

            line.GetComponent<LineScript>().SetWidth(LineWidth);
            line.GetComponent<LineScript>().SetColor(LineColor);
        }
        else if (selected) {
            if (info.transform.parent.parent.GetComponentInChildren<PinScript>()) {
                Transform tform = info.transform.parent.parent;

                line.GetComponent<LineScript>().pin1 = selected.GetComponent<PinScript>();
                line.GetComponent<LineScript>().pin2 = tform.GetComponent<PinScript>();
                line.GetComponent<LineScript>().valid = true;

                line.GetComponent<LineRenderer>().SetPosition(1, new Vector3(info.transform.position.x, info.transform.position.y, info.transform.position.z - .20f));
                selected = null;
                line = null;
            }
            else {
                selected = null;
                Destroy(line);
            }
        }
    }
    private void RightClick() {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono), out RaycastHit info);
        ClearContext();

        if (info.transform?.name == "Back") {
            Vector2 can = new Vector2(GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.x, GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.y);

            ClearContext();
            if (PinCount < pinlimit || !UsePinLimit) {
                GameObject button1 = Instantiate(Resources.Load<GameObject>("Prefabs/Button"), GameObject.Find("ContextParent").transform);
                button1.transform.Find("Text").GetComponent<Text>().text = "Create Label";
                button1.GetComponent<RectTransform>().anchoredPosition = new Vector2((Input.mousePosition.x / Screen.safeArea.width) * can.x + (Input.mousePosition.x < Screen.safeArea.width / 3 ? 80 : -80), (Input.mousePosition.y / Screen.safeArea.height) * can.y + (Input.mousePosition.y < Screen.safeArea.height / 3 ? 15 : -15));
                button1.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                    CreateLabel(info.point, Random.Range(-5, 5));
                    ClearContext();
                }));


                GameObject button2 = Instantiate(Resources.Load<GameObject>("Prefabs/Button"), GameObject.Find("ContextParent").transform);
                button2.transform.Find("Text").GetComponent<Text>().text = "Create Text";
                button2.GetComponent<RectTransform>().anchoredPosition = new Vector2((Input.mousePosition.x / Screen.safeArea.width) * can.x + (Input.mousePosition.x < Screen.safeArea.width / 3 ? 80 : -80), (Input.mousePosition.y / Screen.safeArea.height) * can.y + (Input.mousePosition.y < Screen.safeArea.height / 3 ? 45 : -45));
                button2.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                    CreateText(info.point, Random.Range(-5, 5));
                    ClearContext();
                }));

                GameObject button3 = Instantiate(Resources.Load<GameObject>("Prefabs/Button"), GameObject.Find("ContextParent").transform);
                button3.transform.Find("Text").GetComponent<Text>().text = "Create Picture";
                button3.GetComponent<RectTransform>().anchoredPosition = new Vector2((Input.mousePosition.x / Screen.safeArea.width) * can.x + (Input.mousePosition.x < Screen.safeArea.width / 3 ? 80 : -80), (Input.mousePosition.y / Screen.safeArea.height) * can.y + (Input.mousePosition.y < Screen.safeArea.height / 3 ? 75 : -75));
                button3.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                    CreatePicture(info.point, Random.Range(-5, 5));
                    ClearContext();
                }));

                GameObject button4 = Instantiate(Resources.Load<GameObject>("Prefabs/Button"), GameObject.Find("ContextParent").transform);
                button4.transform.Find("Text").GetComponent<Text>().text = "Create Connector";
                button4.GetComponent<RectTransform>().anchoredPosition = new Vector2((Input.mousePosition.x / Screen.safeArea.width) * can.x + (Input.mousePosition.x < Screen.safeArea.width / 3 ? 80 : -80), (Input.mousePosition.y / Screen.safeArea.height) * can.y + (Input.mousePosition.y < Screen.safeArea.height / 3 ? 105 : -105));
                button4.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                    CreateEmpty(info.point, Random.Range(-5, 5));
                    ClearContext();
                }));
            }
            else {
                GameObject button1 = Instantiate(Resources.Load<GameObject>("Prefabs/Button"), GameObject.Find("ContextParent").transform);
                button1.transform.Find("Text").GetComponent<Text>().text = "Pin limit reached";
                button1.GetComponent<RectTransform>().anchoredPosition = new Vector2((Input.mousePosition.x / Screen.safeArea.width) * can.x + (Input.mousePosition.x < Screen.safeArea.width / 3 ? 80 : -80), (Input.mousePosition.y / Screen.safeArea.height) * can.y + (Input.mousePosition.y < Screen.safeArea.height / 3 ? 15 : -15));
                button1.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                    CreateLabel(info.point, Random.Range(-5, 5));
                    ClearContext();
                }));
                button1.GetComponent<Button>().interactable = false;
            }
        }

        if (info.transform?.parent.parent.GetComponent<PinScript>()) {
            PinScript script = info.transform.parent.parent.GetComponent<PinScript>();

            switch (script.type) {
                case PinTypeEnum.Label:
                    editor = Instantiate(Resources.Load<GameObject>("Prefabs/LabelEditor"), GameObject.Find("Canvas").transform);
                    editor.transform.Find("InputField").GetComponent<InputField>().text = script.transform.Find("Header").Find("Text").GetComponent<TMPro.TextMeshPro>().text;
                    editor.transform.Find("InputField").GetComponent<InputField>().onValueChanged.AddListener(new UnityEngine.Events.UnityAction<string>((val) => {
                        script.transform.Find("Header").Find("Text").GetComponent<TMPro.TextMeshPro>().text = val;
                    }));
                    editor.transform.Find("ScaleSlider").GetComponent<Slider>().value = script.scale * 100;
                    editor.transform.Find("ScaleSlider").GetComponent<Slider>().onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>((val) => {
                        script.scale = val / 100;
                        script.transform.localScale = new Vector3(script.scale, script.scale, script.scale);
                        script.transform.localPosition = new Vector3(script.transform.localPosition.x, /*-.3333333f - ((val / 100) - 1) / 2.6666666f*/script.transform.localPosition.y, script.transform.localPosition.z);
                    }));

                    break;
                case PinTypeEnum.Text:
                    editor = Instantiate(Resources.Load<GameObject>("Prefabs/TextEditor"), GameObject.Find("Canvas").transform);
                    editor.transform.Find("TitleInputField").GetComponent<InputField>().text = script.transform.Find("Text").Find("Title").GetComponent<TMPro.TextMeshPro>().text;
                    editor.transform.Find("TitleInputField").GetComponent<InputField>().onValueChanged.AddListener(new UnityEngine.Events.UnityAction<string>((val) => {
                        script.transform.Find("Text").Find("Title").GetComponent<TMPro.TextMeshPro>().text = val;
                    }));
                    editor.transform.Find("TextInputField").GetComponent<InputField>().text = script.transform.Find("Text").Find("Text").GetComponent<TMPro.TextMeshPro>().text;
                    editor.transform.Find("TextInputField").GetComponent<InputField>().onValueChanged.AddListener(new UnityEngine.Events.UnityAction<string>((val) => {
                        script.transform.Find("Text").Find("Text").GetComponent<TMPro.TextMeshPro>().text = val;
                    }));
                    break;
                case PinTypeEnum.Picture:
                    editor = Instantiate(Resources.Load<GameObject>("Prefabs/PictureEditor"), GameObject.Find("Canvas").transform);
                    editor.transform.Find("InputField").GetComponent<InputField>().text = script.imagePath;
                    editor.transform.Find("InputField").GetComponent<InputField>().onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>((val) => {
                        val = val.Trim('\n', '\r', '\"', '\'', ' ', '\t');
                        if (File.Exists(val)) {
                            using (FileStream stream = new FileStream(val, FileMode.Open, FileAccess.Read)) {
                                byte[] arr = new byte[stream.Length];
                                stream.Read(arr, 0, (int)stream.Length);
                                Texture2D tex = new Texture2D(0, 0);
                                tex.LoadImage(arr);

                                int w = tex.width;
                                int h = tex.height;
                                if (SettingsManager.imageLimit && (w > 512 || h > 512)) {
                                    if (((float)w) / h > 1) {
                                        Debug.Log("512 " + (int)((512f / w) * h));
                                        TextureScale.Bilinear(tex, 512, (int)((512f / w) * h));
                                    }
                                    else if (((float)w) / h < 1) {
                                        Debug.Log((int)((512f / h) * w) + " 512");
                                        TextureScale.Bilinear(tex, (int)((512f / h) * w), 512);
                                    }
                                    else TextureScale.Bilinear(tex, 512, 512);
                                }
                                script.width = tex.width;
                                script.height = tex.height;
                                script.transform.Find("Picture").Find("Image").GetComponent<MeshRenderer>().material.mainTexture = tex;

                                script.ResizeImage();
                            }
                            script.imagePath = val;
                        }
                    }));

                    editor.transform.Find("Slider").GetComponent<Slider>().value = script.scale;
                    editor.transform.Find("Slider").GetComponent<Slider>().onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>((val) => {
                        script.scale = val / 100f;
                        script.ResizeImage();
                    }));

                    editor.transform.Find("LoadImage").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                        GameObject created = Instantiate(Resources.Load<GameObject>("Prefabs/LoadDialog"), GameObject.Find("Canvas").transform);
                        editor.transform.Find("InputField").GetComponent<InputField>().enabled = false;
                        editor.transform.Find("Slider").GetComponent<Slider>().enabled = false;
                        editor.transform.Find("Delete").GetComponent<Button>().enabled = false;
                        editor.transform.Find("Disconnect").GetComponent<Button>().enabled = false;
                        editor.transform.Find("Close").GetComponent<Button>().enabled = false;
                        editor.transform.Find("LoadImage").GetComponent<Button>().enabled = false;
                        editor.transform.Find("SaveImage").GetComponent<Button>().enabled = false;
                        created.GetComponentInChildren<FileDisplay>().cancel = new System.Action(() => {
                            editor.transform.Find("InputField").GetComponent<InputField>().enabled = true;
                            editor.transform.Find("Slider").GetComponent<Slider>().enabled = true;
                            editor.transform.Find("Delete").GetComponent<Button>().enabled = true;
                            editor.transform.Find("Disconnect").GetComponent<Button>().enabled = true;
                            editor.transform.Find("Close").GetComponent<Button>().enabled = true;
                            editor.transform.Find("LoadImage").GetComponent<Button>().enabled = true;
                            editor.transform.Find("SaveImage").GetComponent<Button>().enabled = true;
                        });
                        created.GetComponentInChildren<FileDisplay>().fileAction = new System.Action<FileInfo>((inf) => {
                            if (!inf.Exists) return;
                            SettingsManager.lastImagePath = inf.Directory;
                            editor.transform.Find("InputField").GetComponent<InputField>().text = inf.FullName;
                            editor.transform.Find("InputField").GetComponent<InputField>().onEndEdit.Invoke(inf.FullName);
                            editor.transform.Find("InputField").GetComponent<InputField>().enabled = true;
                            editor.transform.Find("Slider").GetComponent<Slider>().enabled = true;
                            editor.transform.Find("Delete").GetComponent<Button>().enabled = true;
                            editor.transform.Find("Disconnect").GetComponent<Button>().enabled = true;
                            editor.transform.Find("Close").GetComponent<Button>().enabled = true;
                            editor.transform.Find("LoadImage").GetComponent<Button>().enabled = true;
                            editor.transform.Find("SaveImage").GetComponent<Button>().enabled = true;

                            Destroy(created);
                        });

                        created.GetComponentInChildren<FileDisplay>().Display(SettingsManager.lastImagePath, ".jpg", ".jpeg", ".png");
                    }));
                    editor.transform.Find("SaveImage").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                        GameObject created = Instantiate(Resources.Load<GameObject>("Prefabs/ImageSaveDialog"), GameObject.Find("Canvas").transform);
                        editor.transform.Find("InputField").GetComponent<InputField>().enabled = false;
                        editor.transform.Find("Slider").GetComponent<Slider>().enabled = false;
                        editor.transform.Find("Delete").GetComponent<Button>().enabled = false;
                        editor.transform.Find("Disconnect").GetComponent<Button>().enabled = false;
                        editor.transform.Find("Close").GetComponent<Button>().enabled = false;
                        editor.transform.Find("LoadImage").GetComponent<Button>().enabled = false;
                        editor.transform.Find("SaveImage").GetComponent<Button>().enabled = false;
                        created.GetComponentInChildren<FileDisplay>().cancel = new System.Action(() => {
                            editor.transform.Find("InputField").GetComponent<InputField>().enabled = true;
                            editor.transform.Find("Slider").GetComponent<Slider>().enabled = true;
                            editor.transform.Find("Delete").GetComponent<Button>().enabled = true;
                            editor.transform.Find("Disconnect").GetComponent<Button>().enabled = true;
                            editor.transform.Find("Close").GetComponent<Button>().enabled = true;
                            editor.transform.Find("LoadImage").GetComponent<Button>().enabled = true;
                            editor.transform.Find("SaveImage").GetComponent<Button>().enabled = true;
                        });

                        created.transform.Find("Save").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                            string path = created.transform.Find("Path").GetComponent<InputField>().text;
                            string name = created.transform.Find("InputField").GetComponent<InputField>().text == "" ? "Unnamed" : created.transform.Find("InputField").GetComponent<InputField>().text;
                            string ext = created.transform.Find("FormatDropdown").GetComponent<Dropdown>().options[created.transform.Find("FormatDropdown").GetComponent<Dropdown>().value].text;
                            if (File.Exists(path + Path.DirectorySeparatorChar + name + ext)) {
                                dialog = Instantiate(Resources.Load<GameObject>("Prefabs/ConfirmDialog"), GameObject.Find("Canvas").transform);
                                dialog.transform.Find("Text").GetComponent<Text>().text = "Are you sure you want to overwrite this file?";
                                dialog.transform.Find("Accept").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                                    using (FileStream writer = new FileStream(path + Path.DirectorySeparatorChar + name + ext, FileMode.Create, FileAccess.Write)) {
                                        byte[] arr;
                                        switch (ext) {
                                            case ".jpg":
                                                arr = ((Texture2D)script.transform.Find("Picture").Find("Image").GetComponent<MeshRenderer>().material.mainTexture).EncodeToJPG(100);
                                                writer.Write(arr, 0, arr.Length);
                                                break;
                                            case ".png":
                                                arr = ((Texture2D)script.transform.Find("Picture").Find("Image").GetComponent<MeshRenderer>().material.mainTexture).EncodeToPNG();
                                                writer.Write(arr, 0, arr.Length);
                                                break;
                                            case ".tga":
                                                arr = ((Texture2D)script.transform.Find("Picture").Find("Image").GetComponent<MeshRenderer>().material.mainTexture).EncodeToTGA();
                                                writer.Write(arr, 0, arr.Length);
                                                break;
                                        }
                                    };
                                    editor.transform.Find("InputField").GetComponent<InputField>().enabled = true;
                                    editor.transform.Find("Slider").GetComponent<Slider>().enabled = true;
                                    editor.transform.Find("Delete").GetComponent<Button>().enabled = true;
                                    editor.transform.Find("Disconnect").GetComponent<Button>().enabled = true;
                                    editor.transform.Find("Close").GetComponent<Button>().enabled = true;
                                    editor.transform.Find("LoadImage").GetComponent<Button>().enabled = true;
                                    editor.transform.Find("SaveImage").GetComponent<Button>().enabled = true;
                                    Destroy(dialog);
                                    dialog = null;
                                    Destroy(created);
                                }));
                                dialog.transform.Find("Cancel").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                                    Destroy(dialog);
                                    dialog = null;
                                }));
                            }
                            else {
                                using (FileStream writer = new FileStream(path + Path.DirectorySeparatorChar + name + ext, FileMode.Create, FileAccess.Write)) {
                                    byte[] arr;
                                    switch (ext) {
                                        case ".jpg":
                                            arr = ((Texture2D)script.transform.Find("Picture").Find("Image").GetComponent<MeshRenderer>().material.mainTexture).EncodeToJPG(100);
                                            writer.Write(arr, 0, arr.Length);
                                            break;
                                        case ".png":
                                            arr = ((Texture2D)script.transform.Find("Picture").Find("Image").GetComponent<MeshRenderer>().material.mainTexture).EncodeToPNG();
                                            writer.Write(arr, 0, arr.Length);
                                            break;
                                        case ".tga":
                                            arr = ((Texture2D)script.transform.Find("Picture").Find("Image").GetComponent<MeshRenderer>().material.mainTexture).EncodeToTGA();
                                            writer.Write(arr, 0, arr.Length);
                                            break;
                                    }
                                };
                                editor.transform.Find("InputField").GetComponent<InputField>().enabled = true;
                                editor.transform.Find("Slider").GetComponent<Slider>().enabled = true;
                                editor.transform.Find("Delete").GetComponent<Button>().enabled = true;
                                editor.transform.Find("Disconnect").GetComponent<Button>().enabled = true;
                                editor.transform.Find("Close").GetComponent<Button>().enabled = true;
                                editor.transform.Find("LoadImage").GetComponent<Button>().enabled = true;
                                editor.transform.Find("SaveImage").GetComponent<Button>().enabled = true;
                                Destroy(created);
                            }
                        }));

                        created.GetComponentInChildren<FileDisplay>().fileAction = new System.Action<FileInfo>((inf) => {
                            if (!inf.Exists) return;

                            dialog = Instantiate(Resources.Load<GameObject>("Prefabs/ConfirmDialog"), GameObject.Find("Canvas").transform);
                            dialog.transform.Find("Text").GetComponent<Text>().text = "Are you sure you want to overwrite this file?";
                            dialog.transform.Find("Accept").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                                using (FileStream writer = new FileStream(inf.FullName, FileMode.Truncate, FileAccess.Write)) {
                                    byte[] arr;
                                    switch (inf.Extension) {
                                        case ".jpg":
                                            arr = ((Texture2D)script.transform.Find("Picture").Find("Image").GetComponent<MeshRenderer>().material.mainTexture).EncodeToJPG(100);
                                            writer.Write(arr, 0, arr.Length);
                                            break;
                                        case ".jpeg":
                                            arr = ((Texture2D)script.transform.Find("Picture").Find("Image").GetComponent<MeshRenderer>().material.mainTexture).EncodeToJPG(100);
                                            writer.Write(arr, 0, arr.Length);
                                            break;
                                        case ".png":
                                            arr = ((Texture2D)script.transform.Find("Picture").Find("Image").GetComponent<MeshRenderer>().material.mainTexture).EncodeToPNG();
                                            writer.Write(arr, 0, arr.Length);
                                            break;
                                        case ".tga":
                                            arr = ((Texture2D)script.transform.Find("Picture").Find("Image").GetComponent<MeshRenderer>().material.mainTexture).EncodeToTGA();
                                            writer.Write(arr, 0, arr.Length);
                                            break;
                                    }
                                };
                                editor.transform.Find("InputField").GetComponent<InputField>().enabled = true;
                                editor.transform.Find("Slider").GetComponent<Slider>().enabled = true;
                                editor.transform.Find("Delete").GetComponent<Button>().enabled = true;
                                editor.transform.Find("Disconnect").GetComponent<Button>().enabled = true;
                                editor.transform.Find("Close").GetComponent<Button>().enabled = true;
                                editor.transform.Find("LoadImage").GetComponent<Button>().enabled = true;
                                editor.transform.Find("SaveImage").GetComponent<Button>().enabled = true;
                                Destroy(dialog);
                                Destroy(created);
                            }));
                            dialog.transform.Find("Cancel").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                                Destroy(dialog);
                                dialog = null;
                            }));
                        });
                        created.GetComponentInChildren<FileDisplay>().Display(SettingsManager.lastImagePath, ".jpg", ".jpeg", ".png", ".tga");
                    }));
                    break;
                case PinTypeEnum.Empty:
                    editor = Instantiate(Resources.Load<GameObject>("Prefabs/ConnectorEditor"), GameObject.Find("Canvas").transform);
                    break;
            }

            editor.transform.Find("ApplyColor").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                script.SetColor(PinColor);
            }));
            editor.transform.Find("Disconnect").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                foreach (var item in FindObjectsOfType<LineScript>()) {
                    if (item.pin1 == script || item.pin2 == script) {
                        item.pin1 = null;
                        item.pin2 = null;
                    }
                }
            }));
            editor.transform.Find("Delete").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                Destroy(script.transform.gameObject);
                Destroy(editor);
            }));
            editor.transform.Find("Close").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => {
                Destroy(editor);
            }));
        }
    }

    public void ClearContext() {
        for (int i = GameObject.Find("ContextParent").transform.childCount - 1; i >= 0; i--) {
            DestroyImmediate(GameObject.Find("ContextParent").transform.GetChild(i).gameObject);
        }
        Destroy(editor);
    }
    public GameObject CreateEmpty(Vector3 point, float rot) {
        point.z = -.125f;
        GameObject created = Instantiate(Resources.Load<GameObject>("Prefabs/PinFull"), point, Quaternion.Euler(0, 0, rot), GameObject.Find("BoardItems").transform);
        for (int i = 1; i <= pinlimit; i++) {
            if (!GameObject.Find("Pin" + i.ToString("d4"))) {
                created.name = "Pin" + i.ToString("d4");
                break;
            }
        }
        created.transform.Find("Pin").Find("Pin").GetComponent<MeshRenderer>().material = Instantiate(Resources.Load<Material>("Materials/Colors/PinBasic"));
        created.GetComponent<PinScript>().SetColor(PinColor);
        created.GetComponent<PinScript>().type = PinTypeEnum.Empty;
        return created;
    }
    public GameObject CreateLabel(Vector3 point, float rot) {
        point.z = -.125f;
        GameObject created = Instantiate(Resources.Load<GameObject>("Prefabs/PinFull"), point, Quaternion.Euler(0, 0, rot), GameObject.Find("BoardItems").transform);
        for (int i = 1; i <= pinlimit; i++) {
            if (!GameObject.Find("Pin" + i.ToString("d4"))) {
                created.name = "Pin" + i.ToString("d4");
                break;
            }
        }
        created.transform.Find("Pin").Find("Pin").GetComponent<MeshRenderer>().material = Instantiate(Resources.Load<Material>("Materials/Colors/PinBasic"));
        created.GetComponent<PinScript>().SetColor(PinColor);
        created.GetComponent<PinScript>().type = PinTypeEnum.Label;
        created.transform.Find("Header").gameObject.SetActive(true);
        return created;
    }
    private GameObject CreateLabel2(Vector3 point, float rot) {
        point.z = -.125f;
        GameObject created = Instantiate(Resources.Load<GameObject>("Prefabs/Label2"), point, Quaternion.Euler(0, 0, rot), GameObject.Find("BoardItems").transform);
        for (int i = 1; i <= pinlimit; i++) {
            if (!GameObject.Find("Pin" + i.ToString("d4"))) {
                created.name = "Pin" + i.ToString("d4");
                break;
            }
        }
        created.transform.Find("Pin").Find("Pin1").GetComponent<MeshRenderer>().material = Instantiate(Resources.Load<Material>("Materials/Colors/PinBasic"));
        created.transform.Find("Pin").Find("Pin2").GetComponent<MeshRenderer>().material = Instantiate(Resources.Load<Material>("Materials/Colors/PinBasic"));
        created.GetComponent<PinScript>().SetColor(PinColor);
        created.GetComponent<PinScript>().type = PinTypeEnum.Label;
        created.transform.Find("Header").gameObject.SetActive(true);
        return created;
    }
    public GameObject CreatePicture(Vector3 point, float rot) {
        point.z = -.125f;
        GameObject created = Instantiate(Resources.Load<GameObject>("Prefabs/PinFull"), point, Quaternion.Euler(0, 0, rot), GameObject.Find("BoardItems").transform);
        for (int i = 1; i <= pinlimit; i++) {
            if (!GameObject.Find("Pin" + i.ToString("d4"))) {
                created.name = "Pin" + i.ToString("d4");
                break;
            }
        }
        created.transform.Find("Pin").Find("Pin").GetComponent<MeshRenderer>().material = Instantiate(Resources.Load<Material>("Materials/Colors/PinBasic"));
        created.GetComponent<PinScript>().SetColor(PinColor);
        created.GetComponent<PinScript>().type = PinTypeEnum.Picture;
        created.transform.Find("Picture").Find("Image").GetComponent<MeshRenderer>().material.mainTexture = new Texture2D(0, 0);
        created.transform.Find("Picture").gameObject.SetActive(true);
        return created;
    }
    public GameObject CreateText(Vector3 point, float rot) {
        point.z = -.125f;
        GameObject created = Instantiate(Resources.Load<GameObject>("Prefabs/PinFull"), point, Quaternion.Euler(0, 0, rot), GameObject.Find("BoardItems").transform);
        for (int i = 1; i <= pinlimit; i++) {
            if (!GameObject.Find("Pin" + i.ToString("d4"))) {
                created.name = "Pin" + i.ToString("d4");
                break;
            }
        }
        created.transform.Find("Pin").Find("Pin").GetComponent<MeshRenderer>().material = Instantiate(Resources.Load<Material>("Materials/Colors/PinBasic"));
        created.GetComponent<PinScript>().SetColor(PinColor);
        created.GetComponent<PinScript>().type = PinTypeEnum.Text;
        created.transform.Find("Text").gameObject.SetActive(true);
        return created;
    }
}

public static partial class Extensions
{
    /// <summary>
    /// Write 2 bytes of data to stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="word">2 bytes of value to write.</param>
    public static void WriteWord(this FileStream stream, short word) {
        stream.WriteByte((byte)word);
        stream.WriteByte((byte)(word >> 8));
    }
    /// <summary>
    /// Read 2 bytes of data from stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>short value representing 2 bytes of data</returns>
    public static short ReadWord(this FileStream stream) {
        short w;
        w = (byte)stream.ReadByte();
        w += (short)(stream.ReadByte() << 8);
        return w;
    }
    /// <summary>
    /// Write 4 bytes of data to stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="dword">8 bytes of value to write.</param>
    public static void WriteDWord(this FileStream stream, int dword) {
        for (int i = 0; i < 4; i++) {
            stream.WriteByte((byte)(dword >> (i * 8)));
        }
    }
    /// <summary>
    /// Read 4 bytes of data from stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>long value representing 2 bytes of data</returns>
    public static int ReadDWord(this FileStream stream) {
        long r = 0;
        for (int i = 0; i < 4; i++) {
            int a = stream.ReadByte();
            r += a << (i * 8);
        }
        return (int)r;
    }
    /// <summary>
    /// Write 8 bytes of data to stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="qword">8 bytes of value to write.</param>
    public static void WriteQWord(this FileStream stream, long qword) {
        for (int i = 0; i < 8; i++) {
            stream.WriteByte((byte)(qword >> (i * 8)));
        }
    }
    /// <summary>
    /// Read 8 bytes of data from stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>long value representing 2 bytes of data</returns>
    public static long ReadQWord(this FileStream stream) {
        long r = 0;
        for (int i = 0; i < 8; i++) {
            r += (long)stream.ReadByte() << (i * 8);
        }
        return r;
    }
}
