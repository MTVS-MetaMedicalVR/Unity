using UnityEngine;
using System.Collections.Generic;

public class BlendShapeController : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer; // SkinnedMeshRenderer 연결

    // Blend Shape 데이터 관리
    [System.Serializable]
    public class BlendShapeGroup
    {
        public string groupName; // 그룹 이름 (예: "Eye Blink")
        public List<string> blendShapeNames; // 그룹에 포함된 Blend Shape 이름들
        public float maxWeight = 100f; // 최대 Weight (예: 눈을 완전히 감는 값)
        public float minWeight = 0f; // 최소 Weight (예: 눈을 완전히 뜨는 값)
        public float duration = 0.2f; // 애니메이션 지속 시간 (초 단위)
        public bool loop = false; // 반복 여부
        public float interval = 3f; // 반복 간격 (loop가 true일 경우에만 사용)
    }

    [SerializeField] private List<BlendShapeGroup> blendShapeGroups = new List<BlendShapeGroup>(); // Blend Shape 그룹 리스트

    private Dictionary<string, List<int>> blendShapeIndices = new Dictionary<string, List<int>>(); // Blend Shape 그룹의 인덱스 매핑
    private Dictionary<string, float> timers = new Dictionary<string, float>(); // 그룹별 타이머 관리

    private void Start()
    {
        // Blend Shape 그룹 초기화
        foreach (var group in blendShapeGroups)
        {
            List<int> indices = new List<int>();

            foreach (var shapeName in group.blendShapeNames)
            {
                int index = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(shapeName);
                if (index != -1)
                {
                    indices.Add(index); // 유효한 Blend Shape 인덱스를 추가
                }
                else
                {
                    Debug.LogError($"Blend Shape '{shapeName}' not found in group '{group.groupName}'!");
                }
            }

            if (indices.Count > 0)
            {
                blendShapeIndices[group.groupName] = indices; // 그룹 이름으로 인덱스 리스트를 저장
                timers[group.groupName] = Random.Range(0f, group.interval); // 타이머 초기화
            }
        }
    }

    private void Update()
    {
        // 각 그룹에 대해 타이머와 애니메이션 처리
        foreach (var group in blendShapeGroups)
        {
            if (!blendShapeIndices.ContainsKey(group.groupName)) continue;

            timers[group.groupName] += Time.deltaTime;

            // 반복 애니메이션 처리
            if (group.loop && timers[group.groupName] >= group.interval)
            {
                timers[group.groupName] = 0f; // 타이머 초기화
                StartBlendShapeGroupAnimation(group);
            }

            UpdateBlendShapeGroupWeight(group, timers[group.groupName]);
        }
    }

    // Blend Shape 그룹 애니메이션 시작
    private void StartBlendShapeGroupAnimation(BlendShapeGroup group)
    {
        foreach (var index in blendShapeIndices[group.groupName])
        {
            skinnedMeshRenderer.SetBlendShapeWeight(index, group.maxWeight); // 모든 Blend Shape를 최대 Weight로 설정
        }
    }

    // Blend Shape 그룹 Weight 업데이트
    private void UpdateBlendShapeGroupWeight(BlendShapeGroup group, float timer)
    {
        if (timer < group.duration)
        {
            // 진행 중 (Weight를 최대값에서 최소값으로 선형 보간)
            float weight = Mathf.Lerp(group.maxWeight, group.minWeight, timer / group.duration);
            foreach (var index in blendShapeIndices[group.groupName])
            {
                skinnedMeshRenderer.SetBlendShapeWeight(index, weight);
            }
        }
        else
        {
            // 애니메이션 종료 시 Weight를 최소값으로 유지
            foreach (var index in blendShapeIndices[group.groupName])
            {
                skinnedMeshRenderer.SetBlendShapeWeight(index, group.minWeight);
            }
        }
    }
}
