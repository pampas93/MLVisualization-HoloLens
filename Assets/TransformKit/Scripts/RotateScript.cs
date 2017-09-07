using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class RotateScript : MonoBehaviour, IManipulationHandler {

    [Tooltip("Speed of static rotation when tapping game object.")]
    [SerializeField]
    float RotateSpeed = 0.2f;

    [Tooltip("Speed of interactive rotation via navigation gestures.")]
    [SerializeField]
    float RotationFactor = 50f;

    Quaternion lastRotation;

    [SerializeField]
    bool rotatingEnabled = false;

    public void SetRotating(bool enabled)
    {
        rotatingEnabled = enabled;
    }


    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        //Pushing this gameObject into the Modal Input Stack
        InputManager.Instance.PushModalInputHandler(gameObject);
        lastRotation = transform.rotation;
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        if (rotatingEnabled)
        {

            var rotation = new Vector3(eventData.CumulativeDelta.y * RotationFactor,
                    eventData.CumulativeDelta.x * RotationFactor,
                    eventData.CumulativeDelta.z * RotationFactor);

            Rotate(rotation);
        }
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
    }

    //Rotate only along the y axis
    void Rotate(Vector3 rotation)
    {

        var x = lastRotation.x;
        var y = rotation.y * RotateSpeed;
        var z = lastRotation.z;

        transform.Rotate(x, y, z, Space.Self);
    }
}
