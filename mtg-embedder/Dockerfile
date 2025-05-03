# Use a lightweight Python image
FROM python:3.9.22-bullseye

# Set environment variables
ENV PYTHONDONTWRITEBYTECODE=1
ENV PYTHONUNBUFFERED=1
ENV FLASK_APP=embed_service.py
ENV FLASK_RUN_HOST=0.0.0.0

# Set work directory
WORKDIR /app

RUN apt-get update && apt-get install -y build-essential \
    && rm -rf /var/lib/apt/lists/*   # Remove package cache

# Install Python dependencies
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt \
    --extra-index-url https://download.pytorch.org/whl/cpu

# Copy app source code
COPY . .

# Expose the port Flask runs on
EXPOSE 5000

# Run Flask
CMD ["gunicorn", "--bind", "0.0.0.0:5000", "embed_service:app"]
