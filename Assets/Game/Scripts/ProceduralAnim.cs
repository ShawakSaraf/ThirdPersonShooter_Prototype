using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnim : MonoBehaviour
{
    [System.Serializable]
    public class AnimatedParts
    {
        public Transform transform;

        //Multiplier per angle
        public Vector3 eulerRotationsYawDelta;
        public Vector3 eulerRotationsVelocity;

        public float rotationDamp;
        [HideInInspector]
        public Quaternion rotation;
        public Quaternion startRotation;
        public Quaternion localRotation;
    }

    public Transform visuals;

    [Header("Movement")]
    public float accelaration;
    public float maxVelocity;

    [Header("Animation")]
    public float visualsRotationDamp;
    public float maxYawDelta;
    public float yawDeltaDamp;
    public AnimatedParts[] animatedParts;

    //Parts
    public Rigidbody body;

    float yawDelta;
    Vector3 lastForward;

    void Star()
    {
        // body = body.GetComponent<Rigidbody>();
        for (int i = 0; i < animatedParts.Length; i++)
        {
            animatedParts[i].startRotation = animatedParts[i].transform.localRotation; 
        }
    }
    void LateUpdate()
    {
        if(body.velocity.sqrMagnitude > float.Epsilon)    
        {
            Quaternion lookRot = Quaternion.LookRotation(body.velocity, Vector3.up);
            visuals.rotation = Quaternion.Slerp(visuals.rotation, lookRot, Time.deltaTime * visualsRotationDamp);
        }
        
        //Determins Yawdelta, comparing last Rotation with current rotation, divided by deltatime to get the Angular velocity
        float newYawDelta = Vector3.SignedAngle(lastForward, visuals.forward, Vector3.up) / Time.deltaTime;
        // saving current forwar to compare next frame
        lastForward = visuals.forward;

        //Mathf.Abs removes negetive no. making it go0d to compare lenghts, also clamping Yawdelta so rotation don't go out of hand
        if(Mathf.Abs(newYawDelta) > maxYawDelta)
        {
            if (newYawDelta >= 0)
            {
                newYawDelta = maxYawDelta;
            }
            else
            {
                newYawDelta = -maxYawDelta;
            }
        }

        //*0,01f to scale down values
        yawDelta = Mathf.Lerp(yawDelta, newYawDelta * 0.01f, Time.deltaTime * yawDeltaDamp);

        // From world velocity to local velocity (to get negetive no.)
        Vector3 localVelocity = visuals.InverseTransformDirection(body.velocity);
        //we transform local velocity into a vector we want to use for rotation, z(forward) to rotate in x axis
        localVelocity = new Vector3(localVelocity.z, localVelocity.x, localVelocity.x);

        //Looping through Animated Parts and appling Procedural Anim
        for (int i = 0; i < animatedParts.Length; i++)
        {
            Vector3 newYawDeltaRot = animatedParts[i].eulerRotationsYawDelta * yawDelta;
            Vector3 newVelocityRot = Vector3.Scale(animatedParts[i].eulerRotationsVelocity, localVelocity);

            Quaternion newRot = Quaternion.Euler(newVelocityRot + newVelocityRot);

            animatedParts[i].rotation = Quaternion.Slerp(animatedParts[i].rotation, newRot , Time.deltaTime * animatedParts[i].rotationDamp);
            
            animatedParts[i].transform.localRotation = animatedParts[i].startRotation * animatedParts[i].rotation; 
            
            // animatedParts[i].transform.localRotation = 
            // Quaternion.Slerp(animatedParts[i].rotation, newRot, Time.deltaTime * animatedParts[i].rotationDamp);
        }
    }
}