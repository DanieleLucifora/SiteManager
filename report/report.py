#Server Flask
from flask import Flask, request, jsonify   #request è la richiest HTTP che arriva tramite flask
import subprocess
import json

app = Flask(__name__) #creazione dell'app flask

#decorator (quando visitiamo http://localhost:5001/genera_report viene eseguito genera_report()")
@app.route('/genera_report', methods=['POST'])
def genera_report():
    data = request.json
    cantiere = data.get("cantiere")
    tasks = data.get("tasks")
    materiali = data.get("materiali")
    costi = data.get("costi")

    if not cantiere or tasks is None or materiali is None or costi is None:
        return jsonify({"status": "error", "message": "Dati necessari non presenti"}), 400  #HTTP bad request

    try:
        # subprocess.run fa sì che i parametri siano passati tramite riga di comando
        result = subprocess.run(
            ["python3", "/app/report_generator.py", #sys.argv[0]
                cantiere, 
                json.dumps(tasks), 
                json.dumps(materiali),
                json.dumps(costi)
            ], 
            capture_output=True, text=True
        )

        if result.returncode == 0:  #eseguito con successo
            output = result.stdout.strip()
            return jsonify({"status": "success", "report": output}), 200
        else:
            return jsonify({"status": "error", "message": result.stderr.strip()}), 500
    except Exception as e:

        return jsonify({"status": "error", "message": str(e)}), 500

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5001)