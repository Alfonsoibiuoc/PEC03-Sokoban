using System.IO;
using UnityEngine;

public class Datos : MonoBehaviour
{
    public int cantidadNiveles;
    public int nivelActual;
    private void Awake()
    {
        //Se crea un directorio en el PC dónde se almacenarán los niveles.
        Directory.CreateDirectory(Application.persistentDataPath + "/niveles");
        //Numero de niveles creados en la carpeta Resources
        cantidadNiveles = 10;
        //Comprueba si existen archivos del juego
        ComprobarArchivo();
        //Carga la cantidad de niveles del juego
        cantidadNiveles = Directory.GetFiles(Application.persistentDataPath + "/niveles").Length;
        //Se mantiene este objeto durante las demás escenas.
        DontDestroyOnLoad(gameObject);
    }
    public void ComprobarArchivo()
    {
        //Carga el archivo nivelActual.txt para ver si realmente existe
        string filePath = Application.persistentDataPath + "/nivelActual.txt";
        if (System.IO.File.Exists(filePath)){
            //Si el archivo existe se lee el contenido para saber el nivel actual del jugador.
            string path = Application.persistentDataPath + "/nivelActual.txt";
            StreamReader reader = new StreamReader(path);
            string contenido = reader.ReadToEnd();
            nivelActual = int.Parse(contenido);
            reader.Close();
        }else{
            //Si no existe el archivo nivelActual.txt, se copian los niveles de la carpeta resources a la carpeta del PC
            for (int i = 0; i < cantidadNiveles; i++)
            {  
                TextAsset temporal = (TextAsset)Resources.Load(i.ToString());
                StreamWriter writerLevel = new StreamWriter(Application.persistentDataPath +"/niveles/"+i+".txt", true);
                writerLevel.Write(temporal.text);
                writerLevel.Close();
            }
            TextAsset nivelA = (TextAsset)Resources.Load("nivelActual");
            StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/nivelActual.txt", true);
            writer.Write(nivelA.text);
            writer.Close();   
        }
    }
}
