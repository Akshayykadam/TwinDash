using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotateCar : MonoBehaviour
{
    public Vector3 RotateAmount;  // degrees per second to rotate in each axis. Set in inspector.
    float dir;
    int control;
    
    // Start is called before the first frame update
    void Start()
    {
        int controlActive = PlayerPrefs.GetInt("Control", 2);
        if (controlActive == 1)
        {
            control = 1;
        }
        else
        {
            control = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.finished)
        {
            if (control == 1)
            {
                if (!Application.isMobilePlatform)
                {
                    dir = SimpleInput.GetAxis("Horizontal");

                }
                else
                {

                    if (Input.GetMouseButton(0)) // If we press the mouse button, check our position relative to the screen center
                    {
                        // If we are to the right of the screen, rotate to the right
                        if (Input.mousePosition.x > Screen.width * 0.5f)
                        {
                            dir = 0.8f;
                        }
                        else // Othwerwise, rotate to the left
                        {
                            dir = -0.8f;
                        }
                    }
                    else // Otherwise, if we didn't press anything, don't rotate and straighten up
                    {
                        dir = 0;
                    }

                }

                transform.Rotate(dir * RotateAmount * Time.deltaTime);

            }
            else
            {
                dir = SimpleInput.GetAxis("Horizontal");
                transform.Rotate(dir * RotateAmount * Time.deltaTime);



            }
        }
    }
}
