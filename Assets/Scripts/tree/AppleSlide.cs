using UnityEngine;

public class AppleSlide : MonoBehaviour
{
    public float slideDistance = 2f;    // The distance the apple slides left and right.
    public float slideSpeed = 2f;      // Base speed of the sliding motion.
    private Vector3 startPosition;     // The initial position of the apple.
    private float randomTimeOffset;    // A random offset to desynchronize movement.

    void Start()
    {
        // Store the apple's starting position.
        startPosition = transform.position;

        // Generate a random time offset for each apple.
        randomTimeOffset = Random.Range(0f, Mathf.PI * 2);
    }

    void Update()
    {
        // Calculate the sliding movement with random offset.
        float offset = Mathf.Sin(Time.time * slideSpeed + randomTimeOffset) * slideDistance;
        transform.position = startPosition + new Vector3(offset, 0, 0);
    }
}
