import sys
from flask import Flask, request, jsonify
from transformers import ViTFeatureExtractor, ViTModel
from PIL import Image
import io

# Set up Flask application
app = Flask(__name__)

# Get model name from command line argument
arguments = sys.argv
model_name_arg = arguments[1]
print(f"Loading Vision Transformer: {model_name_arg}")

# Load ViT model and feature extractor
feature_extractor = ViTFeatureExtractor.from_pretrained(model_name_arg)
model = ViTModel.from_pretrained(model_name_arg)

# Function to encode images
def encode_image(image_data):
    try:
        # Open image from binary data
        image = Image.open(io.BytesIO(image_data))
        # Preprocess image and generate embeddings
        inputs = feature_extractor(images=image, return_tensors="pt")
        outputs = model(**inputs)
        embeddings = outputs.last_hidden_state
        return embeddings
    except Exception as e:
        print(f"Error processing image: {str(e)}")
        return None

@app.route('/v1/embeddings', methods=['POST'])
def get_embeddings():
    print("POST /v1/embeddings")
    try:
        # Check if request contains files
        if 'image' not in request.files:
            return jsonify({"error": "No image file provided"}), 400

        file = request.files['image']
        
        # Ensure file is a JPEG image
        if file.filename == '' or not file.filename.endswith('.jpg') and not file.filename.endswith('.jpeg'):
            return jsonify({"error": "Unsupported file type. Please provide a JPEG image"}), 400
        
        # Read image file data
        image_data = file.read()
        
        # Encode the image
        embeddings = encode_image(image_data)
        
        if embeddings is None:
            return jsonify({"error": "Error processing image. Please check the image file."}), 500
        
        response = {
            "data": [
                {
                    "embedding": embeddings.tolist(),
                    "index": 0,
                    "object": "embedding"
                }
            ],
            "model": model_name_arg,
            "object": "list"
        }
        
        return jsonify(response), 200
    
    except Exception as e:
        print(f"Error processing request: {str(e)}")
        return jsonify({"error": "An error occurred while processing the request"}), 500

@app.route('/health', methods=['GET'])
def health():
    print("GET /health")
    return f"ViT Model: {model_name_arg}"

if __name__ == '__main__':
    from waitress import serve
    serve(app, host="0.0.0.0", port=5000)
