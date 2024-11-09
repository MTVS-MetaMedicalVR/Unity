// ProcedureTypes.cs
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SEUGNEON
{

    public enum ProcedureInteractionType
    {
        Movement,      // 특정 영역으로 이동
        Outline,       // 아웃라인이 있는 오브젝트와 상호작용
        Animation,     // 애니메이션 실행
        ParticleTimer  // 파티클 효과와 타이머
    }

    // ProcedureData.cs
    [System.Serializable]
    public class ProcedureList
    {
        public List<ProcedureCategory> categories;
    }

    [System.Serializable]
    public class ProcedureCategory
    {
        public string categoryId;
        public string categoryName;
        public List<ProcedureData> procedures;
    }

    [System.Serializable]
    public class ProcedureData
    {
        public string id;
        public string name;
        public string description;
    }

    // ProcedureInteractionConfig.cs
    [System.Serializable]
    public class ProcedureInteractionConfig
    {
        public ProcedureInteractionType interactionType;
        public float highlightBlinkRate = 1f;     // 깜빡임 속도
        public float timerDuration = 0f;          // 타이머 지속 시간
        public string animationTriggerName;       // 애니메이션 트리거 이름
        public Material highlightMaterial;        // 하이라이트 매터리얼
        public ParticleSystem particleEffect;     // 파티클 시스템
    }

    // ProcedureObjectBase.cs
    public class ProcedureObjectBase : MonoBehaviour
    {
        public string procedureId;  // JSON의 id와 매칭
        public bool isDone = false;
        public ProcedureData procedure;

        [SerializeField]
        protected ProcedureInteractionConfig interactionConfig;
        protected Material originalMaterial;
        protected MeshRenderer meshRenderer;
        protected Outline outlineComponent;
        protected Animator animator;
        protected bool isBlinking = false;
        protected Coroutine blinkCoroutine;

        protected virtual void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            outlineComponent = GetComponent<Outline>();
            animator = GetComponent<Animator>();
            if (meshRenderer)
                originalMaterial = meshRenderer.material;
        }

        protected virtual void OnEnable()
        {
            StartInteractionFeedback();
        }

        protected virtual void OnDisable()
        {
            StopInteractionFeedback();
        }

        protected virtual void StartInteractionFeedback()
        {
            switch (interactionConfig.interactionType)
            {
                case ProcedureInteractionType.Movement:
                    if (meshRenderer && interactionConfig.highlightMaterial)
                        blinkCoroutine = StartCoroutine(BlinkMaterial());
                    break;
                case ProcedureInteractionType.Outline:
                    if (outlineComponent)
                        blinkCoroutine = StartCoroutine(BlinkOutline());
                    break;
            }
        }

        protected virtual void StopInteractionFeedback()
        {
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }
            if (meshRenderer)
                meshRenderer.material = originalMaterial;
            if (outlineComponent)
                outlineComponent.enabled = false;
        }

        protected IEnumerator BlinkMaterial()
        {
            while (!isDone)
            {
                meshRenderer.material = interactionConfig.highlightMaterial;
                yield return new WaitForSeconds(interactionConfig.highlightBlinkRate * 0.5f);
                meshRenderer.material = originalMaterial;
                yield return new WaitForSeconds(interactionConfig.highlightBlinkRate * 0.5f);
            }
        }

        protected IEnumerator BlinkOutline()
        {
            while (!isDone)
            {
                outlineComponent.enabled = true;
                yield return new WaitForSeconds(interactionConfig.highlightBlinkRate * 0.5f);
                outlineComponent.enabled = false;
                yield return new WaitForSeconds(interactionConfig.highlightBlinkRate * 0.5f);
            }
        }

        protected virtual void CompleteInteraction()
        {
            StopInteractionFeedback();
            isDone = true;
            gameObject.SetActive(false);
        }
    }

    // ProcedureAnimation.cs
    public class ProcedureAnimation : ProcedureObjectBase
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("PlayerHand") || isDone) return;
            if (animator && !string.IsNullOrEmpty(interactionConfig.animationTriggerName))
            {
                animator.SetTrigger(interactionConfig.animationTriggerName);
                StartCoroutine(WaitForAnimationComplete());
            }
        }

        private IEnumerator WaitForAnimationComplete()
        {
            yield return new WaitForSeconds(0.1f);
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                yield return null;
            }
            CompleteInteraction();
        }
    }

    // ProcedureMovement.cs
    public class ProcedureMovement : ProcedureObjectBase
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || isDone) return;
            CompleteInteraction();
        }
    }

    // ProcedureParticleTimer.cs
    public class ProcedureParticleTimer : ProcedureObjectBase
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("PlayerHand") || isDone) return;
            if (interactionConfig.particleEffect)
            {
                interactionConfig.particleEffect.Play();
                StartCoroutine(WaitForTimer());
            }
        }

        private IEnumerator WaitForTimer()
        {
            yield return new WaitForSeconds(interactionConfig.timerDuration);
            if (interactionConfig.particleEffect)
                interactionConfig.particleEffect.Stop();
            CompleteInteraction();
        }
    }

    // ProcedureShared.cs
    public class ProcedureShared : ProcedureObjectBase
    {
        [SerializeField]
        private string[] validProcedureIds; // 이 오브젝트가 반응할 수 있는 프로시저 ID 목록

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("PlayerHand")) return;
            string currentProcedureId = ProcedureManager.Instance.GetCurrentProcedureId();
            if (System.Array.Exists(validProcedureIds, id => id == currentProcedureId))
            {
                if (animator && !string.IsNullOrEmpty(interactionConfig.animationTriggerName))
                {
                    animator.SetTrigger(interactionConfig.animationTriggerName);
                    StartCoroutine(WaitForAnimationComplete());
                }
                else
                {
                    CompleteInteraction();
                }
            }
        }

        private IEnumerator WaitForAnimationComplete()
        {
            yield return new WaitForSeconds(0.1f);
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                yield return null;
            }
            CompleteInteraction();
        }
    }

    // ProcedureManager.cs
    public class ProcedureManager : MonoBehaviour
    {
        public static ProcedureManager Instance { get; private set; }

        public ProcedureList procedureData;
        public static readonly string fileName = "procedures.json";

        [Header("UI")]
        public Canvas procedureUI;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public Toggle confirmButton;

        private List<ProcedureObjectBase> orderedProcedureObjects = new List<ProcedureObjectBase>();
        private int currentProcedureIndex = 0;
        private bool isWaitingForConfirm = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadProcedureData();
                ArrangeProcedureObjects();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            confirmButton?.onValueChanged.AddListener(OnConfirmButtonClick);
            StartNextProcedure();
        }

        public string GetCurrentProcedureId()
        {
            if (currentProcedureIndex < orderedProcedureObjects.Count)
            {
                return orderedProcedureObjects[currentProcedureIndex].procedureId;
            }
            return string.Empty;
        }

        private void LoadProcedureData()
        {
            string path = Path.Combine(Application.streamingAssetsPath, fileName);
            if (File.Exists(path))
            {
                string jsonContent = File.ReadAllText(path);
                procedureData = JsonUtility.FromJson<ProcedureList>(jsonContent);
            }
        }

        private void ArrangeProcedureObjects()
        {
            var allProcedureObjects = FindObjectsOfType<ProcedureObjectBase>();
            orderedProcedureObjects.Clear();

            if (procedureData?.categories != null)
            {
                foreach (var category in procedureData.categories)
                {
                    foreach (var procedure in category.procedures)
                    {
                        var matchingObject = System.Array.Find(allProcedureObjects,
                            obj => obj.procedureId == procedure.id);

                        if (matchingObject != null)
                        {
                            matchingObject.procedure = procedure;
                            orderedProcedureObjects.Add(matchingObject);
                        }
                    }
                }
            }
        }

        void Update()
        {
            if (!isWaitingForConfirm && currentProcedureIndex < orderedProcedureObjects.Count)
            {
                if (orderedProcedureObjects[currentProcedureIndex].isDone)
                {
                    CompleteCurrentProcedure();
                }
            }
        }

        private void StartNextProcedure()
        {
            if (currentProcedureIndex >= orderedProcedureObjects.Count)
            {
                Debug.Log("모든 절차 완료");
                return;
            }

            var currentProcedure = orderedProcedureObjects[currentProcedureIndex].procedure;
            ShowProcedureUI(currentProcedure.name, currentProcedure.description);
            orderedProcedureObjects[currentProcedureIndex].gameObject.SetActive(true);
            isWaitingForConfirm = true;
        }

        private void ShowProcedureUI(string title, string description)
        {
            if (procedureUI != null)
            {
                procedureUI.gameObject.SetActive(true);
                confirmButton.isOn = false;
                titleText.text = title;
                descriptionText.text = description;
            }
        }

        private void OnConfirmButtonClick(bool isOn)
        {
            if (!isWaitingForConfirm) return;

            procedureUI.gameObject.SetActive(false);
            isWaitingForConfirm = false;
        }

        private void CompleteCurrentProcedure()
        {
            currentProcedureIndex++;
            StartNextProcedure();
        }
    }
}