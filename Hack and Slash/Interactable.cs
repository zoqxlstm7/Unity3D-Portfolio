using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;           //인터랙트 가능 범위
    public Transform interactionPoint;  //인터랙트가 일어날 지점

    Transform player;    

    bool isFocus;                       //인터랙트가 되었는지 유무

    float betweenCheckTime = 0.25f;
    float checkTime;

    public virtual void Interact()
    {
        //Debug.Log("interacting with " + transform.name);
    }

    private void Update()
    {
        if (Time.time > checkTime)
        {
            if (isFocus)
            {
                float distance = Vector3.Distance(player.position, interactionPoint.position);

                if (distance <= radius)
                {
                    Interact();
                }
            }

            checkTime = Time.time + betweenCheckTime;
        }
    }

    public void OnFocused(Transform playerTransform)
    {
        isFocus = true;
        player = playerTransform;
    }

    public void OnDeFocused()
    {
        isFocus = false;
        player = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionPoint.position, radius);
    }
}
