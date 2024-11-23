using UnityEngine;
using System.Collections.Generic;

public class BlendShapeController : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer; // SkinnedMeshRenderer ����

    // Blend Shape ������ ����
    [System.Serializable]
    public class BlendShapeGroup
    {
        public string groupName; // �׷� �̸� (��: "Eye Blink")
        public List<string> blendShapeNames; // �׷쿡 ���Ե� Blend Shape �̸���
        public float maxWeight = 100f; // �ִ� Weight (��: ���� ������ ���� ��)
        public float minWeight = 0f; // �ּ� Weight (��: ���� ������ �ߴ� ��)
        public float duration = 0.2f; // �ִϸ��̼� ���� �ð� (�� ����)
        public bool loop = false; // �ݺ� ����
        public float interval = 3f; // �ݺ� ���� (loop�� true�� ��쿡�� ���)
    }

    [SerializeField] private List<BlendShapeGroup> blendShapeGroups = new List<BlendShapeGroup>(); // Blend Shape �׷� ����Ʈ

    private Dictionary<string, List<int>> blendShapeIndices = new Dictionary<string, List<int>>(); // Blend Shape �׷��� �ε��� ����
    private Dictionary<string, float> timers = new Dictionary<string, float>(); // �׷캰 Ÿ�̸� ����

    private void Start()
    {
        // Blend Shape �׷� �ʱ�ȭ
        foreach (var group in blendShapeGroups)
        {
            List<int> indices = new List<int>();

            foreach (var shapeName in group.blendShapeNames)
            {
                int index = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(shapeName);
                if (index != -1)
                {
                    indices.Add(index); // ��ȿ�� Blend Shape �ε����� �߰�
                }
                else
                {
                    Debug.LogError($"Blend Shape '{shapeName}' not found in group '{group.groupName}'!");
                }
            }

            if (indices.Count > 0)
            {
                blendShapeIndices[group.groupName] = indices; // �׷� �̸����� �ε��� ����Ʈ�� ����
                timers[group.groupName] = Random.Range(0f, group.interval); // Ÿ�̸� �ʱ�ȭ
            }
        }
    }

    private void Update()
    {
        // �� �׷쿡 ���� Ÿ�̸ӿ� �ִϸ��̼� ó��
        foreach (var group in blendShapeGroups)
        {
            if (!blendShapeIndices.ContainsKey(group.groupName)) continue;

            timers[group.groupName] += Time.deltaTime;

            // �ݺ� �ִϸ��̼� ó��
            if (group.loop && timers[group.groupName] >= group.interval)
            {
                timers[group.groupName] = 0f; // Ÿ�̸� �ʱ�ȭ
                StartBlendShapeGroupAnimation(group);
            }

            UpdateBlendShapeGroupWeight(group, timers[group.groupName]);
        }
    }

    // Blend Shape �׷� �ִϸ��̼� ����
    private void StartBlendShapeGroupAnimation(BlendShapeGroup group)
    {
        foreach (var index in blendShapeIndices[group.groupName])
        {
            skinnedMeshRenderer.SetBlendShapeWeight(index, group.maxWeight); // ��� Blend Shape�� �ִ� Weight�� ����
        }
    }

    // Blend Shape �׷� Weight ������Ʈ
    private void UpdateBlendShapeGroupWeight(BlendShapeGroup group, float timer)
    {
        if (timer < group.duration)
        {
            // ���� �� (Weight�� �ִ밪���� �ּҰ����� ���� ����)
            float weight = Mathf.Lerp(group.maxWeight, group.minWeight, timer / group.duration);
            foreach (var index in blendShapeIndices[group.groupName])
            {
                skinnedMeshRenderer.SetBlendShapeWeight(index, weight);
            }
        }
        else
        {
            // �ִϸ��̼� ���� �� Weight�� �ּҰ����� ����
            foreach (var index in blendShapeIndices[group.groupName])
            {
                skinnedMeshRenderer.SetBlendShapeWeight(index, group.minWeight);
            }
        }
    }
}
