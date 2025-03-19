#include "crow.h"
#include <mysql.h>
#include <iostream>
#include <vector>

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
    std::string descrizione;
};

double CalcolaCostoMateriali(const std::vector<Materiale>& materiali) {
    double costoTotale = 0.0;
    for (const auto& materiale : materiali) {
        costoTotale += materiale.quantita * materiale.costoUnitario;
    }
    return costoTotale;
}

double CalcolaCostoPersonale(const std::vector<Presenza>& presenze, const std::vector<Operaio>& operai) {
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

double CalcolaTotaleSpese(const std::vector<Spesa>& spese) {
    double totaleSpese = 0.0;
    for (const auto& spesa : spese) {
        totaleSpese += spesa.importo;
    }
    return totaleSpese;
}

int main() {
    crow::SimpleApp app;

    CROW_ROUTE(app, "/calcolaStatistiche").methods("POST"_method)
    ([](const crow::request& req) {
        try {
            auto body = crow::json::load(req.body);
            if (!body)
                return crow::response(400, "Errore: JSON non valido");

            std::string nomeCantiere = body["cantiere"].s();

            std::vector<Materiale> materiali;
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

            std::vector<Operaio> operai;
            for (const auto& item : body["operai"]) {
                if (!item.has("IdOperaio") || !item.has("CostoOrario")) {
                    return crow::response(400, "Errore: JSON non valido per operai");
                }
                Operaio operaio;
                operaio.idOperaio = item["IdOperaio"].i();
                operaio.costoOrario = item["CostoOrario"].d();
                operai.push_back(operaio);
            }
            
            std::vector<Presenza> presenze;
            for (const auto& item : body["presenze"]) {
                if (!item.has("OperaioId") || !item.has("Ore")) {
                    return crow::response(400, "Errore: JSON non valido per presenze");
                }
                Presenza presenza;
                presenza.idOperaio = item["OperaioId"].i();
                presenza.orePresenza = item["Ore"].i();
                presenze.push_back(presenza);
            }
            
            std::vector<Spesa> spese;
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
        } catch (const std::exception& e) {
            std::cerr << "Errore: " << e.what() << std::endl;
            std::cerr << "JSON ricevuto: " << req.body << std::endl;
            return crow::response(500, std::string("Errore interno del server: ") + e.what());
        }
    });

    app.port(5002).multithreaded().run();
}