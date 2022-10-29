using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StaticCallbackController : MonoBehaviour
{
    void Start()
    {
        GameObject.Find("MenuButton").GetComponent<Toggle>().onValueChanged.AddListener(SettingsManager.MenuOpen);
        GameObject.Find("ShadowsToggle").GetComponent<Toggle>().onValueChanged.AddListener(SettingsManager.ShadowToggle);
        GameObject.Find("FullscreenToggle").GetComponent<Toggle>().onValueChanged.AddListener(SettingsManager.FullscreenToggle);
        GameObject.Find("2DModeToggle").GetComponent<Toggle>().onValueChanged.AddListener(SettingsManager._2DModeToggle);
        GameObject.Find("ResizeSetButton").GetComponent<Button>().onClick.AddListener(BoardSizeController.ResizeSetCallback);
        GameObject.Find("ImageLimitToggle").GetComponent<Toggle>().onValueChanged.AddListener(SettingsManager.ImgLimitToggle);
        GameObject.Find("LineWidthSlider").GetComponent<Slider>().onValueChanged.AddListener(BoardItemController.Singleton.SetLineWidth);
        GameObject.Find("QuitButton").GetComponent<Button>().onClick.AddListener(BoardItemController.Singleton.Quit);

        SettingsManager.Start();
    }

    void Update()
    {
        BoardSizeController.Update();
    }
}
