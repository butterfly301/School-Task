using UnityEngine;

public class FixPlayerRotation : MonoBehaviour
{
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private PlayerController playerController;
    private PlayerCharacter playerCharacter;

    private void Awake()
    {
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
        playerController = GetComponentInParent<PlayerController>();
        playerCharacter = GetComponentInParent<PlayerCharacter>();
    }

    private void LateUpdate()
    {
        if (playerCharacter != null && !playerCharacter.isAlive)
            return;

        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;
    }

    public void OnAttackAnimationFinished()
    {
        playerController?.OnAttackAnimationFinished();
    }
}
