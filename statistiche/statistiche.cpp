#include <crow.h>
#include <mysql_driver.h>
#include <mysql_connection.h>
#include <cppconn/prepared_statement.h>
#include <cppconn/resultset.h>
#include <nlohmann/json.hpp>
#include <iostream>

using json = nlohmann::json;
using namespace sql;

const std::string DB_HOST = "tcp://127.0.0.1:3307"; // Porta 3307 perché il DB è mappato così
const std::string DB_USER = "root";
const std::string DB_PASS = "1234";
const std::string DB_NAME = "SiteManager";

int main() {
    crow::SimpleApp app;

    CROW_ROUTE(app, "/statistiche").methods(crow::HTTPMethod::POST)
    ([](const crow::request& req) {
        auto body = json::parse(req.body);
        if (!body.contains("cantiere")) {
            return crow::response(400, R"({"status": "error", "message": "Cantiere non specificato"})");
        }

        std::string cantiere = body["cantiere"];

        try {
            // Connessione al database
            sql::mysql::MySQL_Driver* driver = sql::mysql::get_mysql_driver_instance();
            std::unique_ptr<sql::Connection> conn(driver->connect(DB_HOST, DB_USER, DB_PASS));
            conn->setSchema(DB_NAME);

            // Query per ottenere i materiali e il costo
            std::unique_ptr<sql::PreparedStatement> stmt(conn->prepareStatement(
                "SELECT materiale, costo FROM Materiali WHERE cantiere = ?"
            ));
            stmt->setString(1, cantiere);

            std::unique_ptr<sql::ResultSet> res(stmt->executeQuery());

            json response;
            response["status"] = "success";
            json materiali = json::array();

            while (res->next()) {
                json entry;
                entry["materiale"] = res->getString("materiale");
                entry["costo"] = res->getDouble("costo");
                materiali.push_back(entry);
            }

            response["materiali"] = materiali;
            return crow::response(200, response.dump());
        } catch (sql::SQLException& e) {
            std::cerr << "Errore SQL: " << e.what() << std::endl;
            return crow::response(500, R"({"status": "error", "message": "Errore del database"})");
        }
    });

    app.port(5002).multithreaded().run();
}