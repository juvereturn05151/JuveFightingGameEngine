using FightingGameEngine;
using System.Collections.Generic;
using UnityEngine;

public class CollisionVisualizer : MonoBehaviour
{
    [SerializeField]
    private Fighter fighter;

    [SerializeField]
    private GameObject pushboxGO;
    [SerializeField]
    private List<GameObject> hurtboxGOs;
    [SerializeField]
    private List<GameObject> hitboxGOs;
    [SerializeField]
    private List<GameObject> grabboxGOs;

    private void Update()
    {
        UpdateBoxes(fighter.pushbox, fighter.hurtboxes, fighter.hitboxes, fighter.grabboxes);
    }

    public void UpdateBoxes(Pushbox pushbox, List<Hurtbox> hurtboxes, List<Hitbox> hitboxes, List<Grabbox> grabboxes)
    {
        // --- Update Pushbox ---
        if (pushbox != null && pushboxGO != null)
        {
            UpdateBoxPosition(pushboxGO, pushbox.rect.position, pushbox.rect.size);
            pushboxGO.SetActive(true);
        }
        else
        {
            pushboxGO?.SetActive(false);
        }

        // --- Update Hurtboxes ---
        for (int i = 0; i < hurtboxGOs.Count; i++)
        {
            if (i < hurtboxes.Count)
            {
                UpdateBoxPosition(hurtboxGOs[i], hurtboxes[i].rect.position, hurtboxes[i].rect.size);
                hurtboxGOs[i].SetActive(true);
            }
            else
            {
                hurtboxGOs[i].SetActive(false);
            }
        }

        // --- Update Hitboxes ---
        for (int i = 0; i < hitboxGOs.Count; i++)
        {
            if (i < hitboxes.Count)
            {
                UpdateBoxPosition(hitboxGOs[i], hitboxes[i].rect.position, hitboxes[i].rect.size);
                hitboxGOs[i].SetActive(true);
            }
            else
            {
                hitboxGOs[i].SetActive(false);
            }
        }

        // --- Update Grabboxes ---
        for (int i = 0; i < grabboxGOs.Count; i++)
        {
            if (i < grabboxes.Count)
            {
                UpdateBoxPosition(grabboxGOs[i], grabboxes[i].rect.position, grabboxes[i].rect.size);
                grabboxGOs[i].SetActive(true);
            }
            else
            {
                grabboxGOs[i].SetActive(false);
            }
        }
    }

    private void UpdateBoxPosition(GameObject boxGO, Vector2 position, Vector2 size)
    {
        boxGO.transform.position = new Vector3(position.x, position.y, boxGO.transform.position.z);
        boxGO.transform.localScale = new Vector3(size.x, size.y, 1f);
    }
}
