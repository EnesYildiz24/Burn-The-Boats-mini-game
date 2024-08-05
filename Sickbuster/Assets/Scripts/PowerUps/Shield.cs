using UnityEngine;

public class Shield : MonoBehaviour
{
    public Transform playerTransform;
    public float followSpeed = 5f; 

    void Update()
    {
        if (playerTransform != null)
        {
            Vector3 targetPosition = playerTransform.position;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}
