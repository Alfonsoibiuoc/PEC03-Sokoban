using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlFinal : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Final());
    }
    //Carga la escena final y tras unos segundos vuelve al inicio
    private IEnumerator Final()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("Menu");
        }    
}
