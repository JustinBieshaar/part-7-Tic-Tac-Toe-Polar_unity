

            using UnityEngine;

            // https://answers.unity.com/questions/319924/object-rotation-relative-to-camera.html
            public class RotateTowardsCamera : MonoBehaviour {
                private void Update() {
                    transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
                }
            }
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            