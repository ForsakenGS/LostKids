using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarningZone : MonoBehaviour {

    private List<CharacterName> affected;

    //public Sprite tooltipImage;
    //private CharacterIcon icon;

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.CompareTag("Player") && affected.Contains(col.gameObject.GetComponent<CharacterStatus>().characterName)) {

            //icon = col.gameObject.GetComponentInChildren<CharacterIcon>();
            //icon.ActiveCanvas(true);
            //icon.SetImage(tooltipImage);

        }
    }

    void OnTriggerExit(Collider col) {
        if (col.gameObject.CompareTag("Player") && affected.Contains(col.gameObject.GetComponent<CharacterStatus>().characterName)) {
            //icon.ActiveCanvas(false);
        }
    }

    public void SetAffectedCharacters(List<CharacterName> characters) {
        affected = characters;
    }

    void OnTriggerStay(Collider col) {
        if (CharacterManager.IsActiveCharacter(col.gameObject)
            && affected.Contains(col.gameObject.GetComponent<CharacterStatus>().characterName)) {

        }
    }

    public void EnableZone() {
        GetComponent<Collider>().enabled = true;
    }

    public void DisableZone() {
        //if (icon != null) {
       //     icon.ActiveCanvas(false);
       // }
        GetComponent<Collider>().enabled = false;
    }
}
