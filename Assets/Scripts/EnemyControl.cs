using UnityEngine;
public class EnemyControl : MonoBehaviour
{
    [Header("Movimiento del Enemigo")]
    public Vector2 positionToMove;
    public float speedMove;
    [Header("Vida del Enemigo")]
    public float energy = 50f;
    public float maxEnergy = 50f;
    public float timeNewEnergy = 5f;
    public float newEnergy = 50f;
    public bool isResting = false;
    private float restTimer = 0f;
    private NodoControl currentNode;
    public MyList<NodoControl> allNodes;
    [Header("Seguimiento al Jugador")]
    public float rango = 5f; 
    public float angle = 60f; 
    public Transform player;     
    private bool isChasing = false; 
    void Update()
    {
        if (isResting)
        {
            restTimer -= Time.deltaTime;
            if (restTimer <= 0f)
            {
                RecoverEnergy();
            }
        }
        else if (isChasing)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speedMove * Time.deltaTime);
            energy -= Time.deltaTime * 2f; 
            if (energy <= 0f)
            {
                Resting(); 
            }
            else
            {
                CanIChase(); 
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, positionToMove, speedMove * Time.deltaTime);
            CanIChase();
        }
    }
    public void SetNewPosition(Vector2 newPosition)
    {
        positionToMove = newPosition;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Nodo"))
        {
            currentNode = collision.GetComponent<NodoControl>();
            if (currentNode != null && !isChasing)
            {
                AdjacentNodeInfo nextNodeInfo = currentNode.MovedAdjacentNode();
                if (energy >= nextNodeInfo.weight)
                {
                    SetNewPosition(nextNodeInfo.node.transform.position);
                    energy = energy - nextNodeInfo.weight;
                    Debug.Log("Energía: " + energy);
                }
                else
                {
                    Resting();
                }
            }
        }
    }
    private void Resting()
    {
        isResting = true;
        restTimer = timeNewEnergy;
        Debug.Log("Enemigo zzzz");
    }
    private void RecoverEnergy()
    {
        energy = Mathf.Min(energy + newEnergy, maxEnergy);
        if (energy >= maxEnergy)
        {
            isResting = false;
            Debug.Log("Energía fina: " + energy);

            if (allNodes != null && allNodes.Count() > 0)
            {
                NodoControl nextNode = MoveNextNode();
                if (nextNode != null)
                {
                    SetNewPosition(nextNode.transform.position);
                }
            }
        }
    }
    private NodoControl MoveNextNode()
    {
        for (int i = 0; i < currentNode.adjacentNodes.Count(); i++)
        {
            NodoControl node = currentNode.adjacentNodes.Get(i);
            if (node != currentNode)
            {
                return node;
            }
        }
        return null;
    }
    private void CanIChase()
    {
        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;
        Debug.DrawRay(transform.position, direction.normalized * rango, Color.green);

        if (distance <= rango)
        {
            float angleToPlayer = Vector3.Angle(Vector2.right, direction);
            if (angleToPlayer < angle / 2f)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rango);
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    isChasing = true;
                    return;
                }
            }
        }
        if (isChasing && distance > rango)
        {
            isChasing = false;
            ReturnToPatrol();  
        }
    }
    private void ReturnToPatrol()
    {
        if (allNodes != null && allNodes.Count() > 0)
        {
            NodoControl nextNode = MoveNextNode();
            if (nextNode != null)
            {
                SetNewPosition(nextNode.transform.position);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rango);
        Vector3 forward = transform.up; 
        Vector3 left = Quaternion.Euler(0, 0, angle / 2) * forward;
        Vector3 right = Quaternion.Euler(0, 0, -angle / 2) * forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + left * rango);
        Gizmos.DrawLine(transform.position, transform.position + right * rango);
    }
}