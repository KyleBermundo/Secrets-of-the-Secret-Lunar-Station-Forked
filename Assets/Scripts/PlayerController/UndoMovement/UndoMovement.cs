using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;

public class UndoMovement : MonoBehaviour, IOnInventoryChange
{
    private List<IOnUndoChargesChange> iOnUndoChargesChange = new List<IOnUndoChargesChange>();
    Stack<Vector3> undoStack = new Stack<Vector3>();
    [SerializeField]
    int undoDistance = 10;
    CharacterController characterController;
    Vector3 lastUndo;
    [SerializeField]
    int undoCharges = 1;
    public int UndoCharges { get { return undoCharges; } set { undoCharges = value; SetObserverCharges(value, false); } }
    bool undoUsed;
    [SerializeField]
    Vector3 startPos;

    UndoIndicator undoInd;
    [SerializeField] bool hasUndo;
    Inventory inventory;


    private void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        inventory.AddInventoryListener(this);
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        undoInd = FindObjectOfType<UndoIndicator>();
        SetObserverCharges (undoCharges, undoUsed = false);
        undoStack.Push (startPos);
    }

    public void AddUndoMovementListener(IOnUndoChargesChange listener)
    {
        if (iOnUndoChargesChange.Contains(listener) == false) iOnUndoChargesChange.Add(listener);
    }

    public void RemoveUndoMovementListener(IOnUndoChargesChange listener)
    {
        if (iOnUndoChargesChange.Contains(listener)) iOnUndoChargesChange.Remove(listener);
    }

    // Update is called once per frame
    void Update()
    {

        if (CheckDistancePeek() && characterController.isGrounded)
        {
            undoStack.Push(transform.position);
        }


        Debug.DrawRay(undoStack.Peek(), Vector3.up, Color.yellow);

        if (Input.GetKeyDown(KeyCode.F) && hasUndo)
        {
            Debug.Log("Undo kutsuttiin");
            if (undoStack.Count > 1 && undoCharges > 0 && Vector3.Distance(transform.position, undoStack.Peek()) > (undoDistance / 2))
            {
                UndoLastMovement();
            }
                
            else if (undoStack.Count > 2 && undoCharges > 0 && Vector3.Distance(transform.position, undoStack.Peek()) < (undoDistance / 2))
            {
                undoStack.Pop();
                UndoLastMovement();
            }
            else if (undoStack.Count <= 1) Debug.Log("No positions available!");
            else if (undoCharges <= 0) Debug.Log("Not enough charges");
        }

    }

    void UndoLastMovement()
    {
        lastUndo = undoStack.Peek();
        transform.position = undoStack.Pop();
        undoCharges--;
        SetObserverCharges(undoCharges, undoUsed = true);
        undoUsed = false;

        if (undoStack.Count < 2 && characterController.isGrounded)
        {
            undoStack.Push(transform.position);
        }
    }

    public void AddUndoCharge()
    {
        undoCharges++;
        SetObserverCharges(undoCharges, undoUsed = false);

    }

    public void ClearCharges()
    {
        undoCharges = 0;
        SetObserverCharges(undoCharges, undoUsed = false);
    }

    public void ClearStack()
    {
        undoStack.Clear();
    }

    bool CheckDistancePeek()
    {
        if (Vector3.Distance(transform.position, undoStack.Peek()) > undoDistance) return true;
        else return false;
    }

    void SetObserverCharges(int undoCharges,bool undoUsed) //another place to edit maybe
    {
        foreach (var i in iOnUndoChargesChange)
        {
            i.OnUndoChargesChange(undoCharges, undoUsed);
        }
    }

    public void PushZero()
    {
        undoStack.Push(startPos);
    }

    public void PushCurrent()
    {
        undoStack.Push(transform.position);
    }

    public void OnUndoPickUp()
    {
        undoCharges = 1;
        undoStack.Clear();
        undoStack.Push(startPos);
        undoStack.Push(startPos);

    }

    public void OnInventoryChange(Inventory inventory)
    {
        hasUndo = inventory.HasUndo;
    }

    private void OnEnable()
    {
        inventory.AddInventoryListener(this);
    }

    private void OnDisable()
    {
        inventory.RemoveInventoryListener(this);
    }
}
