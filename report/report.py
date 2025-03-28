from flask import Flask, request, jsonify
import subprocess
import json

app = Flask(__name__)

@app.route('/genera_report', methods=['POST'])
def genera_report():
    data = request.json
    cantiere = data.get("cantiere")
    tasks = data.get("tasks")
    materiali = data.get("materiali")
    costi = data.get("costi")
    
    completed_process = subprocess.run(
        ["python", "report_generator.py",
            cantiere, 
            json.dumps(tasks), 
            json.dumps(materiali),
            json.dumps(costi)
        ], 
        capture_output=True, text=True
    )

    if completed_process.returncode == 0:
        return jsonify({"status": "success"}), 200
    else:
        return jsonify({"status": "error", "message": completed_process.stderr}), 500

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5001)