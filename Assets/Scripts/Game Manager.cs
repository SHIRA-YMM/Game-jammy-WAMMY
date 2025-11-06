using UnityEngine;
using System.Collections.Generic;

public class MultiNPCBehavior : MonoBehaviour
{
    [System.Serializable]
    public class NPCData
    {
        public string npcName;
        public Transform npcTransform;
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        public GameObject interactionUI;

        [Header("Movement Settings")]
        public float moveSpeed = 2f;
        public float movementRange = 3f;
        public float idleTime = 2f;
        public bool canMove = true;

        // Runtime variables
        [HideInInspector] public Vector3 startPosition;
        [HideInInspector] public float currentTarget;
        [HideInInspector] public int facing = 1;
        [HideInInspector] public bool isIdle = false;
        [HideInInspector] public float idleTimer = 0f;
        [HideInInspector] public bool isInteracting = false;
        [HideInInspector] public Transform playerTransform;
    }

    [Header("NPC List")]
    [SerializeField] private List<NPCData> npcs = new List<NPCData>();

    void Start()
    {
        InitializeAllNPCs();
    }

    private void InitializeAllNPCs()
    {
        foreach (NPCData npc in npcs)
        {
            if (npc.npcTransform != null)
            {
                npc.startPosition = npc.npcTransform.position;
                npc.currentTarget = npc.startPosition.x + npc.movementRange;

                if (npc.interactionUI != null)
                    npc.interactionUI.SetActive(false);
            }
        }
    }

    void Update()
    {
        foreach (NPCData npc in npcs)
        {
            if (npc.npcTransform != null && npc.canMove && !npc.isInteracting)
            {
                UpdateNPCMovement(npc);
            }
            else if (npc.isInteracting && npc.playerTransform != null)
            {
                // Face player during interaction
                npc.facing = (npc.playerTransform.position.x > npc.npcTransform.position.x) ? 1 : -1;
                UpdateNPCAnimation(npc, true, false);
            }
        }
    }

    private void UpdateNPCMovement(NPCData npc)
    {
        if (npc.isIdle)
        {
            npc.idleTimer += Time.deltaTime;
            if (npc.idleTimer >= npc.idleTime)
            {
                npc.isIdle = false;
                npc.idleTimer = 0f;
                SetNextTarget(npc);
            }
            UpdateNPCAnimation(npc, true, false);
            return;
        }

        float step = npc.moveSpeed * Time.deltaTime;
        Vector3 currentPos = npc.npcTransform.position;

        if (Mathf.Abs(currentPos.x - npc.currentTarget) < 0.1f)
        {
            npc.isIdle = true;
            npc.idleTimer = 0f;
            SetNextTarget(npc);
            UpdateNPCAnimation(npc, true, false);
            return;
        }

        float newX = Mathf.MoveTowards(currentPos.x, npc.currentTarget, step);
        bool movedThisFrame = !Mathf.Approximately(newX, currentPos.x);

        if (movedThisFrame)
            npc.npcTransform.position = new Vector3(newX, currentPos.y, currentPos.z);

        if (movedThisFrame)
        {
            npc.facing = (newX > currentPos.x) ? 1 : -1;
        }

        UpdateNPCAnimation(npc, false, movedThisFrame);
    }

    private void SetNextTarget(NPCData npc)
    {
        float currentX = npc.npcTransform.position.x;

        if (Mathf.Abs(currentX - (npc.startPosition.x + npc.movementRange)) < 0.1f)
        {
            npc.currentTarget = npc.startPosition.x - npc.movementRange;
            npc.facing = -1;
        }
        else
        {
            npc.currentTarget = npc.startPosition.x + npc.movementRange;
            npc.facing = 1;
        }
    }

    private void UpdateNPCAnimation(NPCData npc, bool forceIdle, bool actuallyMoving)
    {
        if (npc.animator != null)
        {
            bool isMoving = !forceIdle && actuallyMoving;
            npc.animator.SetBool("isMoving", isMoving);
            npc.animator.SetInteger("Direction", npc.facing);
        }
    }

    // Public methods to control specific NPCs
    public void SetNPCMovement(string npcName, bool canMove)
    {
        NPCData npc = npcs.Find(n => n.npcName == npcName);
        if (npc != null)
            npc.canMove = canMove;
    }

    public void StartNPCInteraction(string npcName, Transform player)
    {
        NPCData npc = npcs.Find(n => n.npcName == npcName);
        if (npc != null)
        {
            npc.isInteracting = true;
            npc.playerTransform = player;

            if (npc.interactionUI != null)
                npc.interactionUI.SetActive(true);
        }
    }

    public void EndNPCInteraction(string npcName)
    {
        NPCData npc = npcs.Find(n => n.npcName == npcName);
        if (npc != null)
        {
            npc.isInteracting = false;
            npc.playerTransform = null;

            if (npc.interactionUI != null)
                npc.interactionUI.SetActive(false);
        }
    }
}