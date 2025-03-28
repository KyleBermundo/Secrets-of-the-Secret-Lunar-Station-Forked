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
    LinkedList<Vector3> undoStack = new LinkedList<Vector3>();
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
        undoStack.AddLast (startPos);
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
            undoStack.AddLast(transform.position); // Changed from Push() to AddLast()
        }

        Debug.DrawRay(undoStack.Last.Value, Vector3.up, Color.yellow);

        if (Input.GetKeyDown(KeyCode.F) && hasUndo)
        {
            Debug.Log("Undo called");
            if (undoStack.Count > 1 && undoCharges > 0 && Vector3.Distance(transform.position, undoStack.Last.Value) > (undoDistance / 2))
            {
                UndoLastMovement();
            }
            else if (undoStack.Count > 2 && undoCharges > 0 && Vector3.Distance(transform.position, undoStack.Last.Value) < (undoDistance / 2))
            {
                undoStack.RemoveLast(); // Changed from Pop() to RemoveLast()
                UndoLastMovement();
            }
            else if (undoStack.Count <= 1) Debug.Log("No positions available!");
            else if (undoCharges <= 0) Debug.Log("Not enough charges");
        }
    }

    void UndoLastMovement()
    {
        if (undoStack.Count > 0)
        {
            lastUndo = undoStack.Last.Value;
            transform.position = lastUndo;
            undoStack.RemoveLast(); // Changed from Pop() to RemoveLast()
            undoCharges--;
            SetObserverCharges(undoCharges, undoUsed = true);
            undoUsed = false;

            if (undoStack.Count < 2 && characterController.isGrounded)
            {
                undoStack.AddLast(transform.position); // Changed from Push() to AddLast()
            }
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
        if (Vector3.Distance(transform.position, undoStack.Last.Value) > undoDistance) return true;
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
        undoStack.AddLast(startPos);
    }

    public void PushCurrent()
    {
        undoStack.AddLast(transform.position);
    }

    public void OnUndoPickUp()
    {
        undoCharges = 1;
        undoStack.Clear();
        undoStack.AddLast(startPos);
        undoStack.AddLast(startPos);

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
