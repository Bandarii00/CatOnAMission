using UnityEngine;

public class GameController : MonoBehaviour
{
    public Transform teleportTarget; 
    public EnemyAI enemyAI;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                other.transform.position = teleportTarget.position;
                controller.enabled = true;

                if (enemyAI != null)
                {
                    enemyAI.ResetChase(); 
                }
            }
        }
    }
}
