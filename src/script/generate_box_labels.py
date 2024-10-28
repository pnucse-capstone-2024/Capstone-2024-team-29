import cv2
import numpy as np
import os

# 객체와 색상의 매핑 정의 (BGR 포맷)
objectColorMapping = {
    "fillet_hole": (0, 165, 255),  # 오렌지색 (BGR)
    "fillet_yong_jeop_jang": (255, 0, 0),  # 파란색 (BGR)
    "longi_yong_jeop_jang": (0, 0, 255),  # 빨간색 (BGR)
    "plate_behind": (255, 255, 0),  # 시안색 (BGR)
    "plate_CP01": (0, 255, 0),  # 초록색 (BGR)
    "plate_CP03": (0, 255, 255),  # 노란색 (BGR)
    "plate_CP05": (255, 0, 255),  # 마젠타색 (BGR)
}


# 색상 감지 오차범위 설정
color_tolerance = 50  # 이 값을 조정하여 색상 감지 범위를 조정할 수 있음


def generate_detection_labels(
    mask_dir, output_dir, debug_dir, objectColorMapping, color_tolerance
):
    # 출력 디렉토리가 없으면 생성
    if not os.path.exists(output_dir):
        os.makedirs(output_dir)
    if not os.path.exists(debug_dir):
        os.makedirs(debug_dir)

    # 마스크 디렉토리 내 모든 파일 리스트를 가져옴
    mask_files = os.listdir(mask_dir)

    if not mask_files:
        print(f"Warning: {mask_dir} is empty. Skipping...")
        return  # 폴더가 비어있으면 넘어감

    for mask_file in mask_files:
        mask_path = os.path.join(mask_dir, mask_file)  # 각 마스크 이미지의 전체 경로
        mask = cv2.imread(mask_path)  # 마스크 이미지를 읽어옴 (컬러로 읽음)

        if mask is None:
            continue  # 마스크 이미지를 읽을 수 없는 경우 건너뜀

        # 디버그 이미지를 위한 복사본 생성
        debug_image = mask.copy()

        label_file = os.path.join(
            output_dir, mask_file.replace(".png", ".txt")
        )  # 라벨 파일 저장 경로
        with open(label_file, "w") as f:  # 라벨 파일을 쓰기 모드로 열기
            for label, color in objectColorMapping.items():
                # 색상 범위를 오차범위 내에서 조정
                lower_bound = np.array([max(0, c - color_tolerance) for c in color])
                upper_bound = np.array([min(255, c + color_tolerance) for c in color])

                # 특정 객체의 색상에 해당하는 픽셀만 추출
                mask_for_color = cv2.inRange(mask, lower_bound, upper_bound)
                contours, _ = cv2.findContours(
                    mask_for_color, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE
                )

                for contour in contours:
                    if cv2.contourArea(contour) < 50:  # 너무 작은 윤곽선은 무시
                        continue

                    # 객체의 경계 상자 추출 (바운딩 박스)
                    x, y, w, h = cv2.boundingRect(contour)

                    # 중심 좌표와 너비, 높이를 YOLO 형식에 맞게 정규화
                    x_center = (x + w / 2) / mask.shape[1]
                    y_center = (y + h / 2) / mask.shape[0]
                    width = w / mask.shape[1]
                    height = h / mask.shape[0]

                    # 클래스 ID와 바운딩 박스 좌표를 한 줄로 구성
                    line = f"{list(objectColorMapping.keys()).index(label)} {x_center} {y_center} {width} {height}\n"
                    f.write(line)

                    # 디버그 이미지를 위한 바운딩 박스 그리기
                    cv2.rectangle(
                        debug_image, (x, y), (x + w, y + h), (255, 255, 255), 2
                    )

        # 디버그 이미지 저장 (모든 객체를 포함한 하나의 이미지)
        cv2.imwrite(os.path.join(debug_dir, f"debug_{mask_file}"), debug_image)


def process_datasets(base_dir, objectColorMapping, color_tolerance):
    splits = ["train", "val", "test"]

    for split in splits:
        mask_directory = os.path.join(base_dir, split, "masks")
        label_directory = os.path.join(base_dir, split, "labels")
        debug_directory = os.path.join(base_dir, split, "debug")

        if not os.path.exists(mask_directory) or not os.listdir(mask_directory):
            print(f"Warning: {mask_directory} is empty or does not exist. Skipping...")
            continue  # 폴더가 없거나 비어있으면 넘어감

        # 라벨 생성 함수 호출
        generate_detection_labels(
            mask_directory,
            label_directory,
            debug_directory,
            objectColorMapping,
            color_tolerance,
        )


# 데이터셋 기본 경로 설정
base_directory = "./dataset"

# 데이터셋 처리 함수 호출
process_datasets(base_directory, objectColorMapping, color_tolerance)
