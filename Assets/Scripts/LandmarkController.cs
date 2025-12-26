using UnityEngine;

public class LandmarkController : MonoBehaviour
{
    public HandDataReceiver handDataReceiver;
    
    private float initialDistance = 0f;
    private Vector3 initialScale;
    public float rotationSpeed = 50f;
    
    void Start()
    {
        initialScale = transform.localScale;
        handDataReceiver = FindObjectOfType<HandDataReceiver>();
    }
    
    void Update()
    {
        if (handDataReceiver == null || handDataReceiver.currentHandData == null)
            return;
        
        if (handDataReceiver.currentHandData.hands.Length >= 2)
        {
            var hand1 = handDataReceiver.currentHandData.hands[0];
            var hand2 = handDataReceiver.currentHandData.hands[1];
            
            if (hand1.landmarks.Length > 8 && hand2.landmarks.Length > 8)
            {
                Vector2 pos1 = new Vector2(hand1.landmarks[8].x, hand1.landmarks[8].y);
                Vector2 pos2 = new Vector2(hand2.landmarks[8].x, hand2.landmarks[8].y);
                
                float xDistance = Mathf.Abs(pos2.x - pos1.x);
                
                if (initialDistance == 0f)
                {
                    initialDistance = xDistance;
                }
                else
                {
                    float scaleFactor = xDistance / initialDistance;
                    transform.localScale = initialScale * Mathf.Clamp(scaleFactor, 0.5f, 3f);
                }
                
                if (pos1.y > pos2.y + 0.05f)
                {
                    transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
                }
                else if (pos2.y > pos1.y + 0.05f)
                {
                    transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
                }
            }
        }
        else
        {
            initialDistance = 0f;
        }
    }
}