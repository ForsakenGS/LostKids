using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharactersDetector : UsableObject {


    private List<GameObject> RequiredCharacters;

    private HashSet<GameObject> charactersInside;

    private Transform canvas;
    private Transform AoiIcon;
    private Transform AkaiIcon;
    private Transform KiIcon;

    void OnDisable() {
        CharacterStatus.KillCharacterEvent -= CharacterDied;
        RoomSettings.RoomPreparedEvent -= ResetCanvas;
    }

    void OnEnable() {
        CharacterStatus.KillCharacterEvent += CharacterDied;
        RoomSettings.RoomPreparedEvent += ResetCanvas;

        
    }

    void Awake() {

        GameObject[] chars = GameObject.FindGameObjectsWithTag("Player");

        RequiredCharacters = new List<GameObject>();

        for(int i = 0; i < 3; i++) {

            RequiredCharacters.Add(chars[i]);

        }

        canvas = transform.Find("Canvas");
        AoiIcon = canvas.transform.Find("Aoi");
        AkaiIcon = canvas.transform.Find("Akai");
        KiIcon = canvas.transform.Find("Ki");

        charactersInside = new HashSet<GameObject>();
    }

	// Use this for initialization
	new void Start () {

        base.Start();


	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter(Collider col) {
        if(col.gameObject.CompareTag("Player")) {

            CheckCharacterIn(col.gameObject);
            
            if(charactersInside.Count==1){
                    CharacterStatus.KillCharacterEvent += CharacterDied;
            }

            UpdateDetector();
        }
    }

    void OnTriggerExit(Collider col) {
        if(col.gameObject.CompareTag("Player")) {

            CheckCharacterOut(col.gameObject);

            if(charactersInside.Count==0){
                CharacterStatus.KillCharacterEvent -= CharacterDied;
            }
        }
    }

    void CheckCharacterIn(GameObject character) {

        if(RequiredCharacters.Contains(character)) {

            switch(character.GetComponent<CharacterStatus>().characterName) {
                case CharacterName.Aoi:
                    AoiIcon.gameObject.SetActive(false);
                    break;
                case CharacterName.Akai:
                    AkaiIcon.gameObject.SetActive(false);
                    break;
                case CharacterName.Ki:
                    KiIcon.gameObject.SetActive(false);
                    break;
                
            }

            charactersInside.Add(character);
        }
        
    }

        void CheckCharacterOut(GameObject character) {

        if(RequiredCharacters.Contains(character)) {

            switch(character.GetComponent<CharacterStatus>().characterName) {
                case CharacterName.Aoi:
                    AoiIcon.gameObject.SetActive(true);
                    break;
                case CharacterName.Akai:
                    AkaiIcon.gameObject.SetActive(true);
                    break;
                case CharacterName.Ki:
                    KiIcon.gameObject.SetActive(true);
                    break;
                
            }

            charactersInside.Remove(character);
        }
        
    }

    void UpdateDetector() {
        if(charactersInside.Count.Equals(RequiredCharacters.Count)) {
            Use();
        }
    }

    override public void Use() {

        if (!onUse) {
            
            //Comportamiento generico de un usable. (Activar objeto o notificar al puzzle segun situacion)
            base.Use();
        }
    }

    override public void CancelUse() {

            //Comportamiento base generico para todos los objetos usables
            base.CancelUse();   
    }

    void CharacterDied(GameObject character){
        CheckCharacterOut(character);
    }

    void ResetCanvas() {
        if (canvas) {

            canvas.gameObject.SetActive(false);
            canvas.gameObject.SetActive(true);

        }
    }
}
