using UnityEngine;

public class RotationController : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 50, 0); // Y ekseninde dön
   
    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}