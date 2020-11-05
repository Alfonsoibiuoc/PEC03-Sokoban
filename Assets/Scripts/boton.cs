using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class boton : MonoBehaviour
{
    public void click() {
        //Se guarda el sprite en seleccion
        Editor.seleccion = GetComponent<Image>().sprite;
    
    }
}
