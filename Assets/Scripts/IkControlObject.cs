using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Touch;
using UnityEngine.Events;
using DG.Tweening;

public class IkControlObject : MonoBehaviour
{
    [SerializeField] private string colorCode;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float onSelectZAxisOffset;
    /// <summary>
    /// can be register in other class or in inspector for position of transform, only invoked when it get selected
    /// </summary>
    public UnityEvent<Vector3, bool> OnVector { get { if (onVector == null) onVector = new UnityEvent<Vector3,bool>(); return onVector; } }
    [SerializeField] private UnityEvent<Vector3,bool> onVector;

    /// <summary>
    /// register for any ik bone to look at this transform, can be register in inspector 
    /// </summary>
    public UnityEvent<Transform> LookObject { get { if (lookObject == null) lookObject = new UnityEvent<Transform>(); return lookObject; } }
    [SerializeField] private UnityEvent<Transform> lookObject;
    public UnityEvent OnFinalStone { get { if (finalStone == null) finalStone = new UnityEvent(); return finalStone; } }
    [SerializeField] private UnityEvent finalStone;
    /*   public UnityEvent HandSelect { get { if (handSelect == null) handSelect = new UnityEvent(); return handSelect; } }
       [SerializeField] private UnityEvent handSelect;

       public UnityEvent HandDeselect { get { if (handDeselect == null) handDeselect = new UnityEvent(); return handDeselect; } }
       [SerializeField] private UnityEvent handDeselect;*/
    private Image image;
    private float zAxis;
    private bool isSelected = false;

    private RaycastHit hit;
    private LeanDragTranslateAlong drag;
    private bool isGrabableStone = false;
    private bool isFinalStore = false; 
    private Vector3 preSelectPosition;
    // Start is called before the first frame update
    private void Awake()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        if (TryGetComponent(out drag))
        {
            SetDistanceFromCam();
        }
        if (TryGetComponent(out LeanSelectableByFinger selectable))
        {
            selectable.OnSelected.AddListener(OnSelected);
            selectable.OnDeselected.AddListener(OnDeselect);
        }
        zAxis = transform.position.z;
    }
    public void OnSelected()
    {
        // transform.position = new Vector3(transform.position.x,transform.position.y, onSelectZAxisOffset);
        preSelectPosition = transform.position;
        transform.position += transform.forward * onSelectZAxisOffset * Time.deltaTime;
        SetDistanceFromCam();
        isSelected = true;
        lookObject.Invoke(transform);
        image.color = Color.red;
    }
    public void OnDeselect()
    {
        //  transform.position -= (Camera.main.transform.rotation * Vector3.back) * 50.0f * Time.deltaTime;
     //   transform.position -= transform.forward * onSelectZAxisOffset * Time.deltaTime;
        SetDistanceFromCam();
        isSelected = false;
        if (ColorUtility.TryParseHtmlString(colorCode, out Color color))
        {
            image.color = color;
        }
        if (isGrabableStone && Physics.Raycast(transform.position, -transform.forward, out hit, 10))
        {
            transform.position = hit.point + transform.forward * 0.5f; // its division factor you can remove or use variable for further use or testing
        }
        else if (isFinalStore && Physics.Raycast(transform.position, -transform.forward, out hit, 10))
        {
            transform.position = hit.point + transform.forward * 0.5f; // its division factor you can remove or use variable for further use or testing
            finalStone?.Invoke();
        }
        else
        {
            transform.DOMove(preSelectPosition, 0.15f);
        }
        //  lookObject.Invoke(null);
    }
    public void SetDistanceFromCam() 
    {
        var heading = transform.position - Camera.main.transform.position;
        drag.ScreenDepth.Distance = Vector3.Dot(heading, Camera.main.transform.forward);
    }
    void Start()
    {
        CreateMirrorImageInCanvas();
    }
    /// <summary>
    /// CreateMirrorImageInCanvas
    /// </summary>
    public void CreateMirrorImageInCanvas() 
    {
        GameObject NewObj = new GameObject();
        image = NewObj.AddComponent<Image>();
        Sprite spt = Resources.Load<Sprite>("Sprites/Lol_circle");
        image.sprite = spt;
        image.rectTransform.SetParent(canvas.transform);
        image.transform.localScale = Vector3.one * 0.5f;
        if (ColorUtility.TryParseHtmlString(colorCode, out Color color))
        {
            image.color = color;
        }
        image.raycastTarget = false;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        FollowTransform();
        if (isSelected)
        {
            onVector?.Invoke(transform.localPosition, isSelected);
            if (Physics.Raycast(transform.position, -transform.forward, out hit, 10))
            {
                Debug.DrawRay(transform.position, -transform.forward * 10, Color.green);
                switch (hit.collider.gameObject.tag)
                {
                    case "Stone":
                        image.color = Color.green;
                        isGrabableStone = true;
                        break;
                    case "EndStone":
                        image.color = Color.green;
                        isFinalStore = true;
                        break;
                    default:
                        image.color = Color.red;
                        isGrabableStone = false;
                        break;
                }
               /* if (hit.collider.gameObject.CompareTag("Stone"))
                {
                    image.color = Color.green;
                    isGrabableStone = true;
                }
                else
                if (!hit.collider.gameObject.CompareTag("Stone"))
                {
                    image.color = Color.red;
                    isGrabableStone = false;
                }*/
            }
        }
    }
    /// <summary>
    /// Follow vector 3 transform on canvas
    /// </summary>
    public void FollowTransform() 
    {
        //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 movePos;

        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPos, canvas.worldCamera, out movePos);
        //Convert the local point to world point
        image.transform.position =  canvas.transform.TransformPoint(movePos);
    }
}