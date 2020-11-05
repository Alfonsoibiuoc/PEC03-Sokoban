using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SFB;



public class Editor : MonoBehaviour
{
    public GameObject sprite;
    public Transform canvas;
    public static Sprite seleccion;
    int anchoCelda = 40;
    int ancho = 14;
    int alto = 12;
    int celdaFinal;
    int numArchivo;
    public GameObject PanelSeleccion;
    public GameObject PanelTablero;
    public GameObject PanelGuardar;
    public GameObject PanelOK;
    public GameObject PanelImportar;
    string output;

    Vector2 devolverPosicion(int x, int y) {
        return new Vector2((x* anchoCelda) + 225,(y* anchoCelda));
    }
    private void Start()
    {
        Debug.Log(Application.persistentDataPath);
        //Se crea el tablero sobre el que se dibuja el nivel
        seleccion = sprite.GetComponent<Image>().sprite;
        numArchivo = Directory.GetFiles(Application.persistentDataPath + "/Niveles/").Length;
        for (int y = alto; y > 0; y--) {
            for (int x = 0; x < ancho; x++)
            {
                GameObject ns = Instantiate(sprite, canvas);
                ns.GetComponent<RectTransform>().position = devolverPosicion(x, y);
            }
        }
        celdaFinal = ancho -1;
    }
    public void Guardar()
    {
        int count = 0;
        string mapa = "";
        //Primero se crea el string con los datos del nivel
        foreach (Image s in canvas.GetComponentsInChildren<Image>()) {
            //Debug.Log(s.sprite.ToString());
            switch (s.sprite.ToString()) {
                case "SokobanClone_byVellidragon_22 (UnityEngine.Sprite)":
                    //Debug.Log("Cara");
                    if (count == celdaFinal){
                        mapa += "4\n";
                        count = 0;
                    }else {
                        mapa += "4,";
                        count++;
                    } 
                    break;
                case "SokobanClone_byVellidragon_0 (UnityEngine.Sprite)":
                    //Debug.Log("caja");
                    if (count == celdaFinal){
                        mapa += "3\n";
                        count = 0;
                    }else{
                        mapa += "3,";
                        count++;
                    }
                    break;
                case "SokobanClone_byVellidragon_8 (UnityEngine.Sprite)":
                    //Debug.Log("suelo");
                    if (count == celdaFinal){
                        mapa += "1\n";
                        count = 0;
                    }else{
                        mapa += "1,";
                        count++;
                    }
                    break;
                case "SokobanClone_byVellidragon_9 (UnityEngine.Sprite)":
                    //Debug.Log("final");
                    if (count == celdaFinal){
                        mapa += "2\n";
                        count = 0;
                    }else{
                        mapa += "2,";
                        count++;
                    }
                    break;
                default:
                    //Debug.Log("vacio");
                    if (count == celdaFinal)
                    {
                        mapa += "0\n";
                        count = 0;
                    }else{
                        mapa += "0,";
                        count++;
                    }
                    break;
            }
        }
        //Se crea el fichero .txt en la carpeta niveles
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/niveles/"+numArchivo+".txt", true);
        writer.Write(mapa);
        writer.Close();
        numArchivo = Directory.GetFiles(Application.persistentDataPath + "/Niveles/").Length;
    }

    public void botonSalir() {
        SceneManager.LoadScene("Menu");
    }

    public void botonGuardar() {
        PanelSeleccion.SetActive(false);
        PanelTablero.SetActive(false);
        PanelGuardar.SetActive(true);  
    }

    public void noGuardar() {
        PanelGuardar.SetActive(false);
        PanelSeleccion.SetActive(true);
        PanelTablero.SetActive(true);
    }

    public IEnumerator procesoGuardar() {
        Guardar();
        PanelGuardar.SetActive(false);
        yield return new WaitForSeconds(1);
        PanelOK.SetActive(true);
        yield return new WaitForSeconds(1);
        PanelOK.SetActive(false);
        yield return new WaitForSeconds(1);
        PanelSeleccion.SetActive(true);
        PanelTablero.SetActive(true);
    }

    public void proceso() {
        StartCoroutine(procesoGuardar());
    }
    
    //Abre una ventana del navegador que permite importar un nivel desde un fichero de texto y añadirlo directamente como un nivel más del juego.
    public void Upload() {
        PanelImportar.SetActive(false);
        PanelSeleccion.SetActive(true);
        PanelTablero.SetActive(true);
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "txt", false);
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }

    private IEnumerator OutputRoutine(string url){
        var loader = new WWW(url);
        yield return loader;
        output = loader.text;
        yield return new WaitForSeconds(1);
        StreamWriter writerImport = new StreamWriter(Application.persistentDataPath + "/niveles/" + numArchivo + ".txt", true);
        writerImport.Write(output);
        writerImport.Close();
        numArchivo = Directory.GetFiles(Application.persistentDataPath + "/Niveles/").Length;   
    }

    public void abrirPanelUpload() {
        PanelSeleccion.SetActive(false);
        PanelTablero.SetActive(false);
        PanelImportar.SetActive(true);
    }

    public void cerrarPanelUpload() {
        PanelImportar.SetActive(false);
        PanelSeleccion.SetActive(true);
        PanelTablero.SetActive(true);
    }

    //Exportar el ivel en formato .txt
    public void exportar() {
        int count = 0;
        string mapa = "";
        //Primero se crea el string con los datos del nivel
        foreach (Image s in canvas.GetComponentsInChildren<Image>())
        {
            //Debug.Log(s.sprite.ToString());
            switch (s.sprite.ToString())
            {
                case "SokobanClone_byVellidragon_22 (UnityEngine.Sprite)":
                    //Debug.Log("Cara");
                    if (count == celdaFinal)
                    {
                        mapa += "4\n";
                        count = 0;
                    }
                    else
                    {
                        mapa += "4,";
                        count++;
                    }
                    break;
                case "SokobanClone_byVellidragon_0 (UnityEngine.Sprite)":
                    //Debug.Log("caja");
                    if (count == celdaFinal)
                    {
                        mapa += "3\n";
                        count = 0;
                    }
                    else
                    {
                        mapa += "3,";
                        count++;
                    }
                    break;
                case "SokobanClone_byVellidragon_8 (UnityEngine.Sprite)":
                    //Debug.Log("suelo");
                    if (count == celdaFinal)
                    {
                        mapa += "1\n";
                        count = 0;
                    }
                    else
                    {
                        mapa += "1,";
                        count++;
                    }
                    break;
                case "SokobanClone_byVellidragon_9 (UnityEngine.Sprite)":
                    //Debug.Log("final");
                    if (count == celdaFinal)
                    {
                        mapa += "2\n";
                        count = 0;
                    }
                    else
                    {
                        mapa += "2,";
                        count++;
                    }
                    break;
                default:
                    //Debug.Log("vacio");
                    if (count == celdaFinal)
                    {
                        mapa += "0\n";
                        count = 0;
                    }
                    else
                    {
                        mapa += "0,";
                        count++;
                    }
                    break;
            }
        }
        var path = StandaloneFileBrowser.SaveFilePanel("Title", "", "Nivel_Sokoalf", "txt");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, mapa);
        }
        PanelSeleccion.SetActive(false);
        PanelTablero.SetActive(false);
        PanelGuardar.SetActive(true);
    }
    
}
