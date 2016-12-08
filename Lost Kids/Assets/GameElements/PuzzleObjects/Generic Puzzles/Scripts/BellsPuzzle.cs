using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Script controlador del puzzle sonoro
/// El jugador debe repetir una secuencia activando las diferentes campanas
/// que componen el puzzle
/// </summary>
public class BellsPuzzle : PuzzleManagerBase,IActivable {

    //Flag para generar una secuencia aleatoria
    public bool randomSequence;

    //Longitud de la secuencia aleatoria
    public int randomSeqLength = 1;

    //Secuencia de sonidos
    public GameObject[] sequence;
    private Bell[] seq;

    //Paso de la secuencia en la que se encuentra el puzzle
    private int sequenceStep = 0;

    //Objeto iniciador de la reproduccion de la secuencia
    private UsableObject initiator;

    private bool playingSequence=false;

    void Awake()
    {
        initiator = transform.Find("Initiator").GetComponentInChildren<UsableObject>();
    }

    // Use this for initialization
    void Start () {
        base.Start();

        //Generacion de secuencia aleatoria
        if(randomSequence)
        {
            sequence = new GameObject[randomSeqLength];
            for(int i=0;i<randomSeqLength;i++)
            {
                sequence[i] = objectList[UnityEngine.Random.Range(0, objectList.Count)];
            }
        }

        //Referencias de los scripts de la secuencia
        seq = new Bell[sequence.Length];
        for(int i=0;i<sequence.Length;i++)
        {
            seq[i] = sequence[i].GetComponent<Bell>();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Reproduce paso a paso la secuencia correcta del puzzle
    /// </summary>
    /// <returns></returns>
    IEnumerator PlaySequence()
    {
        playingSequence = true;
        for(int i=0;i<seq.Length;i++)
        {
            seq[i].PlaySound();
            yield return new WaitForSeconds(1f);
        }
        playingSequence = false;
       yield return 0;
    }

    /// <summary>
    /// La activacion del puzzle implica la reproduccion de la secuencia completa
    /// y la vuelta al primer paso
    /// </summary>
    public void Activate()
    {
        if (!solved)
        {
            StartCoroutine(PlaySequence());
            sequenceStep = 0;
        }
    }

    /// <summary>
    /// Reseteo completo del puzzle
    /// </summary>
    public void CancelActivation()
    {
        Reset();
    }

    /// <summary>
    /// Notifica la activacion de una de las campanas
    /// Comprueba que sea la campana corrspondiente al paso en la secuencia, o resetea el puzzle
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="status"></param>
    public override void NotifyChange(UsableObject sender, bool status)
    {
        if (!playingSequence)
        {

            if (!solved)
            {
                if (status && seq[sequenceStep].Equals(sender))
                {
                    sequenceStep++;
                    if (sequenceStep == seq.Length)
                    {
                        solved = true;
                        Invoke("Solve",1.5f);
                    }
                }
                else
                {
                    Reset();
                }
            }
        }

        
    }

    /// <summary>
    /// Resetea la secuencia desde el principio
    /// </summary>
    public override void Reset()
    {  
        sequenceStep = 0;
        solved = false;
        initiator.CancelUse();

    }

    /// <summary>
    /// Resolucion del puzzle. Activa el objeto resultado
    /// </summary>
    public override void Solve()
    {
        result.GetComponent<IActivable>().Activate();
        AudioManager.Play(GetComponent<AudioSource>(), false, 1);

        iTween.ShakePosition(Camera.main.gameObject, new Vector3(1, 1, 0), 2f);
        InputManagerTLK.BeginVibrationTimed(1, 1);


    }

}
