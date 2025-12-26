FROM python:3.10-slim

# GUI desteği için GTK paketleri
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

# OpenCV GUI desteği ile
RUN pip install --no-cache-dir mediapipe opencv-python websockets

WORKDIR /app

CMD ["/bin/bash"]
