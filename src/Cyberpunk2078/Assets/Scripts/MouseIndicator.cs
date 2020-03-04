using System.Collections.Generic;
using UnityEngine;
//using UnityScript.Steps;


public enum InputType : int
{
    MouseAndKeyboard = 0,
    Joystick = 1,
}


public class MouseIndicator : MonoBehaviour
{
    public static MouseIndicator Singleton { get; private set; }


    // Start is called before the first frame 
    private GameObject player;
    private List<Vector2> directionRef;
    private float timeCount = 0.0f;
    private float deadZone = 0.2f;
    private Vector2 controllerDir;
    private Vector3 lastMousePos;
    private bool locked;
    [SerializeField] private Color correct;
    [SerializeField] private Color error;
    private Color defaultColor;
    private Vector3 defaultScale;
    private SpriteRenderer renderer;


    private InputType currentInputType;


    public Event2<InputType> OnInputTypeChange { get; private set; } = new Event2<InputType>();

    public InputType CurrentInputType
    {
        get
        {
            return currentInputType;
        }
        private set
        {
            if (value != currentInputType)
            {
                InputType previousValue = currentInputType;

                currentInputType = value;
                OnInputTypeChange.Invoke(value, previousValue);
            }
        }
    }


    private void Awake()
    {
        if (Singleton)
            Destroy(gameObject);
        else
            Singleton = this;
    }
    
    public void Hide()
    {
        renderer.enabled = false;
    }

    public void Show()
    {
        renderer.enabled = true;
    }
    void Start()
    {
        player  = GameObject.FindGameObjectWithTag("Player");
        locked = false;
        renderer = GetComponentInChildren<SpriteRenderer>();
        defaultColor = renderer.color;
        defaultScale = renderer.transform.localScale;
    }

    // Update is called once per frame
    private void Update()
    {
        if (locked) return;
        
        //transform.position = player.transform.position;
        var mousePosInScreen = Input.mousePosition;
        if (Mathf.Abs(Input.GetAxis("HorizontalJoyStick")) > 0 || Mathf.Abs(Input.GetAxis("VerticalJoyStick")) > 0)
        {
            controllerDir = new Vector3(Input.GetAxis("HorizontalJoyStick"),Input.GetAxis("VerticalJoyStick")).normalized;
            CurrentInputType = InputType.Joystick;
        }
        else if (Input.mousePosition != lastMousePos)
        {
            CurrentInputType = InputType.MouseAndKeyboard;
        }

        lastMousePos = mousePosInScreen;

        if (CurrentInputType == InputType.Joystick)
        {
            //use indicator for controller

            mousePosInScreen.z = 10; // select distance = 10 units from the camera
            Vector2 direction = -controllerDir;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = rotation;
        }
        else if(CurrentInputType == InputType.MouseAndKeyboard)
        {
            mousePosInScreen.z = 10; // select distance = 10 units from the camera
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(mousePosInScreen);
            Vector2 direction = -(mousePos - (Vector2)transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = rotation;
        }


    }


    public Vector2 GetAttackDirection()
    {
        if (controllerDir != Vector2.zero)
        {
            return controllerDir;
        }

        transform.position = player.transform.position;
        var mousePosInScreen = Input.mousePosition;
        mousePosInScreen.z = 10; // select distance = 10 units from the camera
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mousePosInScreen);
        return   (mousePos - (Vector2) transform.position).normalized;
    }

    public void Lock()
    {
        locked = true;
    }

    public void Unlock()
    {
        locked = false;
    }

    public bool DirectionNotification(Vector2 target, float angleInterval)
    {
        
        if (Vector2.Angle(target, GetAttackDirection()) < angleInterval)
        {
            // Right Direction
            GetComponentInChildren<SpriteRenderer>().color = correct;
            renderer.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
            return true;
        }
        else
        {
            GetComponentInChildren<SpriteRenderer>().color = error;
            renderer.transform.localScale = defaultScale;
            return false;
        }
         
    }

    public void ResetColor()
    {
        GetComponentInChildren<SpriteRenderer>().color = defaultColor;
        renderer.transform.localScale = defaultScale;
    }
    
    public Vector2 GetDirectionCorrection(Vector2 _norm)
    {
        Vector2 direction = GetAttackDirection();
        if (Vector2.Angle(direction, _norm) < 80)
        {
            // Allowed for free dash
            return direction;
        }
        
        // need to be on the axis of perpendicular to norm
        Vector2 corDir  = Vector2.Perpendicular(_norm);
        if (direction.x * corDir.x > 0) return corDir;

        return -corDir;
    }

//    public Vector2 getTranslatedDirection(Vector2 direction)
//    {
//        // the direction parameter must be normalized
//        foreach (var scaler in directionRef)
//        {
//            if (Mathf.Abs(Vector2.Angle(scaler, direction)) < 23f)
//            {
//                return scaler;
//            }
//        }
//        
//        // should not reach here
//        return Vector2.zero;
//    }
}
