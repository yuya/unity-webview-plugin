using UnityEngine;

public class Box : MonoBehaviour {
    void Update() {
        if (transform.position.y < -2.0) {
            Destroy(gameObject);
        }
    }
}
