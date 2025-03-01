from flask import Flask, request, jsonify
import subprocess
import os
import json

app = Flask(__name__)

# Percorso dello script Python
SCRIPT_PATH = os.path.join(os.getcwd(), "report_generator.py")

@app.route('/genera_report', methods=['POST'])
def genera_report():
    data = request.json
    cantiere = data.get("cantiere")
    tasks = data.get("tasks")
    materiali = data.get("materiali")

    if not cantiere or tasks is None or materiali is None:
        return jsonify({"status": "error", "message": "Cantiere non specificato"}), 400

    try:
        result = subprocess.run(
            ["python3", SCRIPT_PATH, 
                cantiere, 
                json.dumps(tasks), 
                json.dumps(materiali)
            ], 
            capture_output=True, text=True
        )

        if result.returncode == 0:
            output = result.stdout.strip()
            return jsonify({"status": "success", "report": output}), 200
        else:
            return jsonify({"status": "error", "message": result.stderr.strip()}), 500
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5001)