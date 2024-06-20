// Observing Conditions driver for Arduino Uno and Nano with
// 3 sensors. BMP280 for Pressure I2C, HTU21D for Temperature and Humidity I2C,
// TSL237 for SQM with 15º lens.

#include <Arduino.h>
#include <Math.H>
#include  <Adafruit_BMP280.h>   // Edited to respond to address 0X76
// #include <HTU21D.h>   // responds to address 0X40
#include <Adafruit_HTU21DF.h>
#include <FreqMeasure.h>  //  * Copyright (c) 2011 PJRC.COM, LLC - Paul Stoffregen <paul@pjrc.com>
// #include "Config.h"       //  * Copyrigh, (c) 2011 PJRC.COM, LLC - Paul Stoffregen <paul@pjrc.com>
#include "TSL235R.h"      //  * Copyright Rob Tillaart, https://github.com/RobTillaart/TSL235R

TSL235R  mySensor;
#define SQM_SAMPLES 5
#define SQM_AVG_DELAY 5
#define SQM_LIMIT 13.51   // Calibration factor ( 14.15 with time limit ) to establish against ASTAP values or with SQM-L
#define Pedestal 0


// Address of BMP280 is 0X76 checked with I2C-check sketch

Adafruit_BMP280 bmp; // I2C Interface
Adafruit_HTU21DF htu = Adafruit_HTU21DF(); // I2C interface

int Query, First_Connect;
float hum, temp, dewpt;
volatile double last_SQM_val; 
volatile double last_Temp, last_Dew, last_Hum, last_Press;



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
  pinMode(8, INPUT_PULLUP);         // Rob Tillaart
  mySensor.setWavelength(450);      // Rob Tillaart

  last_SQM_val = 18.50;
  last_Temp = 15.0;
  last_Hum=45.56;
  last_Dew=10.51;
  last_Press=985.15;
  First_Connect = 1;   // Start from 0 when connecting NINA/... to NANO ObsCon
//  Take_Measurements();
}

void loop() {
  String CMD = "";

//  last_Hum = htu.readHumidity();
//  last_Temp = htu.readTemperature();
//  last_Dew = (last_Temp-(100-last_Hum)/5);
//  last_Press = (bmp.readPressure()/100);
//  First_connection();  // SQM value

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
        last_Dew = (last_Temp-(100-last_Hum)/5);
        Serial.print(last_Dew);
        Serial.print("#");
        last_Temp = htu.readTemperature();
        delay(500);
        last_Hum = htu.readHumidity();
        Serial.flush();
        break;
      case 2:
        last_Press = (bmp.readPressure()/100);
        Serial.print(last_Press);
        Serial.print("#");
        Serial.flush();      
        break;
      case 5:
        Serial.print(last_SQM_val);
        Serial.print("#");
        Serial.flush();     
        break;
      case 6:  
        Serial.print (last_Temp);
        Serial.print("#");      
        Serial.flush();
 //       Take_Measurements();
        First_connection();
        break;
      case 7:
        Serial.print(last_Hum);
        Serial.print("#");
        Serial.flush();
        last_Hum = htu.readHumidity();
        break;
      default:
      break;
    }
   }
}


double First_connection() {
      if ( First_Connect == 0 ) {
          if ( last_SQM_val > 17 ) {
            last_SQM_val = sqm_dark();
          }
          if ( ( last_SQM_val <= 17 ) && ( last_SQM_val > 10 ) ) {
              last_SQM_val = sqm();
          }
          if ( last_SQM_val <= 10 ) {
              last_SQM_val = sqm_daytime();
          }
      } else {
  //      Serial.println("First connection, now set to 0" );
          First_Connect = 0;            // First connect was 1
          if ( last_SQM_val > 17 ) {
            last_SQM_val = sqm_dark();
          }
          if ( ( last_SQM_val <= 17 ) && ( last_SQM_val > 10 ) ) {
              last_SQM_val = sqm();
          }
          if ( last_SQM_val <= 10 ) {
              last_SQM_val = sqm_daytime();
          }
       }
}

double sqm_daytime() {
//  Serial.println("SQM daytime");
   uint16_t cnt = 0;
   uint32_t start = micros();
    while (cnt < 1000 )  
    {
      while (digitalRead(8) == HIGH);  // wait for LOW
      cnt++;
      while (digitalRead(8) == LOW);   // wait for HIGH
    }
  uint32_t duration = micros() - start;
  float freq = (1e6 * cnt) / duration;
  return ( SQM_LIMIT - ( 2.5 * log10(freq/100)));
}

double sqm() {
//  Serial.println("SQM afternoon");
   uint16_t cnt = 0;
   uint32_t start = micros();
    while (cnt < 50 )  
    {
      while (digitalRead(8) == HIGH);  // wait for LOW
      cnt++;
      while (digitalRead(8) == LOW);   // wait for HIGH
    }
  uint32_t duration = micros() - start;
  float freq = (1e6 * cnt) / duration;
  return ( SQM_LIMIT - ( 2.5 * log10(freq/100)));
}

double sqm_dark() {
//    Serial.println("SQM dark");
   uint16_t cnt = 0;
   uint32_t start = micros();
    while ( cnt < 3 )   
    {
      while (digitalRead(8) == HIGH);  // wait for LOW
      cnt++;
      while (digitalRead(8) == LOW);   // wait for HIGH
    }
  uint32_t duration = micros() - start;
  float freq = (1e6 * cnt) / duration;
  return ( SQM_LIMIT - ( 2.5 * log10(freq/100)));
}

void Take_Measurements() {
//  last_Hum = htu.readHumidity();
//  last_Temp = htu.readTemperature();
//  last_Dew = (last_Temp-(100-last_Hum)/5);
//  last_Press = (bmp.readPressure()/100);
  First_connection();  // SQM value
  }
