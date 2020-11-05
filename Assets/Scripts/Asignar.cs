using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Asignar : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Se asigna el Sprite guardado en seleccion al objeto que toca
        GetComponent<Image>().sprite = Editor.seleccion;
    }
}
