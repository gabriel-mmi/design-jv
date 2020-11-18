using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public float speed;

    private void FixedUpdate ()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
 
        Vector3 newPos = target.position + (Vector3.ClampMagnitude((Vector3)mousePosition * 0.08f, 10f));
        newPos.z = -10;

        transform.position = Vector3.Lerp(transform.position, newPos, speed * Time.fixedDeltaTime);
    }
}
