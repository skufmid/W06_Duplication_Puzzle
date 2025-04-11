using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class CentralMovementManager : MonoBehaviour
{
    public static CentralMovementManager Instance;

    // 등록된 모든 캐릭터 목록
    private List<PlayerController> players = new List<PlayerController>();

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
        if (dir == Vector2.zero)
            return;

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

        // 2) 전체 캐릭터 리스트에서 이동 가능한(즉, 아직 움직임 중이 아닌) 캐릭터만 선택.
        List<PlayerController> sortedPlayers = new List<PlayerController>(players);
        sortedPlayers = sortedPlayers.FindAll(p => !p.IsMoving);

        // 3) 우선순위 정렬:
        // 먼저 PlayerId의 오름차순(낮은 값부터)으로 정렬하고,
        // 같은 그룹(PlayerId 동일) 내에서는
        // - PlayerId가 0인 경우: baseComparer 기준 그대로 적용.
        // - PlayerId가 1인 경우: baseComparer 결과를 반전시킴.
        sortedPlayers.Sort((a, b) =>
        {
            int idCompare = a.PlayerId.CompareTo(b.PlayerId);
            if (idCompare != 0)
                return idCompare;

            int cmp = baseComparer(a, b);
            // 동일 그룹에서 PlayerId가 1이면 비교 결과를 뒤집음.
            if (a.PlayerId == 1)
                cmp = -cmp;
            return cmp;
        });

        // 4) 각 캐릭터마다 목적지 예약(reservation) 및 이동 여부 결정.
        // 예약된 칸은 HashSet을 사용해 중복 체크.
        HashSet<Vector3> reservedSpots = new HashSet<Vector3>();
        // Obstacle 및 Player 체크에 사용할 레이어.
        int playerLayer = LayerMask.GetMask("Player");
        int obstacleLayer = LayerMask.GetMask("Obstacle");

        foreach (var pc in sortedPlayers)
        {
            // effective 이동: 각 캐릭터마다 설정된 moveDistance와 Direction(예: 1 혹은 -1)을 곱해서 계산.
            Vector3 proposedTarget = pc.transform.position + (Vector3)(dir * pc.moveDistance * pc.Direction);

            // 장애물 또는 다른 플레이어(또는 예약된 칸) 체크.
            Collider2D hit = Physics2D.OverlapCircle(proposedTarget, 0.1f, obstacleLayer + playerLayer);
            bool blockedByObstacle = (hit != null);
            bool blockedByReservation = reservedSpots.Contains(proposedTarget);

            if (!blockedByObstacle && !blockedByReservation)
            {
                // 이동 가능하면 해당 목적지를 예약하고 저장.
                pc.PlannedTarget = proposedTarget;
                reservedSpots.Add(proposedTarget);
            }
            else
            {
                // 이동 불가하면 현재 위치를 그대로 저장하여 움직이지 않도록 함.
                pc.PlannedTarget = pc.transform.position;
            }
        }

        // 5) 모든 연산이 끝난 후, 각 캐릭터가 실제로 예약된 목표(PlannedTarget)로 동시에 이동.
        foreach (var pc in sortedPlayers)
        {
            pc.StartMoveToPlannedTarget();
        }
    }

}
