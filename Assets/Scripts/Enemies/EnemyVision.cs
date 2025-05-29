using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public EnemyType enemyType; // Link to the Scriptable Object
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("No object with the tag 'Player' was found. Make sure it is assigned.");
        }
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false; // Prevent NullReferenceException before calculating distance

        return Vector2.Distance(transform.position, player.position) <= enemyType.detectionRange;
    }
}
