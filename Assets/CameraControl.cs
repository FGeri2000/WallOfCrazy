using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Extensions
{
    public static void Set(this CharacterController cont, Vector3 pos)
    {
        cont.Move(pos - cont.transform.position);
    }
}

public class CameraControl : MonoBehaviour
{
    private CharacterController _controller;

    public Vector3 SpeedScalar = new Vector3(3f, 3f, 1f);
    public bool Freeze;
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        if (SettingsManager._2dmode)
        {
            GetComponent<Camera>().orthographic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SettingsManager._2dmode)
        {
            GetComponent<Camera>().orthographic = true;
        }
        else
        {
            GetComponent<Camera>().orthographic = false;
        }

        if (!Freeze)
        {
            foreach (var item in FindObjectsOfType<UnityEngine.UI.InputField>())
            {
                if (item.isFocused)
                {
                    return;
                }
            }
            foreach (var item in FindObjectsOfType<UnityEngine.UI.Dropdown>())
            {

            }
            if (FindObjectOfType<BoardItemController>().DialogPresent)
                return;

            _controller.Move(new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * SpeedScalar.x * (transform.position.z / -10), Input.GetAxis("Vertical") * Time.deltaTime * SpeedScalar.y * (transform.position.z / -10), Input.mouseScrollDelta.y * SpeedScalar.z));
            Vector3 pos = transform.localPosition;
            if (SettingsManager._2dmode)
            {
                GameObject.Find("Camera").GetComponent<Camera>().orthographicSize = -transform.position.z;
            }
        }
    }
}
