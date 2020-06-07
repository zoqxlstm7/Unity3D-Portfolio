# Zombie War
<img src="./Image/메인.PNG"></img>
[플레이영상](https://blog.naver.com/zoqxlstm6/221993074008)
<iframe width="544" height="306" src="https://serviceapi.nmv.naver.com/flash/convertIframeTag.nhn?vid=970E18FA45EE5D9046B2832C7E06433F2F0E&outKey=V123d30dfbea3d23c2e18bbc72b7a66561cb99dd5dc2db60b28b3bbc72b7a66561cb9" frameborder="no" scrolling="no" title="NaverVideo" allow="autoplay; gyroscope; accelerometer; encrypted-media" allowfullscreen></iframe>
## 프로젝트 소개
- Photon Pun2를 이용하여 네트워크를 구성하고, 최대 4명까지 수용 가능한 방에 입장하여 좀비들을 물리치고, 보스를 격파하면 승리하는 멀티 슈팅 게임입니다. 플레이어는 시작 전 원하는 캐릭터와 각각의 능력치가 다른 총기를 선택한 후 플레이하게 됩니다. 플레이 중에는 힐팩과 수류탄을 사용할 수 있고, 좀비를 처치하면 확률에 따른 아이템을 얻습니다. 게임 종료시에는 플레이시 적에게 가한 피해량을 계산하여 최고의 플레이를 뽑습니다.
## 기능 소개
- Photon Pun2를 이용한 네트워크 연결

### 주요 문제점 해결
- 로비 입장 시 OnRoomListUpdate() 콜백 함수가 두번 호출되어 삭제될 방이 그대로 남아 있고 2개이상의 방정보를 표시하지 못하는 현상
    - 초기 상황
        - 콜백함수 호출 시 모든 방정보 오브젝트를 파괴하고 콜백함수에서 반환되는 방정보 리스트를 바탕으로 방목록 갱신
    - 문제 원인
        - OnRoomListUpdate() 콜백 함수에 의해 반환되는 방정보 리스트는 모든 방정보를 반환하지 않음
        - 유지될 방과 삭제될 방의 정보를 두번 불러오기 때문에 두번 호출됨
        - 유지될 방정보와 삭제될 방정보를 두번 반환
    - 해결방법
        - OnRoomListUpdate() 콜백함수는 로비 진입시 호출되는 콜백함수인데, 현재 생성된 방의 모든 정보를 불러오는 기능이 아님
        - 정보가 업데이트된 방 중 유지될 방과 삭제될 방을 리스트로 반환
        - RemovedFromList 프로퍼티를 이용하여 삭제되는 방인지 유지되는 방인지 확인
        - 방정보 오브젝트와 같은 이름을 가진 방이 존재 하지 않을 때는 생성되는 방으로 판단하고 방정보 오브젝트 생성
        - 유지되는 방인 경우 이미 생성된 방정보를 나타내는 오브젝트의 방이름을 확인하여 방정보 오브젝트가 존재한다면 현재 입장중인 유저수만 업데이트
        - 삭제되는 방인 경우 생성되어있는 방정보 오브젝트의 방이름을 확인하여 삭제 처리


