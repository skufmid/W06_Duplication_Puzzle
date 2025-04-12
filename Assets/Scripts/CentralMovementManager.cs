using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class CentralMovementManager : MonoBehaviour
{
    public static CentralMovementManager Instance;

    // 등록된 모든 캐릭터 목록
    private List<PlayerController> players = new List<PlayerController>();
    private bool isProcessingMove = false; // 이동 처리가 진행 중인지 여부

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        InputController.OnMoveInput += HandleMoveInput;
    }

    private void OnDisable()
    {
        InputController.OnMoveInput -= HandleMoveInput;
    }

    // 다른 스크립트에서 캐릭터 등록 시 호출
    public void Register(PlayerController pc)
    {
        players.Add(pc);
    }

    public void Unregister(PlayerController pc)
    {
        players.Remove(pc);
    }


    private void HandleMoveInput(Vector2 dir)
    {
        RequestMove(dir);
    }

    /// <summary>
    /// 모든 캐릭터에 대해 입력 방향 dir로 이동 요청을 처리.
    /// 각 캐릭터의 현재 위치와 입력 방향을 기반으로 목적지(proposedTarget)를 계산한 후,
    /// 우선순위에 따라 장애물 혹은 다른 캐릭터의 이동 예약(Reserved)이 있는지 체크합니다.
    /// </summary>
    public void RequestMove(Vector2 dir)
    {
        // 만약 이전 이동이 진행 중이면 입력 무시
        if (isProcessingMove)
            return;

        if (dir == Vector2.zero)
            return;

        isProcessingMove = true; // 이동 처리 시작

        // 1) 이동 방향에 따른 기본 비교기 생성:
        // 예를 들어, dir.x > 0이면 오른쪽 이동이므로 x 좌표가 큰(오른쪽에 있는) 캐릭터부터 처리.
        Comparison<PlayerController> baseComparer = null;
        if (Mathf.Abs(dir.x) > 0)
        {
            if (dir.x > 0)
                baseComparer = (a, b) => b.transform.position.x.CompareTo(a.transform.position.x);
            else
                baseComparer = (a, b) => a.transform.position.x.CompareTo(b.transform.position.x);
        }
        else if (Mathf.Abs(dir.y) > 0)
        {
            if (dir.y > 0)
                baseComparer = (a, b) => b.transform.position.y.CompareTo(a.transform.position.y);
            else
                baseComparer = (a, b) => a.transform.position.y.CompareTo(b.transform.position.y);
        }
        else
        {
            return;
        }

        // 2) 전체 캐릭터 리스트에서 아직 이동 중이지 않은 캐릭터만 선택.
        List<PlayerController> sortedPlayers = new List<PlayerController>(players);
        sortedPlayers = sortedPlayers.FindAll(p => !p.IsMoving);

        // 3) 우선순위 정렬: 우선 PlayerId 오름차순(낮은 값부터) 후, 같은 그룹 내에서는
        //    - PlayerId가 0: 기본 비교기 순서 그대로 적용
        //    - PlayerId가 1: 기본 비교기 결과 반전 적용
        sortedPlayers.Sort((a, b) =>
        {
            int idCompare = a.PlayerId.CompareTo(b.PlayerId);
            if (idCompare != 0)
                return idCompare;

            int cmp = baseComparer(a, b);
            if (a.PlayerId == 1)
                cmp = -cmp;
            return cmp;
        });

        // 4) 각 캐릭터마다 목적지 예약 및 이동 여부 결정.
        // 예약된 칸은 HashSet을 사용해 중복 체크.
        HashSet<Vector3> reservedSpots = new HashSet<Vector3>();
        // 장애물 체크 시 사용할 레이어 (Hierarchy에서 "Obstacle"과 "Player"라는 레이어를 사용)
        int playerLayer = LayerMask.GetMask("Player");
        int obstacleLayer = LayerMask.GetMask("Obstacle");

        // 내부 함수: 블록(플레이어ID 2) 밀기 시도. 성공하면 블록의 목표 위치를 반환.
        bool TryPushBlock(PlayerController block, Vector2 moveDir, out Vector3 newTarget)
        {
            // 블록의 제안 이동 목표 계산 (블록은 일반적으로 Direction = 1로 사용)
            newTarget = block.transform.position + (Vector3)(moveDir * block.moveDistance);
            // 장애물 및 예약된 칸 체크 (블록은 밀리는 대상이므로, 자기 자신 외의 장애물만 고려)
            Collider2D hit = Physics2D.OverlapCircle(newTarget, 0.1f, obstacleLayer | playerLayer);
            // 단, hit가 block 자기 자신인 경우는 무시
            if (hit != null)
            {
                PlayerController hitPC = hit.GetComponent<PlayerController>();
                if (hitPC != null && hitPC == block)
                {
                    hit = null;
                }
            }
            bool isBlocked = (hit != null) || reservedSpots.Contains(newTarget);
            return !isBlocked;
        }

        // 5) 목적지 예약 처리
        // 현재 위치를 예약 처리
        foreach (var pc in sortedPlayers)
        {
            if (pc.PlayerId != 2)
            {
                reservedSpots.Add(pc.transform.position);
            }
        }


        // 각 캐릭터별로 예약 처리:
        foreach (var pc in sortedPlayers)
        {
            if (pc.PlayerId != 2)
            {
                // effective 이동: 각 캐릭터마다 설정된 moveDistance와 Direction을 곱해서 계산.
                Vector3 proposedTarget = pc.transform.position + (Vector3)(dir * pc.moveDistance * pc.Direction);

                // 장애물 체크: 먼저 일반 장애물를 확인.
                Collider2D hitObj = Physics2D.OverlapCircle(proposedTarget, 0.1f, obstacleLayer);
                //Collider2D hitPlayer = Physics2D.OverlapCircle(proposedTarget, 0.1f, playerLayer);
                bool blockedByObstacle = (hitObj != null);
                //bool blockedByPlayer = (hitPlayer != null);
                bool pushed = false;  // 밀렸는지 여부

                // 만약 목적지에 어떤 물체가 있다면...
                if (blockedByObstacle)
                {
                    // 만약 물체가 블록(즉, PlayerId == 2)라면 밀기를 시도
                    PlayerController pushedBlock = hitObj.GetComponent<PlayerController>();
                    if (pushedBlock != null && pushedBlock.PlayerId == 2)
                    {
                        // 블록이 아직 예약되지 않고 움직이지 않은 상태여야 함.
                        if (!pushedBlock.IsMoving && !reservedSpots.Contains(pushedBlock.transform.position))
                        {
                            // 블록 밀기 시도
                            Vector3 blockNewTarget;
                            if (TryPushBlock(pushedBlock, dir * pc.Direction, out blockNewTarget))
                            {
                                // 블록을 밀 수 있다면, 블록의 목표를 갱신하고 예약 처리.
                                pushedBlock.PlannedTarget = blockNewTarget;
                                reservedSpots.Remove(pc.transform.position);
                                reservedSpots.Add(blockNewTarget);
                                pushed = true;
                                // 이후 원래 캐릭터는 pushedBlock이 있던 칸(즉, proposedTarget)으로 이동 가능하게 됨.
                            }
                        }
                    }
                }

                // 만약 목적지 칸이 예약되어 있거나, 일반 장애물이 있으면 이동 불가 처리.
                if ((!pushed && blockedByObstacle) || reservedSpots.Contains(proposedTarget))
                {
                    // 이동 불가: 현재 위치를 그대로 유지
                    pc.PlannedTarget = pc.transform.position;
                }
                else
                {
                    // 이동 가능: 해당 목적지 예약 및 저장.
                    pc.PlannedTarget = proposedTarget;
                    reservedSpots.Remove(pc.transform.position);
                    reservedSpots.Add(proposedTarget);
                }
            }
        }

        // 6) 모든 연산이 끝난 후, 각 캐릭터가 동시에 예약된 목표(PlannedTarget)로 이동.
        foreach (var pc in sortedPlayers)
        {
            pc.StartMoveToPlannedTarget();
        }

        // 모든 캐릭터가 이동을 완료할 때까지 기다린 후 isProcessingMove를 false로 변경
        StartCoroutine(WaitForMoves());
    }

    private System.Collections.IEnumerator WaitForMoves()
    {
        // 모든 캐릭터가 이동 중이지 않을 때까지 대기
        bool anyMoving = true;
        while (anyMoving)
        {
            anyMoving = players.Any(p => p.IsMoving);
            yield return null;
        }
        isProcessingMove = false; // 이제 새로운 입력을 받을 수 있음
    }
}
