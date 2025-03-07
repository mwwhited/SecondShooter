import sys

# https://github.com/korolkiewiczk/sentence_transformer_docker

from flask import Flask, request, jsonify
from sentence_transformers import SentenceTransformer

arguments = sys.argv
model_name_arg = arguments[1]
print(f"Loading Sentence Transformer: {model_name_arg}")

app = Flask(__name__)
model = SentenceTransformer(model_name_arg)

def encode_text(texts):
    # Prefix each text with "zapytanie: "
    prefixed_texts = ["zapytanie: " + text for text in texts]
    
    # Encode the texts and convert to list
    embeddings = model.encode(prefixed_texts, convert_to_tensor=True, show_progress_bar=False)
    embeddings_list = embeddings.tolist()
    
    return embeddings_list

@app.route('/v1/embeddings', methods=['POST'])
def get_embeddings():
    print("POST /v1/embeddings")
    
    if request.is_json:
        # Handle JSON request
        content = request.json
    else:
        # Handle form data
        content = request.form
    
    text_input = content.get('input')
    model_name = content.get('model', model_name_arg)

    if not text_input:
        return jsonify({"error": "Missing input"}), 400

    # Encode the input text
    embedding_list = encode_text([text_input])[0]

    response = {
        "data": [
            {
                "embedding": embedding_list,
                "index": 0,
                "object": "embedding"
            }
        ],
        "model": model_name,
        "object": "list",
        "usage": {
            "prompt_tokens": len(text_input.split()),
            "total_tokens": len(text_input.split())
        }
    }

    return jsonify(response)

@app.route('/generate-embedding', methods=['GET'])
def generate_embedding():
    print("GET /generate-embedding")
    query = request.args.get('query')
    
    # Encode the query
    embedding_list = encode_text([query])[0]

    return jsonify(embedding=embedding_list)

@app.route('/generate-embeddings', methods=['POST'])
def generate_multiple_embeddings():
    print("POST /generate-embeddings")
    if request.is_json:
        # Handle JSON request
        texts = request.json
    else:
        # Handle form data
        texts = request.form
    
    # Encode the multiple texts
    embeddings_list = encode_text(texts)

    return jsonify(embeddings=embeddings_list)
    
@app.route('/health', methods=['GET'])
def health():
    print("GET /health")
    return f"SBERT Model: {model_name_arg}"

if __name__ == '__main__':
    from waitress import serve
    serve(app, host="0.0.0.0", port=5000)