using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // 그룹(예: 플레이어 팀) 식별용. 만약 여러 그룹이 있다면 추가 논리로 우선순위 결정 가능.
    public int PlayerId;
    // 각 캐릭터의 이동 방향 배율. 예를 들어 1이면 기본, -1이면 반대로.
    public int Direction;
    public float moveDistance = 1f;
    public float moveSpeed = 5f;
    
    // 중앙 매니저가 예약한 이동 목표
    public Vector3 PlannedTarget { get; set; }
    private Animator _animator;
    private SpriteRenderer _renderer;
    // 이동 중인지를 외부에서 확인하기 위한 프로퍼티
    public bool IsMoving { get; private set; } = false;
    
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    private void Start()
    {
        // 시작 시 현재 위치를 기본 목표로 저장
        PlannedTarget = transform.position;
        SetOrderInLayer();
        // 중앙 매니저에 자신을 등록
        CentralMovementManager.Instance.Register(this);
    }

    void OnDestroy()
    {
        if (CentralMovementManager.Instance != null)
        {
            CentralMovementManager.Instance.Unregister(this);
        }
    }

    /// <summary>
    /// 중앙 매니저에서 결정한 PlannedTarget으로 이동 시작
    /// </summary>
    public void StartMoveToPlannedTarget()
    {
        if (!IsMoving)
            StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        IsMoving = true;
        SetOrderInLayer();
        // 이동 방향 계산
        Vector3 direction = (PlannedTarget - transform.position).normalized;

        if (_animator != null && direction != Vector3.zero)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0) _animator.SetTrigger("MoveRight");
                else _animator.SetTrigger("MoveLeft");
            }
            else
            {
                if (direction.y > 0) _animator.SetTrigger("MoveUp");
                else _animator.SetTrigger("MoveDown");
            }
        }

        // 이동 처리 (MoveTowards 방식)
        while ((PlannedTarget - transform.position).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, PlannedTarget, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = PlannedTarget;
        IsMoving = false;
    }

    private void SetOrderInLayer()
    {
        _renderer.sortingOrder = 30 - 2 * (int)PlannedTarget.y;
    }
}
