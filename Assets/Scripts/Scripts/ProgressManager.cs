using UnityEngine;
using System.Collections;

public class ProgressManager : MonoBehaviour
{
    public Transform blockerTransform;
    public Transform secondHalfTransform;
    public float timeInSeconds;
    private float blockerRotationAngle;
    private float secondhalfRotationAngle;
    private float initialRotation;
    public bool StartTimer;
    bool shouldRotateBlock;
    bool shouldRotateSecondHalf;

    void Start()
    {
        blockerRotationAngle = 360 / timeInSeconds;
        secondhalfRotationAngle = 360 / timeInSeconds;
        shouldRotateBlock = true;
        shouldRotateSecondHalf = false;
    }


    void Update()
    {
        if (!StartTimer)
            return;

        if (blockerTransform.localRotation.eulerAngles.z <= 181 && blockerTransform.localRotation.eulerAngles.z != 0)
        {
            shouldRotateBlock = false;
            blockerTransform.localPosition = new Vector3(blockerTransform.localPosition.x, blockerTransform.localPosition.y, -1f);
            shouldRotateSecondHalf = true;
        }

        if (secondHalfTransform.localRotation.eulerAngles.z <= 181 && secondHalfTransform.localRotation.eulerAngles.z != 0)
            shouldRotateSecondHalf = false;


        RotateBlocker();
        RotateSecondHalf();
    }



    void RotateBlocker()
    {
        if (!shouldRotateBlock)
            return;

        blockerTransform.Rotate(new Vector3(0, 0, 1), -1 * blockerRotationAngle * Time.deltaTime);
    }

    void RotateSecondHalf()
    {
        if (!shouldRotateSecondHalf)
            return;

        secondHalfTransform.Rotate(new Vector3(0, 0, 1), -1 * secondhalfRotationAngle * Time.deltaTime);
    }
}
