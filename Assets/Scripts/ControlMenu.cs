using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlMenu : MonoBehaviour
{
    public GameObject BotonEditor;
    public GameObject BotonJugar;
    public GameObject BotonSalir;
    public GameObject PanelJugar;
    public GameObject PanelNoNiveles;
    public GameObject DATA;

    public void Jugar() {
        //Comprueba que existen niveles en el juego.
        if (DATA.GetComponent<Datos>().cantidadNiveles == 0)
        {
            //Si no hay
            StartCoroutine(noHayNiveles());
        }
        else
        {
            //Si hay
            if (DATA.GetComponent<Datos>().nivelActual == 0)
            {
                //Si el nivel actual del jugador es cero, carga directamente el nivel.
                SceneManager.LoadScene("Nivel");
            }
            else
            {
                //Si el nivel del jugador es distinto de cero, propone continuar la partida o empezar una nueva.
                BotonEditor.SetActive(false);
                BotonJugar.SetActive(false);
                BotonSalir.SetActive(false);
                PanelJugar.SetActive(true);
            }
        }
    }
    public void BotonContinuar() {
        //Carga la escena del juego desde el nivel actual
        SceneManager.LoadScene("Nivel");
    }
    public void BotonNuevaPartida() {
        //Cambia el nivel actual a 0 y carga el primer nivel        
        File.Delete(Application.persistentDataPath + "/nivelActual.txt");
        DATA.GetComponent<Datos>().nivelActual = 0;
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/nivelActual.txt", true);
        writer.Write(DATA.GetComponent<Datos>().nivelActual);
        writer.Close();
        SceneManager.LoadScene("Nivel");
    }
    public void BotonCancelar() {
        //Vuelve al menu de inicio
        PanelJugar.SetActive(false);
        BotonEditor.SetActive(true);
        BotonJugar.SetActive(true);
        BotonSalir.SetActive(true);
        
    }

    public void Editor() {
        //Carga la escena del editor
        SceneManager.LoadScene("Editor");
    }

    public void Salir() {
        //Sale de la aplicación
        Application.Quit();
    }

    public IEnumerator noHayNiveles()
    {
        BotonEditor.SetActive(false);
        BotonJugar.SetActive(false);
        BotonSalir.SetActive(false);
        yield return new WaitForSeconds(1);
        PanelNoNiveles.SetActive(true);
        yield return new WaitForSeconds(2);
        PanelNoNiveles.SetActive(false);
        yield return new WaitForSeconds(1);
        BotonEditor.SetActive(true);
        BotonJugar.SetActive(true);
        BotonSalir.SetActive(true);
    }
        
        
}
