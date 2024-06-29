using UnityEngine;

public class TrapTrampoline : MonoBehaviour
{
    public float jumpForce = 10f;  // Độ mạnh của lực đẩy ban đầu
    public float decayRate = 5f;   // Tốc độ suy giảm của lực đẩy

    private CharacterController controller;
    private float currentJumpForce;  // Lực đẩy hiện tại

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Lấy component CharacterController của Player
            controller = other.GetComponent<CharacterController>();

            // Kiểm tra nếu Player có CharacterController
            if (controller != null)
            {
                // Khởi tạo lực đẩy
                currentJumpForce = jumpForce;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        currentJumpForce = 0;
    }

    private void Update()
    {
        if (currentJumpForce > 0 && controller != null)
        {
            // Áp dụng lực đẩy theo thời gian
            Vector3 jumpVector = new Vector3(currentJumpForce, 0, 0);
            controller.Move(jumpVector * Time.deltaTime);

            // Giảm lực đẩy dần
            currentJumpForce -= decayRate * Time.deltaTime;
        }
    }
}
