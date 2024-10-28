import os
import cv2
import numpy as np
from ultralytics import YOLO

# 모델 로드
model = YOLO("/home/split/yolov8_v3/result/experiment1/weights/best.pt")

# 세그멘테이션을 수행할 이미지 경로 설정
image_path = "/home/split/yolov8_v3/dataset/test/images"
image_files = [
    os.path.join(image_path, f)
    for f in os.listdir(image_path)
    if f.endswith((".png", ".jpg", ".jpeg"))
]

# 세그멘테이션 결과를 저장할 디렉토리 설정
save_directory = "/home/split/yolov8_v3/segmentation_lines"
os.makedirs(save_directory, exist_ok=True)

# 클래스별 색상 설정
colors = [
    (255, 0, 0),  # longi - 빨간색
    (0, 255, 0),  # floor - 초록색
    (0, 0, 255),  # realFloor - 파란색
    (255, 255, 0),  # plate - 노란색
]

# 세그멘테이션 수행 및 라인 추출
for img_file in image_files:
    results = model(img_file)

    # 원본 이미지를 로드하여 라인을 그릴 준비
    original_image = cv2.imread(img_file)

    for i, result in enumerate(results):
        # 세그멘테이션 마스크 얻기 (result.masks)
        masks = (
            result.masks.data.cpu().numpy()
        )  # 텐서를 CPU로 복사한 후 NumPy 배열로 변환

        # 각 마스크에서 경계선 추출
        for j, mask in enumerate(masks):
            mask = (mask * 255).astype(np.uint8)  # 마스크를 이진화
            contours, _ = cv2.findContours(
                mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE
            )  # 윤곽선 찾기

            # 윤곽선을 원본 이미지에 그리기 (클래스별로 다른 색상)
            class_id = int(result.boxes.cls[j])
            color = colors[class_id % len(colors)]  # 색상 선택
            cv2.drawContours(
                original_image, contours, -1, color, 2
            )  # 경계선을 그리기 (2픽셀 두께)

    # 결과 이미지 저장
    base_name = os.path.basename(img_file)
    name, ext = os.path.splitext(base_name)
    save_path = os.path.join(save_directory, f"{name}_segmentation{ext}")
    cv2.imwrite(save_path, original_image)

print("라인 추출 완료 및 이미지 저장이 완료되었습니다.")
