# Flag AR - Unity Hand Tracking Project

A hand tracking project for Unity using MediaPipe and Docker containerization on Ubuntu 24.04.

## Overview

Ubuntu 24.04 (Noble Numbat) can present compatibility issues with MediaPipe due to Python version dependencies. This project uses Docker to provide a stable Ubuntu 22.04 environment, following industry-standard containerization practices.

## Prerequisites

- Ubuntu 24.04
- Docker
- Unity 2022.3.62f1
- Webcam

## Installation

### 1. Install Docker
```bash
sudo apt update
sudo apt install docker.io -y
sudo usermod -aG docker $USER
newgrp docker
```

### 2. Create Docker Environment

Create project directory and Dockerfile:
```bash
mkdir ~/mediapipe_docker
cd ~/mediapipe_docker
nano Dockerfile
```

Paste the following Dockerfile content:
```dockerfile
FROM python:3.10-slim

RUN apt-get update && apt-get install -y --no-install-recommends \
    libglib2.0-0 \
    libgl1 \
    libgtk2.0-dev \
    pkg-config \
    libavcodec-dev \
    libavformat-dev \
    libswscale-dev \
    libv4l-dev \
    libxvidcore-dev \
    libx264-dev \
    && rm -rf /var/lib/apt/lists/*

RUN pip install --no-cache-dir mediapipe opencv-python websockets

WORKDIR /app

CMD ["/bin/bash"]
```

### 3. Build Docker Image
```bash
docker build -t mediapipe-env .
```

### 4. Download MediaPipe Model
```bash
wget -O hand_landmarker.task https://storage.googleapis.com/mediapipe-models/hand_landmarker/hand_landmarker/float16/1/hand_landmarker.task
```

### 5. Create Python Script
```bash
nano hand_tracking_websocket.py
```

Paste the `hand_tracking_websocket.py` code into this file.

### 6. Run Docker Container
```bash
xhost +local:docker
cd ~/mediapipe_docker
docker run -it --rm \
    --device=/dev/video0 \
    -e DISPLAY=$DISPLAY \
    -v /tmp/.X11-unix:/tmp/.X11-unix \
    -v $(pwd):/app \
    -p 8765:8765 \
    mediapipe-env python3 hand_tracking_websocket.py
```

## Unity Setup

### 1. Create New Project

- Open Unity Hub
- Create new project using **3D (URP)** template
- Unity version: **2022.3.62f1**

### 2. Install WebSocket Package

1. Go to **Window → Package Manager**
2. Click **+** button
3. Select **Add package from git URL**
4. Paste: `https://github.com/endel/NativeWebSocket.git#upm`

### 3. Add Scripts to Main Camera

Create the following C# scripts in Assets folder and add them as components to **Main Camera**:

1. **HandDataReceiver.cs** - Receives hand tracking data from WebSocket
2. **HandVisualizer.cs** - Visualizes hand landmarks in 3D space
2. <img width="322" height="132" alt="image" src="https://github.com/user-attachments/assets/91bf2eb4-4fbd-4e5e-9015-b4d7523779cf" />
3. **HandInteraction.cs** - Handles hand-object interactions
4. **GameController.cs** - Manages game logic and flag matching
4. <img width="955" height="1077" alt="image" src="https://github.com/user-attachments/assets/850d9462-2332-4ae7-98f8-dc473cdaa828" />

### 4. Scene Setup

#### Create Landmark Spawn Point
1. Create a 3D Cube in Hierarchy
2. Rename to `LandmarkSpawnPoint`
3. Configure transform:
   - Position: (0, 0, 0)
   - Rotation: (0, 0, 0)
   - Scale: (1, 1, 1)

#### Create Game Objects
- Create 3D cubes for **flag slots**
- Create 3D cubes for **flags**
- Position them according to your game design
- <img width="489" height="775" alt="image" src="https://github.com/user-attachments/assets/8e0c2681-a7ef-40b4-9750-169f9040d9f4" />
- <img width="489" height="775" alt="image" src="https://github.com/user-attachments/assets/2fd4d55e-b495-4ab9-a006-f6ed700b1abd" />

### 5. Import Assets

1. Open Unity Asset Store
2. Search and import free landmark assets
3. Assign materials and textures to your cubes

## Main Camera Inspector Configuration

Ensure Main Camera has the following components attached:
- HandDataReceiver
- HandVisualizer
- HandInteraction
- GameController
- <img width="489" height="775" alt="image" src="https://github.com/user-attachments/assets/277f17c3-83e7-4c25-a84f-628564a009d0" />
- <img width="610" height="518" alt="image" src="https://github.com/user-attachments/assets/8a8a45b3-18a3-463f-b600-c74ca4a62ccc" />

## Project Structure
```
mediapipe_docker/
├── Dockerfile
├── hand_landmarker.task
└── hand_tracking_websocket.py

Unity Project/
├── Assets/
│   ├── Scripts/
│   │   ├── HandDataReceiver.cs
│   │   ├── HandVisualizer.cs
│   │   ├── HandInteraction.cs
│   │   └── GameController.cs
│   └── Materials/
└── Scene/
```

## Usage

1. Start the Docker container with MediaPipe tracking
2. Open Unity project
3. Press Play in Unity Editor
4. The system will connect to WebSocket and start tracking your hands
5. Interact with flags using hand gestures

## Troubleshooting

- If camera is not detected, check `/dev/video0` permissions
- If Unity can't connect, verify WebSocket is running on port 8765
- For X11 display issues, run `xhost +local:docker` again

## Technologies Used

- **Docker** - Containerization
- **MediaPipe** - Hand tracking
- **OpenCV** - Computer vision
- **WebSocket** - Real-time communication
- **Unity 2022.3.62f1** - Game engine with URP
- **Python 3.10** - Backend processing

## Demo Video
https://youtu.be/hCaKOs2ViGQ
