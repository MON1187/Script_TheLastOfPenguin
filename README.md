# The-Last-Of-Penguin - Character Part Script

Unity 기반 프로젝트인 The Last Of Penguin 프로젝트에서 작업한 스크립트기반으로 재구성하여 정리해놓은 포트폴리오 입니다.

## 개요

프로젝트 The Last Of Penguin을 진행하며 담당하여 개발한 결과물들만 정리하여 제공합니다.

## 코드 개요

폴더내의 있는 스크립트를 정리하여 기능의 필요한 로직간의 관계에 초점을 맞춰 설명드립니다.

### Character Function - Character Forder

#### 개요

 초기 기획 당시 캐릭터의 종류에 따른 각 캐릭터의 특성을 담을 스크립트 입니다.
 제작 당시에는 NormalPenguin으로만 진행되어, 형태만 만들어둔 상태 였습니다.


### Character Function - Refactoring Forder

- penguin Body
  캐릭터의 상태를 관리하는 스크립트 입니다.
  상태이상, 게임 진행에 연관되어 있습니다.

- Penguin Function
  플레이어 캐릭터의 쓰이는 메인 스크립트 입니다.
  플레이어의 행동을 메인으로 모두 받습니다.

  움직임, 아이템 활성화/사용 모두 관리 합니다.

#### UI

- Penguin Status UI
  캐릭터의 현재 상태를 확인하여 UI의 상태를 변경하는데 사용 됩니다.

