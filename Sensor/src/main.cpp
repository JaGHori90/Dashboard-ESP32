#include <Arduino.h>
#include <Wire.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BME280.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>

const char* WIFI_SSID = "home";
const char* WIFI_PASS = "Somaye@liebe";
// Lokale IP des Rechners, auf dem die Database-API (ASP.NET) läuft, plus Port aus launchSettings.json.
// Beispiel: "http://192.168.1.42:5226/api/sensorreadings" -- Adresse bei Bedarf anpassen.
const char* API_URL   = "http://192.168.1.42:5226/api/sensorreadings";

#define LED_PIN 5
#define BUTTON_PIN 27

const unsigned long INTERVAL = 300000; // 5 Minuten
unsigned long lastSendTime = 0;

Adafruit_BME280 bme; 
bool sensorOK = false;

void sendDataToServer(float temp, float hum, float press) {
  if (WiFi.status() != WL_CONNECTED) {
    Serial.println("Fehler: Kein WLAN vorhanden!");
    return;
  }

  HTTPClient http;
  http.begin(API_URL);
  http.addHeader("Content-Type", "application/json");

  JsonDocument doc;
  doc["deviceId"]    = "esp32-balcony";
  doc["temperature"] = temp;
  doc["humidity"]    = hum;
  doc["pressure"]    = press;

  String jsonString;
  serializeJson(doc, jsonString);

  Serial.print("Sende JSON an API: ");
  Serial.println(jsonString);

  int httpResponseCode = http.POST(jsonString);

  if (httpResponseCode == 201) {
    // Die API antwortet mit 201 Created nur, wenn der Wert tatsächlich in der
    // Datenbank gespeichert wurde (siehe SensorReadingsController.Create).
    String response = http.getString();
    Serial.println("==> Wert wurde erfolgreich in der Datenbank gespeichert!");
    Serial.print("    Antwort vom Server: ");
    Serial.println(response);
  } else if (httpResponseCode > 0) {
    String response = http.getString();
    Serial.printf("==> WARNUNG: Server hat mit Code %d geantwortet, Wert wurde NICHT gespeichert!\n", httpResponseCode);
    Serial.print("    Antwort vom Server: ");
    Serial.println(response);
  } else {
    Serial.print("==> FEHLER: Verbindung zum Server fehlgeschlagen: ");
    Serial.println(http.errorToString(httpResponseCode));
  }

  http.end();
}

// Nur EINMAL definieren:
void measureAndSend() {
  digitalWrite(LED_PIN, HIGH);

  if (sensorOK) {
    float temp     = bme.readTemperature();
    float humidity = bme.readHumidity();
    float pressure = bme.readPressure() / 100.0f; 

    Serial.printf("\nTemperatur: %.2f°C", temp);
    Serial.printf("\nLuftfeuchtigkeit: %.2f%%", humidity);
    Serial.printf("\nDruck: %.2f hPa\n", pressure);

    sendDataToServer(temp, humidity, pressure);
  } else {
    Serial.println("Sensor nicht initialisiert");
  }

  delay(500);
  digitalWrite(LED_PIN, LOW);
}

void setup() {
  Serial.begin(115200);
  delay(1000); 

  pinMode(LED_PIN, OUTPUT);
  digitalWrite(LED_PIN, LOW); 
  pinMode(BUTTON_PIN, INPUT_PULLUP);

  WiFi.mode(WIFI_STA);
  WiFi.begin(WIFI_SSID, WIFI_PASS);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("\nWLAN Verbunden!");

  Wire.begin();
  if (bme.begin(0x76, &Wire) || bme.begin(0x77, &Wire)) {
    Serial.println("BME280 Sensor bereit.");
    sensorOK = true;
  } else {
    Serial.println("WARNUNG: BME280 nicht gefunden!");
  }

  measureAndSend();
  lastSendTime = millis();
}

void loop() {
  unsigned long currentMillis = millis();

  if (currentMillis - lastSendTime >= INTERVAL) {
    lastSendTime = currentMillis;
    measureAndSend();
  }

  if (digitalRead(BUTTON_PIN) == LOW) {
    measureAndSend();
    lastSendTime = millis(); 
    delay(500); 
  }
}