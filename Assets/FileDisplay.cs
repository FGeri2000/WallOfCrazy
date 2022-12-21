using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class FileDisplay : MonoBehaviour
{
    public System.Action<FileInfo> fileAction;
    public System.Action cancel;

    private void Start() {
        transform.parent.parent.parent.parent.Find("Path").GetComponent<InputField>().onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>((s) => {
            if (Directory.Exists(s)) {
                previous = current;
                current = new DirectoryInfo(s);
                parent = Directory.GetParent(current.FullName) == null ? current : Directory.GetParent(current.FullName);
                Display();
            }
        }));
    }
    public void Cancel() {
        cancel();
        Destroy(transform.parent.parent.parent.parent.gameObject);
    }

    DirectoryInfo current;
    DirectoryInfo previous;
    DirectoryInfo parent;

    string[] extensions;

    public void Display(DirectoryInfo dir, params string[] extensions) {
        current = dir;
        previous = dir;
        parent = Directory.GetParent(dir.FullName);
        this.extensions = extensions;
        Display();
    }
    public void Display(params string[] extensions) {
        this.extensions = extensions;
        Display();
    }

    public void Up() {
        if (Directory.GetParent(current.FullName) == null) return;
        previous = new DirectoryInfo(current.FullName);
        current = parent;
        parent = Directory.GetParent(current.FullName) == null ? current : Directory.GetParent(current.FullName);
        Display();
    }
    public void Back() {
        DirectoryInfo t = previous;
        previous = new DirectoryInfo(current.FullName);
        current = t;
        parent = Directory.GetParent(current.FullName);
        Display();
    }

    private void Display() {
        System.Collections.Generic.List<string> ext = new System.Collections.Generic.List<string>();
        ext.AddRange(extensions);
        int prevchildcount = transform.childCount;
        for (int i = transform.childCount - 1; i >= 0; i--) {
            Destroy(transform.GetChild(i).gameObject);
        }

        transform.parent.parent.parent.parent.Find("Path").GetComponent<InputField>().text = current.FullName;

        foreach (var item in current.EnumerateDirectories()) {
            GameObject temp = Instantiate(Resources.Load("Prefabs/FileUnitPrefab") as GameObject, transform);
            temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            temp.GetComponent<FileUnit>().fs = item;

            temp.GetComponent<FileUnit>().UpdateText();

            temp.GetComponent<Button>().onClick.AddListener(() => {
                previous = current;
                current = (DirectoryInfo)temp.GetComponent<FileUnit>().fs;
                parent = Directory.GetParent(current.FullName);
                Display();
            });
        }
        foreach (var item in current.EnumerateFiles()) {
            if (ext.Contains(item.Extension.ToLower()) || ext.Count == 0) {
                GameObject temp = Instantiate(Resources.Load("Prefabs/FileUnitPrefab") as GameObject, transform);
                temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                temp.GetComponent<FileUnit>().fs = item;

                temp.GetComponent<FileUnit>().UpdateText();

                temp.GetComponent<Button>().onClick.AddListener(() => {
                    fileAction(item);
                });
            }
        }

        int offset = -10;

        for (int i = prevchildcount; i < transform.childCount; i++) {
            //print(transform.GetChild(i).GetComponent<FileUnit>().fs + " " + offset);
            transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, offset);
            offset -= 20;
        }

        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, 20 * (transform.childCount - prevchildcount));
    }
}
