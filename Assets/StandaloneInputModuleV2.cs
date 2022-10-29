using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StandaloneInputModuleV2 : StandaloneInputModule
{
    public GameObject GameObjectUnderPointer(int pointerId)
    {
        var lastPointer = GetLastPointerEventData(pointerId);
        if (lastPointer != null) {
            return lastPointer.pointerCurrentRaycast.gameObject;
        }

        return null;
    }

    public GameObject GameObjectUnderPointer()
    {
        return GameObjectUnderPointer(kMouseLeftId);
    }
    public static StandaloneInputModuleV2 Singleton { get { return GameObject.Find("EventSystem").GetComponent<StandaloneInputModuleV2>(); } }
}
