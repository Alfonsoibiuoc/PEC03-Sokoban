using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Controlador : MonoBehaviour
{
    //Variables de control
    bool completado = false;
    int numeroNivel = 0;
    string nivel;
    int[,] datos;
    int numeroFilas;
    int numeroColumnas;
    Vector2 centroEscena;
    public float escalaCelda;
    public int contadorCajas;
    int ValorFuera = 0;
    int valorPosicionFinal = 2;
    int ValorCaja = 3;
    int ValorPlayer = 4;
    bool mover = true;

    //Objetos
    GameObject DatosJuego;
    public GameObject panelCompletado;
    public Sprite SpriteSuelo;
    public Sprite SpriteCaja;
    public Sprite SpritePosicionFinal;
    public Sprite SpritePlayer;
    GameObject Caja;
    Vector3 posicionFinal;
    Vector3 posicionFinalCaja;
    Transform cajaMover = null;
    List<Cajas> cajas = new List<Cajas>();
    List<Cajas> finales = new List<Cajas>();

    //UI
    public Text textoNivel;
    int mostrarNivel;

    //Jugador
    GameObject Jugador;
    int posXJugador;
    int posYJugador;
    public float velocidad;

    private void Awake()
    {
        //Se obtiene el objeto DATA dónde se almacena el nivel actual y la canidad de niveles
        DatosJuego = GameObject.Find("DATA");
        numeroNivel = DatosJuego.GetComponent<Datos>().nivelActual;
    }

    void Start()
    {
        nivel = numeroNivel.ToString();
        //Carga los datos del fichero de texto del nivel correspondiente
        obtenerDatosNivel();
        //Muestra el nivel actual al jugador
        mostrarNivel = numeroNivel + 1;
        textoNivel.text = "Nivel:" + mostrarNivel;
    }

    void obtenerDatosNivel()
    {
        //Lee el archivo
        string path = Application.persistentDataPath + "/niveles/"+nivel+".txt";
        StreamReader reader = new StreamReader(path);
        string contenido = reader.ReadToEnd();
        TextAsset archivoTexto = new TextAsset(contenido);
        reader.Close();
        //Divide el archivo por líneas y luego por número
        string[] lineas = archivoTexto.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] numeros = lineas[0].Split(new[] { ',' });
        numeroFilas = lineas.Length;
        numeroColumnas = numeros.Length;
        //Crea la matriz
        datos = new int[numeroFilas, numeroColumnas];
        //Rellena la matriz
        for (int i = 0; i < numeroFilas; i++)
        {
            string valoresFila = lineas[i];
            numeros = valoresFila.Split(new[] { ',' });
            for (int j = 0; j < numeroColumnas; j++)
            {
                int valor;
                if (int.TryParse(numeros[j], out valor))
                {
                    datos[i, j] = valor;  
                }
            }
        }
        //Dibuja el nivel en la pantalla
        dibujarNivel();
    }

    void dibujarNivel()
    {
        //Busca la posición central de la escena
        centroEscena.x = numeroColumnas * escalaCelda * 0.5f - escalaCelda * 0.5f;
        centroEscena.y = numeroFilas * escalaCelda * 0.5f - escalaCelda * 0.5f;
        GameObject casillaSuelo;
        SpriteRenderer SR;
        //Recorre el array
        for (int i = 0; i < numeroFilas; i++)
        {
            for (int j = 0; j < numeroColumnas; j++)
            {
                int valor = datos[i, j];
                if (valor != ValorFuera) {
                    //Si el valor de la celda no es una casilla vacía (No es 0), creo un nuevo objeto
                    casillaSuelo = new GameObject("CasillaSuelo_" + i.ToString() + "_" + j.ToString());
                    casillaSuelo.transform.localScale = Vector2.one * (escalaCelda - 1);
                    //Le añade el componente sprite
                    SR = casillaSuelo.AddComponent<SpriteRenderer>();
                    SR.sprite = SpriteSuelo;
                    casillaSuelo.transform.position = obtenerOrigen(i, j);
                    if (valor == valorPosicionFinal)
                    {
                        //Si es una casilla en la que hay que colocar una caja (valor 3) le añade el sprite correspondiente
                        SR.sprite = SpritePosicionFinal;
                        //Añade la casilla final a un array de la clase "cajas" para saber dónde está.
                        finales.Add(new Cajas(i, j));
                    } else {
                        //Si no se cumple lo anterior, es un jugador o una caja
                        if (valor == ValorPlayer) {
                            //Si es un jugador, lo crea y le asigna el sprite correspondiente
                            Jugador = new GameObject("Jugador");
                            Jugador.transform.localScale = Vector2.one * (escalaCelda - 1);
                            SR = Jugador.AddComponent<SpriteRenderer>();
                            SR.sprite = SpritePlayer;
                            SR.sortingOrder = 1; 
                            Jugador.transform.position = obtenerOrigen(i, j);
                            //Guarda la posicin actual
                            posXJugador = i;
                            posYJugador = j;
                            posicionFinal = Jugador.transform.position;
                        } else if (valor == ValorCaja) {
                            //Si es una caja
                            contadorCajas++;
                            Caja = new GameObject("Caja_" + contadorCajas.ToString());
                            Caja.transform.localScale = Vector2.one * (escalaCelda - 1);
                            //Se le añade unb componente en el que se almacena su posicion en cada momento.
                            Caja.AddComponent<Cajas>();
                            Caja.GetComponent<Cajas>().I = i;
                            Caja.GetComponent<Cajas>().J = j;
                            //Se le añade el sprite correspondiente
                            SR = Caja.AddComponent<SpriteRenderer>();
                            SR.sprite = SpriteCaja;
                            SR.sortingOrder = 1;
                            Caja.transform.position = obtenerOrigen(i, j);
                            cajas.Add(Caja.GetComponent<Cajas>());    
                        }
                    }
                }
            }
        }  
    }

    Vector2 obtenerOrigen(int fila, int columna){
        return new Vector2(columna * escalaCelda - centroEscena.x, fila * -escalaCelda + centroEscena.y);  
    }

    private void Update()
    {
        //Si el nivel no está completado, captura el input del jugador.
        if (!completado) {
            InputJugador();
        }
        //Mueve al jugador a la posición final usando la funcion Lerp, de modo que el movimieto se suaviza.
        Jugador.transform.position = Vector3.Lerp(Jugador.transform.position, posicionFinal, velocidad * Time.deltaTime);
        //Suaviza del mismo modo el movimiento de la caja
        if (cajaMover != null){   
                cajaMover.position = Vector3.Lerp(cajaMover.transform.position, posicionFinalCaja, velocidad * Time.deltaTime);
        }
        //Permite mover de nuevo al personaje cuando alcanza la posicion final
        if (Vector3.Distance(Jugador.transform.position, posicionFinal) < 0.1f) {
            mover = true;
        }
    }

    //A continuación se controlan los movimientos del jugador
    void InputJugador() {
        //Movimiento hacia arriba
        if (Input.GetKeyDown("up") && mover == true) {
            Jugador.transform.localRotation = Quaternion.Euler(0, 0, 0);
            //Se comprueba la casilla a la que se dirige el jugador
            if (datos[posXJugador - 1, posYJugador] == 1 || datos[posXJugador - 1, posYJugador] == 2) {
                //Si la casilla es de suelo o una casilla final de una caja
                //Cambia el valor de la casilla dónde estaba el jugador
                datos[posXJugador, posYJugador] = 1;
                mover = false;
                posXJugador--;
                posicionFinal = obtenerOrigen(posXJugador, posYJugador);          
            } else if (datos[posXJugador - 1, posYJugador] == 3 && (datos[posXJugador - 2, posYJugador] == 1 || datos[posXJugador - 2, posYJugador] == 2) )
                //Comrpueba si en la casilla a la que se va a mover hay una caja y tras esta un espacio al que se puede mover la caja
            {
                foreach (Cajas c in cajas)
                {
                    //Comprueba si todas las cajas están en la posición correcta
                    if (posXJugador - 1 == c.I && posYJugador == c.J) {
                        c.I--;
                        posXJugador--;
                        //Carga la posicin final de la caja
                        posicionFinalCaja = obtenerOrigen(posXJugador -1, posYJugador);
                        cajaMover = c.transform;
                        //carga la posicion final del jugador
                        posicionFinal = obtenerOrigen(posXJugador, posYJugador);
                        datos[posXJugador +1, posYJugador] = 1;
                        datos[posXJugador -1, posYJugador] = 3;
                        //Todas las cajas están en su sitio
                        Completado();
                    }
                }
            }              
        }
        //Movimiento hacia abajo
        if (Input.GetKeyDown("down") && mover == true){
            Jugador.transform.localRotation = Quaternion.Euler(0, 0, 180);
            if (datos[posXJugador + 1, posYJugador] == 1 || datos[posXJugador + 1, posYJugador] == 2){
                datos[posXJugador, posYJugador] = 1;
                mover = false;
                posXJugador++;
                posicionFinal = obtenerOrigen(posXJugador, posYJugador);             
            }
            else if (datos[posXJugador + 1, posYJugador] == 3 && (datos[posXJugador + 2, posYJugador] == 1 || datos[posXJugador + 2, posYJugador] == 2) )
            {
                foreach (Cajas c in cajas)
                {
                    if (posXJugador + 1 == c.I && posYJugador == c.J)
                    {
                        c.I++;
                        posXJugador++;
                        posicionFinalCaja = obtenerOrigen(posXJugador + 1, posYJugador);
                        cajaMover = c.transform;
                        posicionFinal = obtenerOrigen(posXJugador, posYJugador);
                        datos[posXJugador -1, posYJugador] = 1;
                        datos[posXJugador + 1, posYJugador] = 3;
                        Completado();
                    }
                }
            }
        }
        //Movimiento a la derecha
        if (Input.GetKeyDown("right") && mover == true){
            Jugador.transform.localRotation = Quaternion.Euler(0, 0, 270);
            if (datos[posXJugador, posYJugador + 1] == 1 || datos[posXJugador, posYJugador + 1] == 2){
                datos[posXJugador, posYJugador] = 1;
                mover = false;
                posYJugador++;
                posicionFinal = obtenerOrigen(posXJugador, posYJugador);               
            }
            else if (datos[posXJugador, posYJugador + 1] == 3 && datos[posXJugador, posYJugador + 2] == 1 || datos[posXJugador, posYJugador + 2] == 2)
            {
                foreach (Cajas c in cajas)
                {
                    if (posXJugador == c.I && posYJugador + 1 == c.J)
                    {
                        c.J++;
                        posYJugador++;
                        posicionFinalCaja = obtenerOrigen(posXJugador, posYJugador + 1);
                        cajaMover = c.transform;
                        posicionFinal = obtenerOrigen(posXJugador, posYJugador);
                        datos[posXJugador, posYJugador - 1] = 1;
                        datos[posXJugador, posYJugador + 1] = 3;
                        Completado();
                    }
                }
            }
        }
        //Movimiento a la izquierda
        if (Input.GetKeyDown("left") && mover == true){
            Jugador.transform.localRotation = Quaternion.Euler(0, 0, 90);
            if (datos[posXJugador, posYJugador - 1] == 1 || datos[posXJugador, posYJugador - 1] == 2){
                datos[posXJugador, posYJugador] = 1;
                mover = false;
                posYJugador--;
                posicionFinal = obtenerOrigen(posXJugador, posYJugador);
            }
            else if (datos[posXJugador, posYJugador - 1] == 3 && datos[posXJugador, posYJugador - 2] == 1 || datos[posXJugador, posYJugador - 2] == 2)
            {
                foreach (Cajas c in cajas)
                {
                    if (posXJugador == c.I && posYJugador - 1 == c.J)
                    {
                        c.J--;
                        posYJugador--;
                        posicionFinalCaja = obtenerOrigen(posXJugador, posYJugador - 1);
                        cajaMover = c.transform;
                        posicionFinal = obtenerOrigen(posXJugador, posYJugador);
                        datos[posXJugador, posYJugador + 1] = 1;
                        datos[posXJugador, posYJugador - 1] = 3;
                        Completado();
                    }
                }
            }
        }
    }

    //Compara el array de las posiciones finales de las cajas con el de las posiciones de las cajas
    bool comprobarCaja(Cajas c) {
        foreach (Cajas f in finales) {
            if (c.I == f.I && c.J == f.J) {
                return true;
            }
        }
        return false;
    }
    void Completado()
    {
        completado = true;
        foreach (Cajas s in cajas)
        {
            if (!comprobarCaja(s))
            {
                completado = false;
            }
        }
        if (completado) {
            //Lanza los eventos al completar el nivel
            StartCoroutine(NivelCompletado());     
        } 
    }

    private IEnumerator NivelCompletado()
    {
        yield return new WaitForSeconds(1);
        panelCompletado.SetActive(true);
        yield return new WaitForSeconds(2);
        //Comprueba si es el último nivel del juego.
        if (DatosJuego.GetComponent<Datos>().nivelActual == DatosJuego.GetComponent<Datos>().cantidadNiveles -1)
        {
            //Es el último
            SceneManager.LoadScene("Final");    
        }
        else
        {
            //No es el último
            //Modifica la información de nivelActual en el txt.
            File.Delete(Application.persistentDataPath + "/nivelActual.txt");
            DatosJuego.GetComponent<Datos>().nivelActual = numeroNivel + 1;
            StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/nivelActual.txt", true);
            writer.Write(DatosJuego.GetComponent<Datos>().nivelActual);
            writer.Close();
            //Carga el siguiente nivel
            SceneManager.LoadScene("Nivel");
        }
    }

    public void reiniciar() 
    {
        //Carga el nivel de nuevo
        SceneManager.LoadScene("Nivel");
    
    }
    public void salir()
    {
        //Sale al menu principal.
        SceneManager.LoadScene("Menu");
    }


}


