public interface IAttackable
{
    // 현재 공격 행동
    AttackBehaviour CurrentAttackBehaviour { get; }
    // 공격 로직 호출 함수
    void OnExecuteAttack();
}
