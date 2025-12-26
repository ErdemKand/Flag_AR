using UnityEngine;

public class HandInteraction : MonoBehaviour
{
    public HandDataReceiver handDataReceiver;
    public GameObject handCursor;
    public GameController gameController;

    
    private GameObject selectedFlag = null;
    private Vector3 handPosition;
    private bool isGrabbing = false;
    
    void Start()
    {
        handCursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        handCursor.transform.localScale = Vector3.one * 0.15f;
        handCursor.GetComponent<Renderer>().material.color = Color.yellow;
        Destroy(handCursor.GetComponent<Collider>());
    }
    
    void Update()
    {
        if (handDataReceiver == null || handDataReceiver.currentHandData == null)
            return;
        
        if (handDataReceiver.currentHandData.hands.Length > 0)
        {
            var hand = handDataReceiver.currentHandData.hands[0];
            
            if (hand.landmarks.Length > 8)
            {
                // Index finger tip
                var indexTip = hand.landmarks[8];
                float x = (indexTip.x - 0.5f) * 12f;  
                float y = -(indexTip.y - 0.5f) * 12f; 
                float z = 0;
                
                handPosition = new Vector3(x, y, z);
                handCursor.transform.position = handPosition;
                
                // Grab detection: Thumb tip ve Index tip arası mesafe
                if (hand.landmarks.Length > 4)
                {
                    var thumbTip = hand.landmarks[4];
                    float distance = Vector2.Distance(
                        new Vector2(indexTip.x, indexTip.y),
                        new Vector2(thumbTip.x, thumbTip.y)
                    );
                    
                    // Mesafe küçükse = parmaklar birbirine yakın = tutma hareketi
                    isGrabbing = distance < 0.05f;
                    
                    // Görsel feedback
                    handCursor.GetComponent<Renderer>().material.color = 
                        isGrabbing ? Color.red : Color.yellow;
                }
                
                // Bayrak tutma
                if (isGrabbing && selectedFlag == null)
                {
                    TryGrabFlag();
                }
                else if (!isGrabbing && selectedFlag != null)
                {
                    TryDropFlag();
                }
                
                // Tutulan bayrağı el ile taşı
                if (selectedFlag != null)
                {
                    selectedFlag.transform.position = handPosition;
                }
            }
        }
    }
    
    void TryGrabFlag()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(handPosition, 0.3f);
        
        foreach (var col in nearbyObjects)
        {
            if (col.gameObject.name.StartsWith("Flag_"))
            {
                selectedFlag = col.gameObject;
                Debug.Log("Bayrak tutuldu: " + selectedFlag.name);
                break;
            }
        }
    }
    
    void TryDropFlag()
{
    Collider[] nearbyObjects = Physics.OverlapSphere(handPosition, 0.4f);
    
    foreach (var col in nearbyObjects)
    {
        if (col.gameObject.name.StartsWith("Slot_"))
        {
            if (gameController != null)
            {
                bool isCorrect = gameController.CheckMatch(selectedFlag.name, col.gameObject.name);
                
                if (isCorrect)
                {
                    selectedFlag.transform.position = col.transform.position;
                    
                    Destroy(selectedFlag.GetComponent<Collider>());
                    
                    Debug.Log("Bayrak kilitlendi!");
                }
                else
                {
                    Debug.Log("Yanlış eşleşme!");
                }
            }
            
            selectedFlag = null;
            return;
        }
    }
    
    selectedFlag = null;
    Debug.Log("Bayrak bırakıldı");

}
}