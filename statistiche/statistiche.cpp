#include "crow.h"
#include <iostream>
#include <vector>

using namespace std;

struct Materiale {
    int idMateriale;
    int quantita;
    double costoUnitario;
};

struct Presenza {
    int idOperaio;
    int orePresenza;
};

struct Operaio {
    int idOperaio;
    double costoOrario;
};

struct Spesa {
    int idSpesa;
    double importo;
    string descrizione;
};

template<typename T>
double calcolaSomma(const vector<T>& items);

template<>
double calcolaSomma(const vector<Materiale>& materiali) {
    double somma = 0.0;
    for (const Materiale& materiale : materiali) {
        somma += materiale.quantita * materiale.costoUnitario;
    }
    return somma;
}

template<>
double calcolaSomma(const vector<Spesa>& spese) {
    double somma = 0.0;
    for (const Spesa& spesa : spese) {
        somma += spesa.importo;
    }
    return somma;
}

double CalcolaCostoMateriali(const vector<Materiale>& materiali) {
    return calcolaSomma(materiali);
}

double CalcolaCostoPersonale(const vector<Presenza>& presenze, const vector<Operaio>& operai) {
    double costoPersonale = 0.0;
    
    for (const auto& presenza : presenze) {
        for (const auto& operaio : operai) {
            if (operaio.idOperaio == presenza.idOperaio) {
                costoPersonale += presenza.orePresenza * operaio.costoOrario;
                break;
            }
        }
    }
    
    return costoPersonale;
}

double CalcolaTotaleSpese(const vector<Spesa>& spese) {
    return calcolaSomma(spese);
}

int main() {
    crow::SimpleApp app;

    CROW_ROUTE(app, "/calcolaStatistiche").methods("POST"_method)
    ([](const crow::request& req) {
        try {
            auto body = crow::json::load(req.body);
            if (!body)
                return crow::response(400, "Errore: JSON non valido");

            string nomeCantiere = body["cantiere"].s();

            vector<Materiale> materiali;
            for (const auto& item : body["materiali"]) {
                if (!item["Materiale"].has("IdMateriale") || !item["Materiale"].has("CostoUnitario") || !item.has("QuantitaUtilizzata")) {
                    return crow::response(400, "Errore: JSON non valido per materiali");
                }
                Materiale materiale;
                materiale.idMateriale = item["Materiale"]["IdMateriale"].i();
                materiale.quantita = item["QuantitaUtilizzata"].i();
                materiale.costoUnitario = item["Materiale"]["CostoUnitario"].d();
                materiali.push_back(materiale);
            }

            vector<Operaio> operai;
            for (const auto& item : body["operai"]) {
                if (!item.has("IdOperaio") || !item.has("CostoOrario")) {
                    return crow::response(400, "Errore: JSON non valido per operai");
                }
                Operaio operaio;
                operaio.idOperaio = item["IdOperaio"].i();
                operaio.costoOrario = item["CostoOrario"].d();
                operai.push_back(operaio);
            }
            
            vector<Presenza> presenze;
            for (const auto& item : body["presenze"]) {
                if (!item.has("OperaioId") || !item.has("Ore")) {
                    return crow::response(400, "Errore: JSON non valido per presenze");
                }
                Presenza presenza;
                presenza.idOperaio = item["OperaioId"].i();
                presenza.orePresenza = item["Ore"].i();
                presenze.push_back(presenza);
            }
            
            vector<Spesa> spese;
            for (const auto& item : body["spese"]) {
                if (!item.has("IdSpesa") || !item.has("Costo") || !item.has("Descrizione")) {
                    return crow::response(400, "Errore: JSON non valido per spese");
                }
                Spesa spesa;
                spesa.idSpesa = item["IdSpesa"].i();
                spesa.importo = item["Costo"].d();
                spesa.descrizione = item["Descrizione"].s();
                spese.push_back(spesa);
            }

            double costoMateriali = CalcolaCostoMateriali(materiali);
            double costoPersonale = CalcolaCostoPersonale(presenze, operai);
            double speseCantiere = CalcolaTotaleSpese(spese);

            crow::json::wvalue response;
            response["costoMateriali"] = costoMateriali;
            response["costoPersonale"] = costoPersonale;
            response["speseCantiere"] = speseCantiere;
            response["totale"] = costoMateriali + costoPersonale + speseCantiere;

            return crow::response{response};
        } catch (const exception& e) {
            cerr << "Errore: " << e.what() << endl;
            cerr << "JSON ricevuto: " << req.body << endl;
            return crow::response(500, string("Errore interno del server: ") + e.what());
        }
    });

    app.port(5002).multithreaded().run();
}