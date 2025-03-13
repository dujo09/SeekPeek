"""
Service Package
"""
import requests
# import ctypes
import clr
import System
import sys
from flask import Flask, jsonify, abort, request, make_response, send_file
from flask_cors import CORS
from common import status
import os
import pythonnet
import subprocess
import json

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

@app.route("/getExcel", methods=["POST"])
def getExcel():
    """Prima formu od frontenda i generira excel"""

    data = request.get_json()
    print(f"Recieved POST request at /getExcel")

    if not data or "answers" not in data:
        return abort(status.HTTP_400_BAD_REQUEST, "Missing 'question' in request body")

    answers = data["answers"]
    with open("data.txt","w", encoding="utf-8") as file:
        file.write(answers)

    csprojPath = os.path.join(os.path.dirname(os.path.realpath(__file__)), "generateExcel", "SeekPeek", "SeekPeek.csproj")
    requestDataPath = os.path.join(os.path.dirname(os.path.realpath(__file__)), "data.txt")

    subprocess.run(["dotnet", "run", "--project", csprojPath, requestDataPath], stdout=subprocess.PIPE)

    excelFullFilePath = os.path.join(os.path.dirname(os.path.realpath(__file__)), "generateExcel", "obrazac-output.xlsx")

    if os.path.isfile(excelFullFilePath):
        return send_file(excelFullFilePath, as_attachment=True)
    else:
        return make_response("Excel file not found.", status.HTTP_404_NOT_FOUND)