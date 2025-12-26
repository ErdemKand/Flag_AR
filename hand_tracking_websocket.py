import cv2
import mediapipe as mp
from mediapipe.tasks import python
from mediapipe.tasks.python import vision
import asyncio
import websockets
import json

# Global değişkenler
latest_hand_data = {"hands": []}

# MediaPipe setup
base_options = python.BaseOptions(model_asset_path='hand_landmarker.task')
options = vision.HandLandmarkerOptions(
    base_options=base_options,
    num_hands=2,
    min_hand_detection_confidence=0.5,
    min_hand_presence_confidence=0.5,
    min_tracking_confidence=0.5
)
detector = vision.HandLandmarker.create_from_options(options)

# WebSocket server
async def send_hand_data(websocket):
    print(f"Unity bağlandı: {websocket.remote_address}")
    try:
        while True:
            # El verilerini gönder
            await websocket.send(json.dumps(latest_hand_data))
            await asyncio.sleep(0.01)  # 100 FPS
    except websockets.exceptions.ConnectionClosed:
        print("Unity bağlantısı kesildi")

async def process_camera():
    global latest_hand_data
    cap = cv2.VideoCapture(0)
    
    print("Kamera başlatıldı. WebSocket sunucusu port 8765'te dinliyor...")
    
    while cap.isOpened():
        ret, frame = cap.read()
        if not ret:
            break
        
        # MediaPipe işleme
        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=rgb_frame)
        results = detector.detect(mp_image)
        
        # Veriyi hazırla
        hands_data = []
        if results.hand_landmarks:
            for hand_idx, hand_landmarks in enumerate(results.hand_landmarks):
                landmarks_list = []
                for landmark in hand_landmarks:
                    landmarks_list.append({
                        "x": landmark.x,
                        "y": landmark.y,
                        "z": landmark.z
                    })
                
                # Handedness (sol/sağ el)
                handedness = "Unknown"
                if results.handedness and hand_idx < len(results.handedness):
                    handedness = results.handedness[hand_idx][0].category_name
                
                hands_data.append({
                    "handedness": handedness,
                    "landmarks": landmarks_list
                })
        
        latest_hand_data = {"hands": hands_data}
        
        # Görüntü göster (opsiyonel)
        if results.hand_landmarks:
            for hand_landmarks in results.hand_landmarks:
                for landmark in hand_landmarks:
                    x = int(landmark.x * frame.shape[1])
                    y = int(landmark.y * frame.shape[0])
                    cv2.circle(frame, (x, y), 5, (0, 255, 0), -1)
        
        cv2.imshow('Hand Tracking', frame)
        
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break
        
        await asyncio.sleep(0.001)
    
    cap.release()
    cv2.destroyAllWindows()

async def main():
    # WebSocket server ve kamera işlemeyi paralel çalıştır
    server = await websockets.serve(send_hand_data, "0.0.0.0", 8765)
    await process_camera()
    server.close()
    await server.wait_closed()

if __name__ == "__main__":
    asyncio.run(main())
