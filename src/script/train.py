from ultralytics import YOLO


# 모델 학습 함수
def train_model(model_size):
    model = YOLO(f"yolov8{model_size}.pt")  # 모델 사이즈에 따라 's', 'm', 'l' 모델 선택
    model.train(
        data="/home/split/yolov8_box_v15/data.yaml",
        epochs=30,
        imgsz=640,
        save_dir=f"./runs/train/{model_size}",
        workers=2,
    )


if __name__ == "__main__":
    model_sizes = ["s", "m"]  # 's', 'm' 크기 모델을 학습할 리스트

    for size in model_sizes:
        train_model(size)  # 각 모델의 학습을 순차적으로 실행
