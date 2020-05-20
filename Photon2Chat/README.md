# 큰제목
## 중간 제목
### 하위 제목
>인용문
- 순서없는 목록
    - 순서없는 목록
        - 순서없는 목록
1. 순서있는 목록
2. 순서있는 목록
3. 순서있는 목록  
'인라인 코드 블록'
```
#include <stdio.h>
int main(){
    return 0;
}
```

# 유니티 3D RPG 기능 구현
## 주요 오브젝트
- Player
- Enemy
- Inventory
- Equipment Manager
- Pickup Item
- Guide Manager
- Health UI
- QuickSlot
- Boss Manager
- Camera Move

> Player
- PlayerAnimator  
    - 플레이어 애니메이션 처리
- PlayerCtrl
    - 종합적인 플레이어 로직 처리
- PlayerStats
    - 플레이어 능력치 설정 및 능력치 관련 처리
- Motor
    - 플레이어 이동 및 회전 처리
- HealthUI
    - 플레이어 체력 정보 표시

> Enemy
- EnemyFSM
- EnemyCtrl
- CharacterStats
- DropItemCtrl
- HealthUI
