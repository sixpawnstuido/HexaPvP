using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitPlace : MonoBehaviour
{
    // Adjust this speed value to control the object's movement speed while holding down
    public float followSpeed = 5f;

    // Flag to check if the object should move directly to the mouse on the first click
    private bool moveToMouse = true;

    public Camera cam;

    public GameObject dottedLine;

    public static bool TouchBlock;

    void Update()
    {
        if (TouchBlock) return;
        if (Meta2DManager.spawnedFruit == null) return;

        if (Input.GetMouseButtonDown(0))
        {

            if (Meta2DManager.Instance.secondTutorial.activeInHierarchy) Meta2DManager.Instance.secondTutorial.SetActive(false);
            // Get the current mouse position in world coordinates
            Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

            // Ensure the y-coordinate remains unchanged
            mousePosition.y = transform.position.y;
            mousePosition.z = 0;

            float clampedX = Mathf.Clamp(mousePosition.x, 42f, 45.4f);

            // Move directly to the mouse position
            transform.position = new Vector3(clampedX, mousePosition.y, 0);

            // Set the flag to false to indicate that we should follow the mouse now
            moveToMouse = false;

            if (Meta2DManager.spawnedFruit != null) dottedLine.SetActive(true);
            else dottedLine.SetActive(false);
        }

        // Check if the left mouse button is held down
        if (Input.GetMouseButton(0) && !moveToMouse)
        {
            // Get the current mouse position in world coordinates
            Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

            // Ensure the y-coordinate remains unchanged
            mousePosition.y = transform.position.y;
            mousePosition.z = 0;

            float clampedX = Mathf.Clamp(mousePosition.x, 42f, 45.4f);

            // Move towards the mouse position
            transform.position = Vector3.Lerp(transform.position, new Vector3(clampedX, mousePosition.y, 0), followSpeed * Time.deltaTime);

            if (Meta2DManager.spawnedFruit != null) dottedLine.SetActive(true);
            else dottedLine.SetActive(false);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (Meta2DManager.spawnedFruit == null) return;

            AudioManager.Instance.Play(AudioManager.AudioEnums.FingerTap);
            AudioManager.Instance.PlayHaptic(Lofelt.NiceVibrations.HapticPatterns.PresetType.LightImpact);

            MetaFruit tmpFruit = Meta2DManager.spawnedFruit;

            Meta2DManager.spawnedFruit = null;

            dottedLine.SetActive(false);

            tmpFruit.transform.parent = transform.parent;

            Rigidbody2D rb = tmpFruit.rb;
            Collider2D coll = tmpFruit.coll;

            coll.enabled = true;
            rb.bodyType = RigidbodyType2D.Dynamic;

            tmpFruit.EnableIsFalled();

            Meta2DManager.Instance.SpawnAFruit(.3f);
        }
    }
}
