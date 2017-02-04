using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleArrowKeySystem : MonoBehaviour
{
    public Sprite arrowLeftSprite;
    public Sprite arrowRightSprite;
    public Sprite arrowDownSprite;
    public Sprite arrowUpSprite;

    public float errorDuration;
    public float errorSize;
    public float successDuration;
    public float successSize;
    public float fadeAwayDuration;

    public Vector2 arrowPosition;
    public float arrowSize;
    public float arrowGap;
    public List<char> keys = new List<char>();

    private List<GameObject> arrowEntities = new List<GameObject>();
    private int currentKeyIndex;
    private float startTime;
    private bool busy;
    private bool finished;

    public void Begin()
    {
        // reset state
        currentKeyIndex = 0;
        startTime = Time.time;
        busy = false;
        finished = false;

        // cleanup previous entities
        foreach (GameObject go in arrowEntities)
        {
            Destroy(go.gameObject);
        }
        arrowEntities.Clear();

        // generate new entities
        Canvas canvas = FindObjectOfType<Canvas>();

        for (int i = 0; i < keys.Count; i++)
        {
            char key = keys[i];

            GameObject go = Instantiate(Resources.Load("Prefabs/ArrowEntityPrefab")) as GameObject;
            Image image = go.GetComponentInChildren<Image>();
            ArrowEntityBehaviour behaviour = go.GetComponent<ArrowEntityBehaviour>();

            image.rectTransform.sizeDelta = new Vector2(arrowSize, arrowSize);
            go.transform.parent = canvas.transform;
            go.transform.position = new Vector3(arrowPosition.x + (arrowSize + arrowGap) * i, arrowPosition.y);
            behaviour.image = image;
            behaviour.system = this;

            behaviour.errorDuration = errorDuration;
            behaviour.errorSize = errorSize;
            behaviour.successDuration = successDuration;
            behaviour.successSize = successSize;
            behaviour.fadeAwayDuration = fadeAwayDuration;

            arrowEntities.Add(go);

            switch (key)
            {
                case '<':
                    {
                        image.sprite = arrowLeftSprite;
                        break;
                    }
                case '>':
                    {
                        image.sprite = arrowRightSprite;
                        break;
                    }
                case 'v':
                    {
                        image.sprite = arrowDownSprite;
                        break;
                    }
                case '^':
                    {
                        image.sprite = arrowUpSprite;
                        break;
                    }
            }
        }

        Debug.Log("[BattleArrowKeySystem] Beginning new round.");
        Debug.Log("[BattleArrowKeySystem] startTime = " + startTime);
    }

    public void End()
    {
        finished = true;
        float timePassed = Time.time - startTime;

        Debug.Log("[BattleArrowKeySystem] Ending round.");
        Debug.Log("[BattleArrowKeySystem] ~> Time passed: " + timePassed);
    }

    public void Step(char key)
    {
        if (busy || finished)
        {
            return;
        }

        busy = true;

        char currentKey = keys[currentKeyIndex];
        bool success = key == currentKey;

        Debug.Log("[BattleArrowKeySystem] Attempting a step");
        Debug.Log("[BattleArrowKeySystem] ~> Provided key: " + key);
        Debug.Log("[BattleArrowKeySystem] ~> Required key: " + currentKey);
        Debug.Log("[BattleArrowKeySystem] ~> Successful? " + (success ? "Yes!" : "No."));

        if (success)
        {
            int actorIndex = currentKeyIndex;

            currentKeyIndex++;
            if (currentKeyIndex == keys.Count)
            {
                End();
            }

            for (int i = 0; i < arrowEntities.Count; i++)
            {
                GameObject go = arrowEntities[i];
                ArrowEntityBehaviour behaviour = go.GetComponent<ArrowEntityBehaviour>();

                if (i == actorIndex)
                {
                    behaviour.ActSuccess();
                }
                else
                {
                    behaviour.ActMoveOver();
                }
            }
        }
        else
        {
            GameObject go = arrowEntities[currentKeyIndex];
            ArrowEntityBehaviour behaviour = go.GetComponent<ArrowEntityBehaviour>();

            behaviour.ActError();
        }
    }

    public void Unbusy()
    {
        Debug.Log("[BattleArrowKeySystem] Unbusy() !");
        busy = false;

        if (finished)
        {
            Debug.Log("[BattleArrowKeySystem] finished!");

            for (int i = 0; i < arrowEntities.Count; i++)
            {
                GameObject go = arrowEntities[i];
                ArrowEntityBehaviour behaviour = go.GetComponent<ArrowEntityBehaviour>();

                behaviour.ActFadeAway();
            }
        }
    }

    // FOR TESTING ONLY
    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            Begin();
        }
        if (Input.GetKeyDown("up"))
        {
            Step('^');
        }
        if (Input.GetKeyDown("down"))
        {
            Step('v');
        }
        if (Input.GetKeyDown("left"))
        {
            Step('<');
        }
        if (Input.GetKeyDown("right"))
        {
            Step('>');
        }
    }

    // FOR TESTING ONLY
    void Start()
    {
        Begin();
    }
}
