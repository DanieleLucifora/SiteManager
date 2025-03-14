import json
import sys
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
    stampa_intestazione("Task Completati:")
    stampa_lista(cantiere.tasks)
    stampa_intestazione("Materiali utilizzati:")
    stampa_lista_dizionari(cantiere.materiali)
    pdf.save()

def stampa_titolo(nome, data):
    global pdf
    pdf.setFont("Helvetica-Bold", 16)
    pdf.drawString(COLONNA_1, ALTEZZA_INIZIO, "Report Cantiere " + nome)
    pdf.setFont("Helvetica", 12)
    pdf.drawString(COLONNA_1, ALTEZZA_INIZIO - 20, data)

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

def stampa_lista_dizionari(lista_dizionari):
    global cursore, pdf
    pdf.setFont("Helvetica", 12)
    for dizionario in lista_dizionari:
        valori = list(dizionario.values())  #lista per accedere ai campi
        aggiorna_cursore()
        pdf.drawString(cursore.x, cursore.y, f"- {valori[0]} {valori[1]}{valori[2]}")
        cursore.y -= INTERLINEA

def stampa_dizionario(dizionario):   # per i costi 
    global cursore, pdf
    pdf.setFont("Helvetica", 12)
    for chiave, valore in dizionario.items():
        aggiorna_cursore()
        pdf.drawString(cursore.x, cursore.y, f"- {chiave}: {valore}")
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
        self.materiali = [] #lista di dizionari
        self.costi = {}
    
    def aggiungi_task(self, task):
        if len(task) > 42:
            task = task[:42] + "..."
        self.tasks.append(task)

    def aggiungi_materiale(self, nome, quantità, unità):               #è necessario non avere duplicati passati
            nuovo_materiale = {"Nome": nome, "Quantità": quantità, "Unità": unità}
            self.materiali.append(nuovo_materiale)      #Risultato: {Materiale: Metallo, Quantità 10, Unità: kg}

    def aggiungi_costi(self, descrizione, costo):
        if descrizione in self.costi:
            self.costi[descrizione] += costo 
        else:
            self.costi[descrizione] = costo

    def totale_costi(self):
        return sum(self.costi.values())

def main():    
    nome_cantiere = sys.argv[1]                         # sys.argv è la lista (vettore) degli argomenti passati tramite riga di comando
    lista_tasks_json = json.loads(sys.argv[2])          # è una lista di dizionari {'IdTasks': 0, 'Descrizione': "Rifare il tetto dell'aula D34", 'Data': etc...}
    lista_materiali_json = json.loads(sys.argv[3])      # è una lista di dizionari. In ogni dizionario il valore associato alla chiave "Materiale" è a sua volta un dizionario
                                                        # [{'IdMaterialeCantiere': 1, 'IdCantiere': 1, 'IdMateriale': 1, 'QuantitaUtilizzata': 10, 'Cantiere': None, 
                                                        # 'Materiale': {'IdMateriale': 1, 'Nome': 'Metallo', 'Quantita': 0, 'Unita': 'kg', 'CostoUnitario': 20.0}}, ...]
    cantiere = Cantiere(nome_cantiere)

    for dizionario in lista_tasks_json:
        for chiave, valore in dizionario.items():
            if chiave == "Descrizione":
                cantiere.aggiungi_task(valore)

    for dizionario in lista_materiali_json:
        for chiave, valore in dizionario.items():
            if chiave == "QuantitaUtilizzata":
                quantità = valore
            if chiave == "Materiale":
                for c, v in valore.items():             # itero il dizionario memorizzato come valore della chiave 'Materiale'
                    if c == "Nome":
                        nome = v
                    if c == "Unita":
                        unità = v   
        cantiere.aggiungi_materiale(nome, quantità, unità)

    crea_report(cantiere)

if __name__ == "__main__":
    main()