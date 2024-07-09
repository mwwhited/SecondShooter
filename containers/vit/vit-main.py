import sys
from flask import Flask, request, jsonify, send_file
from transformers import ViTFeatureExtractor, ViTModel
from PIL import Image
import io
import rawpy

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

# Helper function to convert any image format to JPEG
def convert_to_jpeg(image_data):
    try:
        image = Image.open(io.BytesIO(image_data))
        # Convert to JPEG format
        with io.BytesIO() as output:
            image.convert("RGB").save(output, format="JPEG")
            return output.getvalue()
    except Exception as e:
        print(f"Error converting image to JPEG: {str(e)}")
        return None

# Function to convert raw images to JPEG
def convert_raw_to_jpeg(image_data):
    try:
        # Use rawpy to read raw image data
        with rawpy.imread(io.BytesIO(image_data)) as raw:
            rgb = raw.postprocess()
        
        # Convert to PIL Image
        pil_image = Image.fromarray(rgb)
        
        # Convert to JPEG format
        with io.BytesIO() as output:
            pil_image.convert("RGB").save(output, format="JPEG")
            return output.getvalue()
    
    except rawpy.LibRawNonFatalError as e:
        print(f"LibRaw non-fatal error: {str(e)}")
        return None
    except Exception as e:
        print(f"Error converting raw image to JPEG: {str(e)}")
        return None

@app.route('/v1/embeddings', methods=['POST'])
def get_embeddings():
    print("POST /v1/embeddings")
    try:
        # Check if request contains files
        if 'image' in request.files:
            file = request.files['image']
            image_data = file.read()

            # Determine image type and convert if necessary
            if file.filename.lower().endswith(('.nef', '.cr2', '.arw', '.dng', '.raw')):
                # Convert raw image to JPEG
                image_data = convert_raw_to_jpeg(image_data)
                if image_data is None:
                    return jsonify({"error": "Error converting raw image to JPEG"}), 500
            else:
                # Convert any other image format to JPEG
                image_data = convert_to_jpeg(image_data)
                if image_data is None:
                    return jsonify({"error": "Error converting image to JPEG"}), 500
        elif 'image' in request.files or request.data:
            image_data = request.data

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

@app.route('/v1/jpeg', methods=['POST'])
def convert_and_return_jpeg():
    print("POST /v1/jpeg")
    try:
        if 'image' in request.files:
            file = request.files['image']
            image_data = file.read()
        elif 'image' in request.files or request.data:
            image_data = request.data

        # Convert image to JPEG format
        image_data_jpeg = convert_to_jpeg(image_data)
        if image_data_jpeg is None:
            return jsonify({"error": "Error converting image to JPEG"}), 500
        
        # Return the converted JPEG image
        return send_file(io.BytesIO(image_data_jpeg), mimetype='image/jpeg')

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
