using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tanuki : UsableObject {
    // Listado con índices de los mensajes a mostrar
    public List<int> askList;
    public List<int> thanksList;

    // Objeto requerido
    public InventoryObject requested;

    // Nombre del objeto requerido
    private string requestedObjectName;

    // Referencias
    private CutScene cutScene;
    private CutScene fearCutScene;
    private GameObject canvas;
    private MessageManager messageManager;
    private Animator animator;
    private AudioSource strongHit;
    private AudioSource softHit;
    private AudioSource idleSound1;
    private AudioSource idleSound2;

    // Use this for references
    void Awake() {
        messageManager = GameObject.FindGameObjectWithTag("MessageManager").GetComponent<MessageManager>();
        cutScene = GetComponent<CutScene>();
        fearCutScene = targets[0].GetComponent<CutScene>();
        canvas = GetComponentInChildren<Canvas>().gameObject;
        animator = GetComponent<Animator>();
        ChangeTooltipStatus(TooltipManager.On);
    }

    // Use this for initialization
    new void Start() {
        base.Start();
        type = UsableTypes.Instant;
        requestedObjectName = requested.GetComponent<InventoryObject>().objectName;
        AudioLoader audioLoader = GetComponent<AudioLoader>();
        strongHit = audioLoader.GetSound("StrongHit");
        softHit = audioLoader.GetSound("SoftHit");
        idleSound1 = audioLoader.GetSound("Idle1");
        idleSound2 = audioLoader.GetSound("Idle2");
    }

    void BeginAskConversation() {
        BeginConversation(askList);
    }

    void BeginConversation(List<int> indexList) {
        messageManager.ShowConversation(indexList);
    }

    void BeginThanksConversation() {
        BeginConversation(thanksList);
    }

    // Muestra/oculta el tooltip del Tanuki
    void ChangeTooltipStatus(bool status) {
        canvas.SetActive(status);
    }

    /// <summary>
    /// Called when TanukiIdle animation ends and chooses if TanukiSoftKits animation is played
    /// </summary>
    public void IdleFinished() {
        if (Random.Range(0,3) == 2) {
            animator.SetTrigger("SoftHits");
        }
        if (Random.Range(0,5) == 0) {
            AudioManager.Play(idleSound1, false, 1);
        } else {
            AudioManager.Play(idleSound2, false, 1);
        }
    }

    void OnEnable() {
        TooltipManager.TooltipOnOff += ChangeTooltipStatus;
    }

    void OnDisable() {
        TooltipManager.TooltipOnOff -= ChangeTooltipStatus;
    }

    void SolveFearAnimation() {
        animator.SetTrigger("HardHits");
    }

    void SolveFear() {
        if (fearCutScene == null) {
            base.Use();
        } else {
            fearCutScene.BeginCutScene(base.Use);
        }
        Invoke("ShowThanksConversation", cutScene.cutSceneTime + 0.5f);
    }

    override public void Use() {
        // Comprueba si el personaje posee el objeto solicitado
        if (CharacterManager.GetActiveCharacter().GetComponent<CharacterInventory>().GetObject(requestedObjectName)) {
            // Eliminación del miedo
            cutScene.BeginCutScene(SolveFearAnimation);
            Invoke("MoveCharacterToFront", 0.5f);
            Invoke("SolveFear", 3.5f);
        } else {
            // Muestra la conversación para pedir el objeto
            if (cutScene == null) {
                messageManager.ShowConversation(askList);
            } else {
                cutScene.BeginCutScene(BeginAskConversation);
                Invoke("MoveCharacterToFront", 0.5f);
            }
        }
    }

    private void MoveCharacterToFront() {
        GameObject character = CharacterManager.GetActiveCharacter();
        Vector3 newCharacterPos = character.transform.position;
        newCharacterPos.z = transform.position.z;
        newCharacterPos.x = transform.position.x;
        newCharacterPos += transform.forward * 1.8f;
        character.transform.position = newCharacterPos;
        Vector3 lookPosition = transform.position;
        lookPosition.y = newCharacterPos.y;
        character.transform.LookAt(lookPosition);
    }

    void ShowThanksConversation() {
        // Muestra la conversación de agradecimiento
        if (cutScene == null) {
            messageManager.ShowConversation(thanksList);
        } else {
            cutScene.BeginCutScene(BeginThanksConversation);
            Invoke("MoveCharacterToFront", 0.5f);
        }
    }

    public void StrongHit() {
        AudioManager.Play(strongHit, false, 1);
    }

    public void SoftHit() {
        AudioManager.Play(softHit, false, 1, 0.9f, 1.1f);
    }
}
