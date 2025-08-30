using UnityEngine;

public class TestForce : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _rigidbody;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            Debug.Log("Good");
            _rigidbody.linearVelocity = Vector2.left * 20;
        }    
    }
}
