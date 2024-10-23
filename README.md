### 1. 프로젝트 소개
#### 1.1. 배경 및 필요성
> 조선업 및 철강 산업에서 구조물의 부재 간 용접 작업은 매우 중요한 공정으로, 작업의 정확성과 효율성이 생산성에 큰 영향을 미친다. 특히, 대형 구조물의 경우 용접해야 할 부위와 길이를 정확하게 파악하는 것이 필수적이다. 최근에는 이러한 용접 작업의 자동화를 위해 딥러닝을 활용한 다양한 연구가 진행되고 있으며, 이를 통해 현장의 작업 효율성을 높이고, 인적 오류를 최소화하려는 시도가 이루어지고 있다.
본 연구는 딥러닝 모델을 활용하여 용접 부위의 길이를 자동으로 검출하고, 이를 AI를 통해 학습시킴으로써 용접 작업의 효율성을 극대화하는 것을 목표로 한다. 특히, Unity 기반의 가상 환경을 활용하여 학습 데이터를 증강하고, 실세계의 데이터를 보완함으로써 모델의 성능을 극대화하려는 시도를 포함하고 있다.


#### 1.2. 목표 및 주요 내용
> 본 연구의 목표는 딥러닝 모델을 활용하여 셀 내부의 부재 간 용접 부위와 그 길이를 자동으로 인식하고, 이를 검출하는 시스템을 구축하는 것이다. 이를 위해 Depth 카메라를 사용하여 3D 포인트 클라우드 데이터를 수집하고, Unity 기반의 가상 환경에서 다양한 학습 데이터를 증강하여 딥러닝 모델의 성능을 높이는 작업을 진행하였다. 최종적으로는 YOLO와 Line Detection 모델을 결합하여 용접선과 그 길이를 정확하게 추출하고, 이를 기반으로 한 자동화된 용접 검출 시스템을 개발하는 것이 연구의 궁극적인 목표이다.


### 2. 상세설계
#### 2.1. 시스템 구성도
> 시스템 구성도(infra, front, back등의 node 간의 관계)의 사진을 삽입하세요.

#### 2.1. 사용 기술
> 스택 별(backend, frontend, designer등) 사용한 기술 및 버전을 작성하세요.
> 
> ex) React.Js - React14, Node.js - v20.0.2

### 3. 설치 및 사용 방법
> 제품을 설치하기 위헤 필요한 소프트웨어 및 설치 방법을 작성하세요.
>
> 제품을 설치하고 난 후, 실행 할 수 있는 방법을 작성하세요.

### 4. 소개 및 시연 영상
> 프로젝트에 대한 소개와 시연 영상을 넣으세요.

### 5. 팀 소개
> 팀원 소개 & 구성원 별 역할 분담 & 간단한 연락처를 작성하세요.
