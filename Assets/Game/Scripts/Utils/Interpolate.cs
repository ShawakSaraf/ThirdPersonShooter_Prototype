using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace My.Utils
{
    public class Interpolators
    {
        // THIS AIN'T WORKING!

        public static IEnumerator CR_Lerp(float LHS, float a, float b, float t, WaitForSeconds waitForSeconds)
        {
            while (true)
            {
                LHS = Mathf.Lerp( a, b, t);
                yield return waitForSeconds;
            }
        }

        public static IEnumerator CR_Lerp(Vector3 LHS, Vector3 a, Vector3 b, float t, WaitForSeconds waitForSeconds)
        {
            while (true)
            {
                LHS = Vector3.Lerp( a, b, t);
                yield return waitForSeconds;
            }
        }

        public static IEnumerator CR_SmoothStep(float LHS, float a, float b, float t, WaitForSeconds waitForSeconds)
        {
            while (true)
            {
                LHS = Mathf.SmoothStep( a, b, t);
                yield return waitForSeconds;
            }
        }

        public static IEnumerator CR_SmoothDamp(float LHS, float a, float b, float currVelocity, float smoothTime, float maxSpeed , WaitForSeconds waitForSeconds)
        {
            while (true)
            {
                LHS = Mathf.SmoothDamp( a, b, ref currVelocity, smoothTime, maxSpeed);
                yield return waitForSeconds;
            }
        }
        public static IEnumerator CR_SmoothDamp(Vector3 LHS, Vector3 a, Vector3 b, Vector3 currVelocity, float smoothTime, float maxSpeed , WaitForSeconds waitForSeconds)
        {
            Vector3 CV = currVelocity;
            while (true)
            {
                LHS = Vector3.SmoothDamp( a, b, ref CV, smoothTime, maxSpeed);
                yield return waitForSeconds;
            }
        }
    }
}
