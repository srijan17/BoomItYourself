using UnityEngine;

public class KeyRotator : MonoBehaviour
{
	[SerializeField] private float rotationSpeed = 90f;
	[SerializeField] private float bobHeight = 0.25f;
	[SerializeField] private float bobFrequency = 1f;

	private Vector3 startPosition;
	private float elapsedTime;
	private float bobPhase;

	private void OnEnable()
	{
		startPosition = transform.localPosition;
		elapsedTime = 0f;
		bobPhase = Random.Range(0f, 2f * Mathf.PI);
	}

	private void Update()
	{
		elapsedTime += Time.deltaTime;
		transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
		transform.localPosition = startPosition + Vector3.up *
			(Mathf.Sin(elapsedTime * bobFrequency * 2f * Mathf.PI + bobPhase) * bobHeight);
	}

	private void OnDisable()
	{
		transform.localPosition = startPosition;
	}
}