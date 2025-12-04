using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerInput input;
    public CharacterController cc;
    [Header("move Controll")]
    public float moveSpeed = 10f;

    [Header("view Controll")]
    public Transform camTrans;
    public float roundSpeedX = 200f;
    public float roundSpeedY = 200f;
    public float vertLimitMin = -60f;
    public float vertLimitMax = 60f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float camXRotation = 0f;

    public float interactDistance = 3.5f;
    private BaseInteractableObj interactableObj;


    // Start is called before the first frame update
    void Start()
    {
        input.onActionTriggered += OnActionTriggered;
        camXRotation = camTrans.localEulerAngles.x;     //x rotation of cam in the begin

        //close the cursor
        Cursor.lockState = CursorLockMode.Locked;   //lock the cursor to then center of screen  which can let camera rotate infinity;
        Cursor.visible = false;
    }

    private void OnActionTriggered(InputAction.CallbackContext context)
    {
        //get the input
        switch (context.action.name) {
            case "Move":
                moveInput =  context.ReadValue<Vector2>();
                break;
            case "Look":
                lookInput = context.ReadValue<Vector2>();
                break;
            case "Interact":
                if(context.phase == InputActionPhase.Performed) {
                    print("try interact with an object");
                    interactableObj?.Interact();
                }
                break;
        }
    }

    private void Update()
    {
        //move controll
        Vector3 moveDir = transform.forward * moveInput.y + transform.right * moveInput.x; 
        cc.SimpleMove(moveDir * moveSpeed);

        //rotate controll
        float angleX = lookInput.x * roundSpeedX * Time.deltaTime;
        float angleY = lookInput.y * roundSpeedY * Time.deltaTime;
        transform.localRotation = transform.rotation * Quaternion.AngleAxis(angleX, transform.up);
        camXRotation = Mathf.Clamp(camXRotation - angleY, vertLimitMin, vertLimitMax);
        camTrans.localEulerAngles = Vector3.right * camXRotation;

        //interact controll
        Ray ray = new Ray(camTrans.position, camTrans.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);

        //���߼���Ƿ�������
        if(Physics.Raycast(ray, out RaycastHit hitInfo, interactDistance, 1 << LayerMask.NameToLayer("InteractableObj"))) {
            //if hit and now without obj , execute enter logic
            if(interactableObj == null) {
                interactableObj = hitInfo.collider.GetComponent<BaseInteractableObj>();
                interactableObj.EnterView();
            }
            //if hit and now has an obj and this obj is different with prev, change it and let it go out
            else if(interactableObj != null && !interactableObj.gameObject.Equals(hitInfo.collider.gameObject)) {
                interactableObj.ExitView();
                interactableObj = hitInfo.collider.GetComponent<BaseInteractableObj>();
                interactableObj.EnterView();
            }
            
        }
        else if(interactableObj != null){
            //if now hit nothing and prev has an obj, then excute it exit logic
            interactableObj.ExitView();
            interactableObj = null;
        }

        //other
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnDestroy()
    {
        input.onActionTriggered -= OnActionTriggered;
    }
}
