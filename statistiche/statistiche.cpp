#include "crow.h"
#include <mysql.h>
#include <iostream>
#include <vector>

struct Materiale {
    int idMateriale;
    int quantita;
    double costoUnitario;
};

// Funzione per calcolare il costo totale dei materiali
double CalcolaCostoTotale(const std::vector<Materiale>& materiali) {
    double costoTotale = 0.0;
    for (const auto& materiale : materiali) {
        costoTotale += materiale.quantita * materiale.costoUnitario;
    }
    return costoTotale;
}

int main() {
    crow::SimpleApp app;

    // Endpoint per calcolare il costo totale
    CROW_ROUTE(app, "/calcolaCostoMateriali").methods("POST"_method)
    ([](const crow::request& req) {
        auto body = crow::json::load(req.body);
        if (!body)
            return crow::response(400, "Errore: JSON non valido");

        std::vector<Materiale> materiali;
        for (const auto& item : body["materiali"]) {
            Materiale materiale;
            materiale.idMateriale = item["Materiale"]["IdMateriale"].i();
            materiale.quantita = item["QuantitaUtilizzata"].i();
            materiale.costoUnitario = item["Materiale"]["CostoUnitario"].d();
            materiali.push_back(materiale);
        }

        double costoTotale = CalcolaCostoTotale(materiali);

        crow::json::wvalue response;
        response = costoTotale;
        return crow::response{response};
    });

    app.port(5002).multithreaded().run();
}

