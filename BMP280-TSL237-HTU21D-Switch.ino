// Observing Conditions driver for Arduino Uno and Nano with
// 3 sensors. BMP280 for Pressure I2C, HTU21D for Temperature and Humidity I2C,
// TSL237 for SQM with 15º lens.

#include <Arduino.h>
#include <Math.H>
#include  <Adafruit_BMP280.h>   // Editado para ponder 0X76
// #include <HTU21D.h>   // responde a dirección 0X40
#include <Adafruit_HTU21DF.h>
#include <FreqMeasure.h>
#include "Config.h"


// Direccion BMP280 es 0X76 comprobado con I2C-check

Adafruit_BMP280 bmp; // I2C Interface
Adafruit_HTU21DF htu = Adafruit_HTU21DF(); // I2C interface

int Query;
float hum, temp, dewpt;
double altura, val; 

void setup() { 
  Serial.begin(9600);
     if (!htu.begin()) {
        Serial.println(F("Could not find a valid HTU21D sensor,  check wiring!")); 
//        while (1);   
     }
     if  (!bmp.begin()) {
        Serial.println(F("Could not find a valid BMP280 sensor,  check wiring!"));
//        while (1);
     }

  /* Default settings from datasheet.  */
  bmp.setSampling(Adafruit_BMP280::MODE_NORMAL,     /* Operating Mode. */
                  Adafruit_BMP280::SAMPLING_X2,     /* Temp. oversampling */
                  Adafruit_BMP280::SAMPLING_X16,    /* Pressure oversampling */
                  Adafruit_BMP280::FILTER_X16,      /* Filtering. */
                  Adafruit_BMP280::STANDBY_MS_500);  /* Standby time. */

  pinMode( LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, LOW);

}

void loop() {
  String CMD = "";
  if (Serial.available() >0 ){
    CMD = Serial.readStringUntil('#');
    CMD.trim();
    Query = CMD.toInt();
    switch ( Query ){
      case 0:
        Serial.print("ArduinoNANO");
        Serial.print("#"); 
        Serial.flush();
        break;
      case 1:
        hum = htu.readHumidity(); //Leemos la Humedad
        delay(100);
        temp = htu.readTemperature(); //Leemos la temperatura en grados Celsius
        delay(100);
        dewpt = (temp-(100-hum)/5);
        Serial.print(dewpt);
        Serial.print("#");
        Serial.flush();
        break;
      case 2:
        Serial.print(bmp.readPressure()/100);  //displaying the Pressure in hPa, you can change the unit
        Serial.print("#");
        Serial.flush();      
        break;
      case 3:
        Serial.print(bmp.readAltitude(1013.25));  //The "1013.2" is the pressure(hPa) at sea level in day in your region 030724 994.85 to reach 459 m
        Serial.print(" metros#  "); 
        Serial.flush();   
        break;
      case 4:
        Serial.print(bmp.readTemperature());
        Serial.print("#");
        Serial.flush();     
        break;
      case 5:
        val = sqm();
        Serial.print(val);
        Serial.print("#");
        Serial.flush();     
        break;
      case 6:
        Serial.print(htu.readTemperature());
        Serial.print("#");      
        Serial.flush();
        break;
      case 7:
        Serial.print(htu.readHumidity());
        Serial.print("#");
        Serial.flush();
        break;
      default:
      break;
    }
   }
}   

double sqm() {
    FreqMeasure.begin();
    int count = 0;
    double sum = 0.0;
    while (count < SQM_SAMPLES) {
        if (FreqMeasure.available()) {
            sum += FreqMeasure.read();
            count++;
            delay(SQM_AVG_DELAY);
        }
    }
    FreqMeasure.end();
    double frequency = F_CPU / (sum / count);
    return SQM_LIMIT - 2.5 * log10(frequency);
}
