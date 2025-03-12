"""
Service Package
"""
import requests
from flask import Flask, jsonify, abort, request
from flask_cors import CORS, cross_origin
from common import status

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
    app.logger.info("Received question: %s", question)

    # try:
    #     response = requests.post(AI_API_URL, json={"question": question})
    #     response.raise_for_status() #Ako AI API vrati grešku, podići će izuzetak
    # except requests.RequestException as e:
    #     app.logger.error("Error contacting AI API: %s", str(e))
    #     return abort(status.HTTP_500_INTERNAL_SERVER_ERROR, "Error contacting AI service")

    # ai_response = response.json()
    # if "answer" not in ai_response:
    #     return abort(status.HTTP_500_INTERNAL_SERVER_ERROR, "Invalid response from AI service")

    # answer = ai_response["answer"]
    # app.logger.info("AI response: %s", answer)

    return jsonify(answer="temporary hardcoded answer"), status.HTTP_200_OK