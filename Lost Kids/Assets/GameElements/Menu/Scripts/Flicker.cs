using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Flicker : MonoBehaviour {

	//Agregamos las variables de Texto y Strings
	Text textoParpadeante;
	string textoQueParpadea;
	string textoEnBlanco;

	//Agregamos una bandera que sera nuestro identificador para cambiar el texto
	bool estaParpadeando = true;

	void Start () 
	{
		//obtenemos el componente del texto
		textoParpadeante = GetComponent<Text>();

        // Inicializa los textos
        textoQueParpadea = textoParpadeante.text;
        textoEnBlanco = "";

        //llamamos al coroutine de la funcion de TextoParpadeo
        StartCoroutine(TextoParpadeo());

	}

	//funcion para que parpadee el texto
	public IEnumerator TextoParpadeo()
	{
		//el parpadeo sera infinito hasta que termine la condicion dependiendo tu eleccion
		while(estaParpadeando)
		{
			//Establecemos nuestro texto en blanco
			textoParpadeante.text = textoEnBlanco;

			//mostramos el texto en blanco por 0.5 segundos
			yield return new WaitForSeconds(.5f);

			//mostramos nuestro texto en mi caso Depredador1220 por otros 0.5 segundos
			textoParpadeante.text = textoQueParpadea;
			yield return new WaitForSeconds(.5f);
		}
	}
		
} 