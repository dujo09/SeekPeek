"""
Controller for routes
"""

import requests
from flask import jsonify, url_for, abort
from service import app
from service.common import status

AI_API_URL = "https://api.example.com/getAiResponse"

COUNTER = {}

 
############################################################
# receive query from frontend and send query to ai and then send the answer back to frontend, does that make sense
############################################################
@app.route("/getAiResponse", methods=["POST"])
def getAiResponse():
    """Prima pitanje od frontenda, šalje ga eksternom AI API-ju i vraća odgovor"""
    app.logger.info("Request to ask AI...")

    data = request.get_json()
    if not data or "question" not in data:
        return abort(status.HTTP_400_BAD_REQUEST, "Missing 'question' in request body")

    question = data["question"]
    app.logger.info("Received question: %s", question)

    try:
        response = requests.post(AI_API_URL, json={"question": question})
        response.raise_for_status() #Ako AI API vrati grešku, podići će izuzetak
    except requests.RequestException as e:
        app.logger.error("Error contacting AI API: %s", str(e))
        return abort(status.HTTP_500_INTERNAL_SERVER_ERROR, "Error contacting AI service")

    ai_response = response.json()
    if "answer" not in ai_response:
        return abort(status.HTTP_500_INTERNAL_SERVER_ERROR, "Invalid response from AI service")

    answer = ai_response["answer"]
    app.logger.info("AI response: %s", answer)

    return jsonify(answer=answer), status.HTTP_200_OK
