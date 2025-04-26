from flask import Flask, request, jsonify
from sentence_transformers import SentenceTransformer

app = Flask(__name__)
model = SentenceTransformer("all-MiniLM-L6-v2")

@app.route("/embed", methods=["POST"])
def embed_text():
    data = request.get_json()
    texts = data.get("texts")
    batch_size = data.get("batch_size")
    if not texts or not isinstance(texts, list):
        return jsonify({"error": "Missing 'texts' array"}), 400

    # Write log line
    print(f"Received {len(texts)} texts to embed with batch size {batch_size}")
    vectors = model.encode(texts, batch_size=batch_size).tolist()
    return jsonify({"vectors": vectors})

if __name__ == "__main__":
    app.run(port=5000)
