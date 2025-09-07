using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform moving_object;
    public float speed = 20f;


    private Joystick controller;
    private void Start()
    {
        controller = this.GetComponent<Joystick>();
    }

    private void FixedUpdate()
    {
        Vector3 moveDir = new Vector3(controller.Vertical, 0, controller.Horizontal);

        if (moveDir == Vector3.zero) return;

        moving_object.rotation = Quaternion.LookRotation(moveDir);
        moving_object.Translate(Vector3.forward * Time.fixedDeltaTime * speed);
    }
}