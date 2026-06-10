using UnityEngine;

public class PngPongRotate : MonoBehaviour
{
    public float maxAngle = 30f; // 最大旋转角�?
        public float speed = 1f;     // 旋转速度
        
        private float initialYRotation;
        
        void Start()
        {
            initialYRotation = transform.eulerAngles.y;
        }
        
        void Update()
        {
            float angle = Mathf.PingPong(Time.time * speed, maxAngle);
            transform.rotation = Quaternion.Euler(0f, initialYRotation + angle, 0f);
        }
}
