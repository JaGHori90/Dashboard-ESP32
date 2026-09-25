#include <Arduino.h>
#include <Wire.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BME280.h>
#include <WiFi.h>
#include <WiFiClientSecure.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include "secrets.h" 
#include <esp_task_wdt.h>


const char* API_URL   = "https://webapi20260907135900-a8g7dybugngfh0bk.westus3-01.azurewebsites.net/api/Measurments/Post";

#define BUTTON_PIN 27

const unsigned long INTERVAL = 900000; // 15 Minuten zwischen normalen Messungen
const unsigned long WIFI_RECONNECT_TIMEOUT = 15000; // 15 Sekunden pro Verbindungsversuch
const unsigned long MAX_TIME_WITHOUT_SUCCESS = 3600000; // 1 Stunde ohne erfolgreichen Versand -> Neustart
const uint32_t WDT_TIMEOUT_SECONDS = 30;

unsigned long lastSendTime = 0;
unsigned long lastSuccessTime = 0;

Adafruit_BME280 bme;
bool sensorOK = false;
WiFiClientSecure secureClient;

bool ensureWifiConnected() {
  if (WiFi.status() == WL_CONNECTED) return true;

  Serial.println("WLAN getrennt, verbinde neu...");
  WiFi.disconnect(true, true);
  delay(200);
  WiFi.mode(WIFI_STA);
  WiFi.begin(WIFI_SSID, WIFI_PASS);

  unsigned long start = millis();
  while (WiFi.status() != WL_CONNECTED && millis() - start < WIFI_RECONNECT_TIMEOUT) {
    delay(500);
  }

  return WiFi.status() == WL_CONNECTED;
}

bool sendDataToServer(float temp, float hum, float press) {
  if (!ensureWifiConnected()) {
    Serial.println("Fehler: Kein WLAN vorhanden!");
    return false;
  }

  HTTPClient http;
  http.setConnectTimeout(10000);
  http.setTimeout(10000);
  http.begin(secureClient, API_URL);
  http.addHeader("Content-Type", "application/json");
  http.addHeader("X-Api-Key", API_KEY);
  
  JsonDocument doc;
  doc["sensorId"]    = 2;
  doc["temperature"] = temp;
  doc["humidity"]    = hum;
  doc["airPressure"]    = press;

  String jsonString;
  serializeJson(doc, jsonString);

  Serial.print("Sende JSON an API: ");
  Serial.println(jsonString);

  int httpResponseCode = http.POST(jsonString);
  bool success = httpResponseCode == 201 || httpResponseCode == 200;

  if (success) {
    Serial.printf("HTTP Antwort-Code vom Server: %d\n", httpResponseCode);
  } else if (httpResponseCode > 0) {
    Serial.printf("Unerwarteter HTTP Antwort-Code: %d\n", httpResponseCode);
  } else {
    Serial.print("HTTP Fehler: ");
    Serial.println(http.errorToString(httpResponseCode));
  }

  http.end();
  return success;
}

void measureAndSend() {
  if (!sensorOK) {
    Serial.println("Sensor nicht initialisiert");
    return;
  }

  float temp       = bme.readTemperature();
  float humidity    = bme.readHumidity();
  float airPressure = bme.readPressure() / 100.0f;

  Serial.printf("\nTemperatur: %.2f°C", temp);
  Serial.printf("\nLuftfeuchtigkeit: %.2f%%", humidity);
  Serial.printf("\nDruck: %.2f hPa\n", airPressure);

  if (sendDataToServer(temp, humidity, airPressure)) {
    lastSuccessTime = millis();
  }
}

void setup() {
  Serial.begin(115200);
  delay(1000);

  pinMode(BUTTON_PIN, INPUT_PULLUP);

  secureClient.setInsecure();

  WiFi.mode(WIFI_STA);
  WiFi.setTxPower(WIFI_POWER_11dBm);
  WiFi.begin(WIFI_SSID, WIFI_PASS);

  unsigned long wifiStart = millis();
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
    if (millis() - wifiStart > WIFI_RECONNECT_TIMEOUT) {
      Serial.println("\nWLAN Verbindung fehlgeschlagen, Neustart ...");
      ESP.restart();
    }
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
  lastSuccessTime = millis();

  esp_task_wdt_init(WDT_TIMEOUT_SECONDS,true);
  esp_task_wdt_add(NULL); 

}

void loop() {
  esp_task_wdt_reset();

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

  if (currentMillis - lastSuccessTime > MAX_TIME_WITHOUT_SUCCESS) {
    Serial.println("Zu lange kein erfolgreicher Versand, Neustart ...");
    ESP.restart();
  }
}
