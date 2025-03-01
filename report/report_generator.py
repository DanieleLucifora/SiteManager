import argparse #per accettare i parametri in ingresso
import json
from reportlab.pdfgen import canvas as canvas
from reportlab.lib.pagesizes import A4
from datetime import datetime

'''
Formato del file PDF:

    y
    ^       Intestazione
    |   Colonna 1   Colonna 2
    |   
    |   testo       testo
    |   ...         ...
    |
    |
    |
    |
    |
    |
    |
  (0,0)-----------------------> x

'''

COLONNA_1 = 30  #Valore di inizio sull'asse x 
COLONNA_2 = 310
ALTEZZA_INIZIO = 750
ALTEZZA_FINE = 60
INTERLINEA = 20
INTERLINEA_TITOLO = 10

#Posizione attuale sulla pagina (x, y)
# (0, 0) è l'angolo inferiore sinistro
class Cursore():
    def __init__(self):
        self.x = COLONNA_1
        self.y = ALTEZZA_INIZIO - 40
        self.prima_pagina = True

cursore = Cursore()
pdf = canvas.Canvas("Sample", pagesize = A4)

def crea_report(cantiere, tasks, materiali):
    cantiere = Cantiere(cantiere, tasks, materiali)
    global pdf
    timestamp = datetime.now() 
    data = timestamp.strftime("%d-%m-%Y")
    pdf = canvas.Canvas("Report Cantiere "+ cantiere.nome + " " + data + ".pdf", pagesize = A4)
    stampa_titolo(cantiere.nome, data)
    stampa_sezione("Task Completati:", cantiere.tasks, None)
    stampa_sezione("Materiali utilizzati:", cantiere.materiali, "kg")
    pdf.save()

def stampa_titolo(nome, data):
    global pdf
    pdf.setFont("Helvetica-Bold", 16)
    pdf.drawString(COLONNA_1, ALTEZZA_INIZIO, "Report Cantiere " + nome)
    pdf.setFont("Helvetica", 12)
    pdf.drawString(COLONNA_1, ALTEZZA_INIZIO - 20, data)

def stampa_sezione(titolo, collezione, unità):
    stampa_intestazione(titolo)
    if isinstance(collezione, dict):
        stampa_dizionario(collezione, unità)
    elif isinstance(collezione, list):
        stampa_lista(collezione)

def stampa_intestazione(titolo):
    global cursore, pdf
    cursore.y -= INTERLINEA_TITOLO
    pdf.setFont("Helvetica-Bold", 14)
    pdf.drawString(cursore.x, cursore.y, titolo)
    cursore.y -= INTERLINEA

def stampa_lista(lista):
    global cursore, pdf
    pdf.setFont("Helvetica", 12)
    for entrata in lista:
        aggiorna_cursore()
        if isinstance(entrata, dict):
            for chiave, valore in entrata.items():
                # Se la chiave è "Materiale", filtriamo solo i campi ammessi
                if chiave == "Materiale" and isinstance(valore, dict):
                    valore = {k: valore[k] for k in ["Nome", "Unita", "CostoUnitario"] if k in valore}

                # Stampiamo solo le chiavi consentite
                if chiave in ["Descrizione", "Data", "QuantitaUtilizzata", "Materiale"]:
                    pdf.drawString(cursore.x, cursore.y, f"- {chiave}: {valore}")
                    cursore.y -= INTERLINEA
        else:
            pdf.drawString(cursore.x, cursore.y, "- " + entrata)
            cursore.y -= INTERLINEA

def stampa_dizionario(dizionario, unità):
    global cursore, pdf
    pdf.setFont("Helvetica", 12)
    for chiave, valore in dizionario.items():
        aggiorna_cursore()
        pdf.drawString(cursore.x, cursore.y, f"- {chiave}: {valore}{unità}")
        cursore.y -= INTERLINEA

def aggiorna_cursore():
    global cursore, pdf
    if cursore.y <= ALTEZZA_FINE:
        if cursore.x == COLONNA_1:
            cursore.x = COLONNA_2
            cursore.y = ALTEZZA_INIZIO - 70 if cursore.prima_pagina else ALTEZZA_INIZIO
        elif cursore.x == COLONNA_2:
            pdf.showPage()
            cursore.x = COLONNA_1
            cursore.y = ALTEZZA_INIZIO
            cursore.prima_pagina = False

class Cantiere():
    def __init__(self, nome, tasks=None, materiali=None):
        self.nome = nome
        self.tasks = tasks if tasks is not None else []
        self.materiali = materiali if materiali is not None else {}
    
    def aggiungi_task(self, task):
        if len(task) > 42:
            task = task[:42] + "..."
        self.tasks.append(task)

    def aggiungi_materiale(self, materiale, quantità):
        if materiale in self.materiali:
            self.materiali[materiale] += quantità 
        else:
            self.materiali[materiale] = quantità

    def aggiungi_costi(self, descrizione, costo):
        if descrizione in self.costi:
            self.costi[descrizione] += costo 
        else:
            self.costi[descrizione] = costo

    def totale_costi(self):
        return sum(self.costi.values())

    def totale_materiali(self):
        return sum(self.materiali.values())



def main():
    parser = argparse.ArgumentParser(description="Genera un report PDF per un cantiere.")
    parser.add_argument("cantiere", type=str, help="Nome del cantiere")
    parser.add_argument("tasks", type=str, help="Lista delle tasks in formato JSON")
    parser.add_argument("materiali", type=str, help="Dizionario dei materiali in formato JSON")

    
    args = parser.parse_args()
    
    tasks = json.loads(args.tasks)
    materiali = json.loads(args.materiali)

    
    crea_report(args.cantiere, tasks, materiali)

if __name__ == "__main__":
    main()