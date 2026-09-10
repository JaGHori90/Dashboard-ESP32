#include <Arduino.h>
#include <Wire.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BME280.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>


const char* WIFI_SSID = "home";
const char* WIFI_PASS = "Somaye@liebe";
const char* API_URL   = "https://webapi20260907135900-a8g7dybugngfh0bk.westus3-01.azurewebsites.net/api/Measurments/Post";

#define LED_PIN 5
#define BUTTON_PIN 27

const unsigned long INTERVAL = 900000; 
unsigned long lastSendTime = 0;
const unsigned long WIFI_RECONNECT_TIMEOUT = 15000; // 15 seconds

Adafruit_BME280 bme; 
bool sensorOK = false;

void blinkCode(int times){
  for(int i=0; i<times; i++){
    digitalWrite(LED_PIN,HIGH);
    delay(150);
    digitalWrite(LED_PIN,LOW);
    delay(150);
  }
}

bool ensureWifiConnected(){
  if(WiFi.status() == WL_CONNECTED) return true;

  Serial.println("Verbinde mit WLAN...");
  WiFi.disconnect();
  WiFi.reconnect();

  unsigned long start = millis();
  while (WiFi.status() != WL_CONNECTED && millis() - start < WIFI_RECONNECT_TIMEOUT)
  {
    delay(500);
  }

  return WiFi.status() == WL_CONNECTED;
}

void sendDataToServer(float temp, float hum, float press) {
  if (!ensureWifiConnected()) {
    Serial.println("Fehler: Kein WLAN vorhanden!");
    blinkCode(2);
    return;
  }

  HTTPClient http;
  http.begin(API_URL);
  http.addHeader("Content-Type", "application/json");

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

  if (httpResponseCode == 201 || httpResponseCode == 200) {
    Serial.printf("HTTP Antwort-Code vom Server: %d\n", httpResponseCode);
    blinkCode(1);
  } else if(httpResponseCode > 0){
    blinkCode(4);
  }else {
    Serial.print("HTTP Fehler: ");
    Serial.println(http.errorToString(httpResponseCode));
    blinkCode(3);
  }

  http.end();
}

// Nur EINMAL definieren:
void measureAndSend() {
  digitalWrite(LED_PIN, HIGH);

  if (sensorOK) {
    float temp     = bme.readTemperature();
    float humidity = bme.readHumidity();
    float airPressure = bme.readPressure() / 100.0f; 

    Serial.printf("\nTemperatur: %.2f°C", temp);
    Serial.printf("\nLuftfeuchtigkeit: %.2f%%", humidity);
    Serial.printf("\nDruck: %.2f hPa\n", airPressure);

    sendDataToServer(temp, humidity, airPressure);
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
  WiFi.setTxPower(WIFI_POWER_11dBm);
  WiFi.begin(WIFI_SSID, WIFI_PASS);

  unsigned long wifiStart = millis();

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
    if(millis() - wifiStart > WIFI_RECONNECT_TIMEOUT){
      Serial.println("WLAN Verbindung fehgeschlagen, Neustart ...");
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