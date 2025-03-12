"""
Service Package
"""
import requests
from flask import Flask, jsonify, abort, request
from flask_cors import CORS
from common import status

AI_API_URL = "https://rg-ent-poc-euwe-seekpeek-rwncr.westeurope.inference.ml.azure.com/score"
AI_API_KEY = "4lf1BQnLEKotUUwbeJbOPjleN0ihIZgZStiPtD3XewF8DmdjBwE6JQQJ99BCAAAAAAAAAAAAINFRAZML2OX2"

app = Flask(__name__)
CORS(app)

@app.route("/getAiResponse", methods=["POST"])
def getAiResponse():
    """Prima pitanje od frontenda, šalje ga eksternom AI API-ju i vraća odgovor"""

    data = request.get_json()
    print(f"Recieved POST request at /getAiResponse with data: {data}")

    if not data or "question" not in data:
        return abort(status.HTTP_400_BAD_REQUEST, "Missing 'question' in request body")

    question = data["question"]

    try:
        response = requests.post(AI_API_URL, json={"question": question}, headers={"Accept": "application/json", "Authorization": f"Bearer {AI_API_KEY}", "Content-Type": "application/json"})
        response.raise_for_status()
    except requests.RequestException as e:
        print(f"Error contacting AI API: {str(e)}")
        return abort(status.HTTP_500_INTERNAL_SERVER_ERROR, "Error contacting AI service")

    ai_response = response.json()
    print(f"AI RESPONSE: {ai_response}")

    return jsonify(answer=ai_response), status.HTTP_200_OK