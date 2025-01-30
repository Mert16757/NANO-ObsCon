// Observing Conditions driver for Arduino Uno and Nano with
// 3 sensors. BMP280 for Pressure I2C, HTU21D for Temperature and Humidity I2C,
// TSL237 for SQM with 15º lens.

#include <Arduino.h>
#include <Math.H>
#include  <Adafruit_BMP280.h>   // Edited to respond to address 0X76
// #include <HTU21D.h>   // responds to address 0X40
#include <Adafruit_HTU21DF.h>
#include "FreqMeasure.h"  //  * Copyright (c) 2011 PJRC.COM, LLC - Paul Stoffregen <paul@pjrc.com>
// #include "Config.h"       //  * Copyrigh, (c) 2011 PJRC.COM, LLC - Paul Stoffregen <paul@pjrc.com>
#include "TSL235R.h"      //  * Copyright Rob Tillaart, https://github.com/RobTillaart/TSL235R

TSL235R  mySensor;
#define SQM_SAMPLES 5
#define SQM_AVG_DELAY 5
#define SQM_LIMIT 13.55   // Calibration factor ( 14.35 with time limit ) to establish against ASTAP values or with SQM-
#define Pedestal 0


// Address of BMP280 is 0X76 checked with I2C-check sketch

Adafruit_BMP280 bmp; // I2C Interface
Adafruit_HTU21DF htu = Adafruit_HTU21DF(); // I2C interface

int Query, First_Connect;
float hum, temp, dewpt;
double Promedia, Last_Value, Current_Value;
double altura, val, last_SQM_val; 

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
  First_Connect = 1;   // Start from 0 when connecting NINA/... to NANO ObsCon
  
}

void loop() {
  String CMD = "";
// Here the looping commands gathering data to be read out
//   sqm_dark(); 
   
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
        last_SQM_val= sqm_dark();
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
        val = last_SQM_val;
        // if ( First_Connect == 1 ){
        //  First_Connect = 0;
        // }
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
//    if ( First_Connect == 0 ) {
//          last_SQM_val = sqm_dark();    // We read the value of SQM because it can take up to 35 seconds when very dark
//    }
//    if ( last_SQM_val > 16 ) {
//      last_SQM_val = sqm_dark();
//    } else {
//      last_SQM_val= sqm();
//    }
   }
}   

double sqm() {
   uint16_t cnt = 0;
   uint32_t start = micros();
    while (cnt < 10 )  
    {
      while (digitalRead(8) == HIGH);  // wait for LOW
      cnt++;
      while (digitalRead(8) == LOW);   // wait for HIGH
    }
  uint32_t duration = micros() - start;
  float freq = (1e6 * cnt) / duration;
//  last_SQM_value = ( SQM_LIMIT - ( 2.5 * log10(freq/100)));
  return ( SQM_LIMIT - ( 2.5 * log10(freq/100)));
}

double sqm_dark() {
   uint16_t cnt = 0;
   uint32_t start = micros();
    while ( cnt < 2 )   
    {
      while (digitalRead(8) == HIGH);  // wait for LOW
      cnt++;
      while (digitalRead(8) == LOW);   // wait for HIGH
//      cnt++;                           // added to half the wait time
    }
  uint32_t duration = micros() - start;
  float freq = (1e6 * cnt) / duration;  
//  float freq = (1e6 * cnt/2) / duration;  // divide by 2 to compensate for half the wait time
//  last_SQM_value = ( SQM_LIMIT - ( 2.5 * log10(freq/100)));
  return ( SQM_LIMIT - ( 2.5 * log10(freq/100)));
}
