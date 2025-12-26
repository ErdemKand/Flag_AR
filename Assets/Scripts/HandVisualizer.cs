using UnityEngine;

public class HandVisualizer : MonoBehaviour
{
    public HandDataReceiver handDataReceiver;
    public GameObject landmarkPrefab;
    
    private GameObject[] leftHandObjects = new GameObject[21];
    private GameObject[] rightHandObjects = new GameObject[21];
    
    private LineRenderer[] leftHandLines = new LineRenderer[21];
    private LineRenderer[] rightHandLines = new LineRenderer[21];
    
    private int[][] handConnections = new int[][] {
        new int[] {0, 1}, new int[] {1, 2}, new int[] {2, 3}, new int[] {3, 4},
        new int[] {0, 5}, new int[] {5, 6}, new int[] {6, 7}, new int[] {7, 8},
        new int[] {5, 9}, new int[] {9, 10}, new int[] {10, 11}, new int[] {11, 12},
        new int[] {9, 13}, new int[] {13, 14}, new int[] {14, 15}, new int[] {15, 16},
        new int[] {13, 17}, new int[] {0, 17}, new int[] {17, 18}, new int[] {18, 19}, new int[] {19, 20}
    };
    
    void Start()
    {
        for (int i = 0; i < 21; i++)
        {
            leftHandObjects[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftHandObjects[i].transform.localScale = Vector3.one * 0.2f;
            leftHandObjects[i].GetComponent<Renderer>().material.color = Color.green;
            leftHandObjects[i].SetActive(false);
            
            rightHandObjects[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightHandObjects[i].transform.localScale = Vector3.one * 0.2f;
            rightHandObjects[i].GetComponent<Renderer>().material.color = Color.red;
            rightHandObjects[i].SetActive(false);
        }
        
        for (int i = 0; i < 21; i++)
        {
            GameObject lineObjLeft = new GameObject("LeftHandLine" + i);
            leftHandLines[i] = lineObjLeft.AddComponent<LineRenderer>();
            leftHandLines[i].material = new Material(Shader.Find("Sprites/Default"));
            leftHandLines[i].startColor = Color.green;
            leftHandLines[i].endColor = Color.green;
            leftHandLines[i].startWidth = 0.05f;
            leftHandLines[i].endWidth = 0.05f;
            leftHandLines[i].positionCount = 2;
            leftHandLines[i].enabled = false;
            
            GameObject lineObjRight = new GameObject("RightHandLine" + i);
            rightHandLines[i] = lineObjRight.AddComponent<LineRenderer>();
            rightHandLines[i].material = new Material(Shader.Find("Sprites/Default"));
            rightHandLines[i].startColor = Color.red;
            rightHandLines[i].endColor = Color.red;
            rightHandLines[i].startWidth = 0.05f;
            rightHandLines[i].endWidth = 0.05f;
            rightHandLines[i].positionCount = 2;
            rightHandLines[i].enabled = false;
        }
    }
    
    void Update()
    {
        if (handDataReceiver == null || handDataReceiver.currentHandData == null)
            return;
        
        foreach (var obj in leftHandObjects) obj.SetActive(false);
        foreach (var obj in rightHandObjects) obj.SetActive(false);
        foreach (var line in leftHandLines) line.enabled = false;
        foreach (var line in rightHandLines) line.enabled = false;
        
        foreach (var hand in handDataReceiver.currentHandData.hands)
        {
            GameObject[] handObjects = hand.handedness == "Left" ? leftHandObjects : rightHandObjects;
            LineRenderer[] handLines = hand.handedness == "Left" ? leftHandLines : rightHandLines;
            
            for (int i = 0; i < hand.landmarks.Length && i < 21; i++)
            {
                var landmark = hand.landmarks[i];                
                float x = (landmark.x - 0.5f) * 12f;  
                float y = -(landmark.y - 0.5f) * 12f; 
                float z = -landmark.z * 3f;
                
                handObjects[i].transform.position = new Vector3(x, y, z);
                handObjects[i].SetActive(true);
            }
            
            for (int i = 0; i < handConnections.Length; i++)
            {
                int start = handConnections[i][0];
                int end = handConnections[i][1];
                
                if (start < hand.landmarks.Length && end < hand.landmarks.Length)
                {
                    handLines[i].SetPosition(0, handObjects[start].transform.position);
                    handLines[i].SetPosition(1, handObjects[end].transform.position);
                    handLines[i].enabled = true;
                }
            }
        }
    }
}