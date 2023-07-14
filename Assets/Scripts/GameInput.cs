using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameInput : MonoBehaviour { 
    
    private Vector2 initialTouchPosition;
    private Vector2 touchPosition;
    private bool touchStarted = true;

    /// <summary>
    ///This method calculates the normalized dragging direction on the touchscreen
    /// </summary>
    /// 
    /// <param name="initialPosition">Initial position of the finger when touch event started.</param>
    /// <param name="touchposition">Position of the finger when the touch continues.</param>
    /// <returns>The dragging direction on the touchscreen.</returns>
    public Vector2 GetTouchDragVectorNormalized() { 
        //initial position is only changed once when the touch is started

        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if (touchStarted == false)
            {
                touchStarted = true;
                initialTouchPosition = touch.position;
            }
           
            touchPosition = touch.position;
           
        }
        else
        {
            touchStarted = false;
            initialTouchPosition = Vector2.zero;
            touchPosition = Vector2.zero;
        }
        //if not touched, method returns 0,0 Vector2
       
        return  ((touchPosition - initialTouchPosition).normalized);
    }     
}

        
