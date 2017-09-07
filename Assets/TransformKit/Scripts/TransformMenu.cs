using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class TransformMenu : MonoBehaviour, IInputClickHandler {

    public static TransformMenu instance;

    [HideInInspector]
    public enum Mode { Move, Scale, Rotate, Reset, None };

    [HideInInspector]
    public Mode currentMode = Mode.None;

    [HideInInspector]
    public bool showMenu = false;

    public GameObject Graph;

    private Vector3 position;
    private Vector3 scale;
    private Quaternion rotation;

    private BoxCollider GraphCollider;

    MoveScript moveComponent;
    RotateScript rotateComponent;
    ScaleScript scaleComponent;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        scale = Graph.transform.localScale;
        rotation = Graph.transform.localRotation;
        position = Graph.transform.localPosition;

        moveComponent = Graph.GetComponent<MoveScript>();
        rotateComponent = Graph.GetComponent<RotateScript>();
        scaleComponent = Graph.GetComponent<ScaleScript>();

        Graph.GetComponent<BoxCollider>().enabled = false;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (!showMenu)
        {
            showMenu = true;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            Graph.GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            showMenu = false;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            Graph.GetComponent<BoxCollider>().enabled = false;

            TransformMenu.instance.currentMode = Mode.None;
        }

    }

    private void Update()
    {
        switch (TransformMenu.instance.currentMode)
        {
            case TransformMenu.Mode.Move:
                moveComponent.SetDragging(true);
                rotateComponent.SetRotating(false);
                scaleComponent.SetResizing(false);
                break;
            case TransformMenu.Mode.Rotate:
                rotateComponent.SetRotating(true);
                moveComponent.SetDragging(false);
                scaleComponent.SetResizing(false);
                break;
            case TransformMenu.Mode.Scale:
                scaleComponent.SetResizing(true);
                moveComponent.SetDragging(false);
                rotateComponent.SetRotating(false);
                break;
            case TransformMenu.Mode.Reset:
                TransformMenu.instance.currentMode = TransformMenu.Mode.None;
                //Call the reset function
                ResetTransform();
                break;
            default:
                scaleComponent.SetResizing(false);
                moveComponent.SetDragging(false);
                rotateComponent.SetRotating(false);
                //Debug.Log("Nothing is enabled");
                break;
        }
    }

    void ResetTransform()
    {
        Graph.transform.localScale = scale;
        Graph.transform.localRotation = rotation;
        Graph.transform.localPosition = position;
    }

}
