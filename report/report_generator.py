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

def crea_report(cantiere):
    global pdf
    timestamp = datetime.now() 
    data = timestamp.strftime("%d-%m-%Y")
    pdf = canvas.Canvas("Report Cantiere "+ cantiere.nome + " " + data + ".pdf", pagesize = A4)
    stampa_titolo(cantiere.nome, data)
    stampa_sezione("Task Completati:", cantiere.tasks, None)
    stampa_sezione("Materiali utilizzati:", cantiere.materiali, "kg")
    stampa_sezione("Costi:", cantiere.costi, "€")
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
    def __init__(self, nome):
        self.nome = nome
        self.tasks = []
        self.materiali = {}
        self.costi = {}
    
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

cantiere_sofia = Cantiere("Via Santa Sofia")

for i in range(10):
    cantiere_sofia.aggiungi_task("Finito il pavimento del bagno. Commento troncato")
    cantiere_sofia.aggiungi_task("Finito il muro della sala da pranzo.")
    cantiere_sofia.aggiungi_task("Finito il tetto del garage.")

cantiere_sofia.aggiungi_materiale("Acciaio", 10)
cantiere_sofia.aggiungi_materiale("Cemento", 20)
cantiere_sofia.aggiungi_materiale("Vetro", 30)

cantiere_sofia.aggiungi_costi("Benzina", 10)
cantiere_sofia.aggiungi_costi("Cavi", 20)
cantiere_sofia.aggiungi_costi("Mattonelle", 30)

crea_report(cantiere_sofia)