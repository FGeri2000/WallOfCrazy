using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class FileUnit : MonoBehaviour
{
    public FileSystemInfo fs;

    public void UpdateText()
    {
        foreach (Image item in GetComponentsInChildren<Image>()) {
            item.enabled = true;
        }
        foreach (Text item in GetComponentsInChildren<Text>()) {
            item.enabled = true;
        }

        transform.Find("Name").GetComponent<Text>().text = fs.Name;
        transform.Find("LastModified").GetComponent<Text>().text = string.Format("{0} {1}", fs.LastWriteTime.ToShortDateString(), fs.LastWriteTime.ToShortTimeString());

        if (fs.Attributes.HasFlag(FileAttributes.Directory))
        {
            transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Materials/directory");
        }
        else
        {
            transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Materials/file");
        }
    }
}
